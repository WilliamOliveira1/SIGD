using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SIGD.Helper;
using SIGD.Interfaces;
using SIGD.Models;
using SIGD.Services;
using System;
using System.Net;
using System.Security;

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

        public RegisterAPIController(IActivationAccountRepository databaseService, ITokenService tokenService, IRegisterLoginService registerLoginService)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            this.registerLoginService = registerLoginService ?? throw new ArgumentNullException(nameof(registerLoginService));
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

        [HttpPost("login")]
        public IActionResult Login([FromBody] JObject input)//string email, string oldPassword
        {
            string username = input?["username"]?.ToString();
            string email = input?["email"]?.ToString();

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
                    SecureString oldPassword = new NetworkCredential("", input["password"]?.ToString()).SecurePassword;
                    bool isMatch = registerLoginService.TokenMatch(oldPassword, new NetworkCredential("", account.password).SecurePassword);

                    // TODO lock if get wrong more than 3 times
                    if (!isMatch)
                    {
                        return StatusCode((int)HttpStatusCode.BadRequest, SharedMessages.ERROR_PASSWORD_NOT_MATCH);
                    }
                    else if (!account.IsActivated && isMatch)
                    {
                        return StatusCode((int)HttpStatusCode.OK, SharedMessages.CHANGE_FIRST_ACCESS_PASSWORD);
                    }
                    // TODO Authenticate the user
                    else if (account.IsActivated && isMatch)
                    {
                        return StatusCode((int)HttpStatusCode.OK);
                    }

                    return StatusCode((int)HttpStatusCode.OK);
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
                    bool isMatch = registerLoginService.TokenMatch(oldPassword, new NetworkCredential("", account.password).SecurePassword);
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
