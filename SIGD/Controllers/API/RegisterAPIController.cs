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
using System;
using System.Net;
using System.Security;
using System.Threading.Tasks;

namespace SIGD.Controllers.API
{
    [Route("api/register")]
    [Produces("application/json")]
    [ApiController]
    public class RegisterAPIController : Controller
    {        
        private IRegisterLoginService registerLoginService;
        private readonly IConfiguration _config;
        private string generatedToken = null;
        private CookieOptions httpOnlyAndSecureFlag = new CookieOptions { HttpOnly = true, Secure = true };
        private IMemoryCache lockAccount;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private const int MAX_ATTEMPT = 3;

        public RegisterAPIController(
            IRegisterLoginService registerLoginService,
            IConfiguration _config,
            IMemoryCache lockAccount,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {                        
            this.registerLoginService = registerLoginService ?? throw new ArgumentNullException(nameof(registerLoginService));
            this._config = _config ?? throw new ArgumentNullException(nameof(_config));
            this.lockAccount = lockAccount ?? throw new ArgumentNullException(nameof(lockAccount));
            this._signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [AllowAnonymous]
        [HttpPost("registernewadmin")]
        public IActionResult RegisterNewAdmin([FromBody] ActivationAccount activationAccount)
        {
            ActivationAccount account = new ActivationAccount();

            if(string.IsNullOrEmpty(activationAccount.Email) || string.IsNullOrEmpty(activationAccount.UserName))
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            if (activationAccount != null)
            {
                ActivationAccount accountByEmail = registerLoginService.GetUserByEmail(activationAccount.Email);
                ActivationAccount accountByUser = registerLoginService.GetUserByUsername(activationAccount.UserName);

                if (accountByEmail != null || accountByUser != null)
                {
                    if (accountByEmail?.Email == activationAccount.Email)
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, SharedMessages.ERROR_EMAIL_TAKEN);
                    }
                    else if (accountByUser?.UserName == activationAccount.UserName)
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, SharedMessages.ERROR_USERNAME_TAKEN);
                    }                    
                }

                account = registerLoginService.CreateNewUser(activationAccount, Role.Administrator);
                if(account == null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, SharedMessages.ERROR_SENDING_EMAIL);
                }

                bool canSave = registerLoginService.SaveAccount(account, true);

                if(canSave)
                {
                    return StatusCode((int)HttpStatusCode.OK);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Forbidden);
                }
            }
            else
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] JObject input)
        {
            string username = input?["username"]?.ToString();
            string email = input?["email"]?.ToString();
            int userLoginAttempt = 0;            

            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(email))
            {
                ActivationAccount account = new ActivationAccount();

                if (!string.IsNullOrEmpty(email))
                {
                    account = registerLoginService.GetUserByEmail(email);
                }
                else if (!string.IsNullOrEmpty(username))
                {
                    account = registerLoginService.GetUserByUsername(username);
                }

                if (account != null)
                {
                    // time that will stay locked thee account
                    var lockAccountTime = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

                    SecureString oldPassword = new NetworkCredential("", input["password"]?.ToString()).SecurePassword;
                    bool isMatch = registerLoginService.TokenMatch(oldPassword, new NetworkCredential("", account.Password).SecurePassword);

                    if(lockAccount.TryGetValue(account.UserName, out userLoginAttempt))
                    {
                        if (userLoginAttempt == MAX_ATTEMPT || userLoginAttempt >= MAX_ATTEMPT)
                        {
                            return StatusCode((int)HttpStatusCode.BadRequest);
                        }
                    }

                    // TODO lock if get wrong more than 3 times
                    if (account.IsActivated && isMatch)
                    {
                        var validUser = registerLoginService.GetUser(account);
                        if (validUser != null)
                        {
                            generatedToken = registerLoginService.GetJWTToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), validUser);
                            if (generatedToken != null)
                            {                                                            
                                await SetAdminUser(account);
                                return StatusCode((int)HttpStatusCode.OK);
                            }
                            else
                            {
                                return StatusCode((int)HttpStatusCode.InternalServerError);
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

            if (string.IsNullOrEmpty(input?["email"]?.ToString()) || string.IsNullOrEmpty(input?["oldpassword"]?.ToString())
                || string.IsNullOrEmpty(input?["password"]?.ToString()) || string.IsNullOrEmpty(input?["username"]?.ToString()))
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            if (!string.IsNullOrEmpty(input?["email"]?.ToString()) || !string.IsNullOrEmpty(input?["oldpassword"]?.ToString())
                || !string.IsNullOrEmpty(input?["password"]?.ToString()) || !string.IsNullOrEmpty(input?["username"]?.ToString()))
            {
                ActivationAccount account = new ActivationAccount();
                string username = input?["username"]?.ToString();
                string email = input?["email"]?.ToString();
                SecureString oldPassword = new NetworkCredential("", input["oldpassword"]?.ToString()).SecurePassword;
                SecureString newPassword = new NetworkCredential("", input["password"]?.ToString()).SecurePassword;
                if (!string.IsNullOrEmpty(email))
                {
                    account = registerLoginService.GetUserByEmail(email);
                }
                else if (!string.IsNullOrEmpty(username))
                {
                    account = registerLoginService.GetUserByUsername(username);
                }
                
                if (account != null)
                {                    
                    bool isMatch = registerLoginService.TokenMatch(oldPassword, new NetworkCredential("", account.Password).SecurePassword);
                    if(isMatch)
                    {
                        account = registerLoginService.ChangePassword(account, newPassword);
                        canSave = registerLoginService.SaveAccount(account, false);
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

        [HttpPost("registernewaprincipal")]
        public IActionResult RegisterNewPrincipal([FromBody] ActivationAccount activationAccount)
        {
            if (activationAccount != null)
            {
                ActivationAccount accountByEmail = registerLoginService.GetUserByEmail(activationAccount.Email);
                ActivationAccount accountByUser = registerLoginService.GetUserByUsername(activationAccount.UserName);

                if (accountByEmail != null || accountByUser != null)
                {
                    if (accountByEmail?.Email == activationAccount.Email)
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, SharedMessages.ERROR_EMAIL_TAKEN);
                    }
                    else if (accountByUser?.UserName == activationAccount.UserName)
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, SharedMessages.ERROR_USERNAME_TAKEN);
                    }
                }

                string supervisorUsername = User.Identity.Name;
                var account = registerLoginService.CreateNewUser(activationAccount, Role.Principal, supervisorUsername);
                if (account == null)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, SharedMessages.ERROR_SENDING_EMAIL);
                }

                if (registerLoginService.SaveAccount(account, true))
                {
                    return StatusCode((int)HttpStatusCode.OK);
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


        private async Task<string> SetAdminUser(ActivationAccount account)
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
    }
}
