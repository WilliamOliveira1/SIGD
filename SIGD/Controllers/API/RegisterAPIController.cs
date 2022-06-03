using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SIGD.Helper;
using SIGD.Interfaces;
using SIGD.Models;
using SIGD.Services;
using System;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SIGD.Controllers.API
{
    [Route("api/register")]
    [Produces("application/json")]
    [ApiController]
    public class RegisterAPIController : Controller
    {
        private IActivationAccountRepository databaseService;
        private ITokenService tokenService;
        private IRegisterLoginService registerLoginService;
        private readonly IConfiguration _config;
        private string generatedToken = null;
        private CookieOptions httpOnlyAndSecureFlag = new CookieOptions { HttpOnly = true, Secure = true };
        private IMemoryCache lockAccount;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private const int MAX_ATTEMPT = 3;

        public RegisterAPIController(
            IActivationAccountRepository databaseService, 
            ITokenService tokenService, 
            IRegisterLoginService registerLoginService,
            IConfiguration _config,
            IMemoryCache lockAccount,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            this.registerLoginService = registerLoginService ?? throw new ArgumentNullException(nameof(registerLoginService));
            this._config = _config ?? throw new ArgumentNullException(nameof(_config));
            this.lockAccount = lockAccount ?? throw new ArgumentNullException(nameof(lockAccount));
            this._signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpPost("registernew")]
        public IActionResult RegisterNew([FromBody] ActivationAccount activationAccount)
        {
            if (activationAccount != null)
            {
                var account = registerLoginService.CreateNewAdminUser(activationAccount);
                if(account == null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, SharedMessages.ERROR_SENDING_EMAIL);
                }

                bool canSave = databaseService.Save(account, true);

                if(canSave)
                {
                    return Ok();
                }
                else
                {
                    return Forbid();
                }
            }
            else
            {
                return NotFound();
            }
        }               

        private ActivationAccount GetUser(ActivationAccount userModel)
        {
            // Write your code here to authenticate the user     
            return databaseService.GetUser(userModel);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] JObject input)//string email, string oldPassword
        {
            string username = input?["username"]?.ToString();
            string email = input?["email"]?.ToString();
            int userLoginAttempt = 0;            

            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(email))
            {
                ActivationAccount account = new ActivationAccount();
                
                if (!string.IsNullOrEmpty(email))
                {
                    account = databaseService.GetActivationAccountByEmail(email);
                }
                else if(!string.IsNullOrEmpty(username))
                {
                    account = databaseService.GetActivationAccountByUserName(username);
                }

                if(account != null)
                {
                    // time that will stay locked thee account
                    var lockAccountTime = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

                    SecureString oldPassword = new NetworkCredential("", input["password"]?.ToString()).SecurePassword;
                    bool isMatch = registerLoginService.TokenMatch(oldPassword, new NetworkCredential("", account.Password).SecurePassword);

                    if(lockAccount.TryGetValue(account.UserName, out userLoginAttempt))
                    {
                        if (userLoginAttempt == MAX_ATTEMPT)
                        {
                            return StatusCode((int)HttpStatusCode.BadRequest);
                        }
                    }

                    // TODO lock if get wrong more than 3 times
                    if (account.IsActivated && isMatch)
                    {
                        var validUser = GetUser(account);
                        if (validUser != null)
                        {
                            generatedToken = tokenService.BuildToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), validUser);
                            if (generatedToken != null)
                            {                                                            
                                await SetUser(account);
                                return StatusCode((int)HttpStatusCode.OK);
                            }
                            else
                            {
                                return StatusCode((int)HttpStatusCode.NotFound);
                            }
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.NotFound);
                        }
                    }
                    else if (!account.IsActivated && isMatch)
                    {
                        if (userLoginAttempt == MAX_ATTEMPT)
                        {
                            return StatusCode((int)HttpStatusCode.BadRequest);
                        }
                        return StatusCode((int)HttpStatusCode.OK, SharedMessages.CHANGE_FIRST_ACCESS_PASSWORD);
                    }
                    else if (!isMatch)
                    {                        
                        userLoginAttempt++;
                        lockAccount.Set(account.UserName, userLoginAttempt, lockAccountTime);
                        if (userLoginAttempt == MAX_ATTEMPT)
                        {
                            return StatusCode((int)HttpStatusCode.BadRequest);
                        }
                        return StatusCode((int)HttpStatusCode.BadRequest, SharedMessages.ERROR_PASSWORD_NOT_MATCH);
                    }                    
                    else
                    {
                        lockAccount.TryGetValue(account.UserName, out userLoginAttempt);
                        userLoginAttempt++;
                        lockAccount.Set(account.UserName, userLoginAttempt, lockAccountTime);
                        if (userLoginAttempt == MAX_ATTEMPT)
                        {
                            return StatusCode((int)HttpStatusCode.BadRequest);
                        }
                        return BadRequest();
                    }                    
                }
                else
                {                    
                    return StatusCode((int)HttpStatusCode.NotFound);
                }
                
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }
        }

        private async Task<string> SetUser(ActivationAccount account)
        {
            try
            {
                IdentityUser user = new IdentityUser { UserName = account.UserName, Email = account.Email };
                HttpContext.Session.SetString("Token", generatedToken);
                HttpContext.Session.Set(account.UserName, System.Text.Encoding.ASCII.GetBytes(account.role.ToString()));
                Response.Cookies.Append("token", $"{generatedToken}", httpOnlyAndSecureFlag);                
                await _signInManager.SignInAsync(user, isPersistent: false);                
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }            
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost("changepassword")]
        public IActionResult ChangePassword([FromBody] JObject input)
        {
            bool canSave = false;

            if (!string.IsNullOrEmpty(input?["email"]?.ToString()) || !string.IsNullOrEmpty(input?["oldpassword"]?.ToString()) 
                || !string.IsNullOrEmpty(input?["password"]?.ToString()) || !string.IsNullOrEmpty(input?["username"]?.ToString()))
            {
                ActivationAccount account = new ActivationAccount();
                string username = input?["username"]?.ToString();
                string email = input?["email"]?.ToString();
                if (!string.IsNullOrEmpty(email))
                {
                    account = databaseService.GetActivationAccountByEmail(email);
                }
                else if (!string.IsNullOrEmpty(username))
                {
                    account = databaseService.GetActivationAccountByUserName(username);
                }
                
                if (account != null)
                {
                    SecureString oldPassword = new NetworkCredential("", input["oldpassword"]?.ToString()).SecurePassword;
                    SecureString newPassword = new NetworkCredential("", input["password"]?.ToString()).SecurePassword;
                    bool isMatch = registerLoginService.TokenMatch(oldPassword, new NetworkCredential("", account.Password).SecurePassword);
                    if(isMatch)
                    {
                        account = registerLoginService.ChangePassword(account, newPassword);
                        canSave = databaseService.Save(account, false);
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.BadRequest, SharedMessages.ERROR_FIRST_ACCESS_PASSWORD);
                    }
                    
                    if(canSave)
                    {
                        bool isEmailSended = registerLoginService.ChangePasswordSendEmail(account);
                        if(isEmailSended)
                        {
                            return StatusCode((int)HttpStatusCode.OK);
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.OK, SharedMessages.ERROR_SENDING_EMAIL_CHANGE_PASSWORD);
                        }                        
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, SharedMessages.ERROR_SAVING_DATA);
                    }                    
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.NotFound);
                }

            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }
        }
    }
}
