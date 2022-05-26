using Microsoft.AspNetCore.Mvc;
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
                    return Forbid();
                }

                bool canSave = databaseService.Save(account);

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
        public IActionResult Login([FromBody] string email, string oldPassword)
        {          
            if (!string.IsNullOrEmpty(email))
            {
                ActivationAccount account = databaseService.GetActivationAccountByEmail(email);
                if(account != null)
                {
                    bool isMatch = registerLoginService.TokenMatch(new NetworkCredential("", oldPassword).SecurePassword, 
                        new NetworkCredential("", account.password).SecurePassword);

                    // TODO Message in front-end must be "Password and/or email is wrong try again"
                    // TODO lock if get wrong more than 3 times
                    if (!isMatch)
                    {
                        return BadRequest();
                    }
                    // TODO Create new password page
                    else if (!account.IsActivated && isMatch)
                    {
                        return Ok();
                    }
                    // TODO Authenticate the user
                    else if (account.IsActivated && isMatch)
                    {
                        return Ok();
                    }

                    return Ok();
                }
                else
                {
                    return NotFound();
                }
                
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpPost("changepassword")]
        public IActionResult ChangePassword([FromBody] string email, SecureString newPassword, SecureString oldPassword)
        {
            if (!string.IsNullOrEmpty(email))
            {
                ActivationAccount account = databaseService.GetActivationAccountByEmail(email);
                if (account != null)
                {
                    bool isMatch = registerLoginService.TokenMatch(oldPassword, new NetworkCredential("", account.password).SecurePassword);                    

                    return Ok();
                }
                else
                {
                    return NotFound();
                }

            }
            else
            {
                return BadRequest();
            }
        }
    }
}
