using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SIGD.Data;
using SIGD.Helper;
using SIGD.Models;
using SIGD.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Controllers.API
{
    [Route("api/register")]
    [Produces("application/json")]
    [ApiController]
    public class RegisterAPIController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RegisterAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("registernew")]
        public IActionResult RegisterNew([FromBody] ActivationAccount activationAccount)
        {
            RegisterLoginService registerLoginService = new RegisterLoginService();

            if (activationAccount != null)
            {
                var account = registerLoginService.CreateNewAdminUser(activationAccount);
                _context.Add(account);
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] string email, string oldPassword)
        {
            RegisterLoginService registerLoginService = new RegisterLoginService();

            if (!string.IsNullOrEmpty(email))
            {
                ActivationAccount account = _context.ActivationAccount.Where(x => x.Email == email).FirstOrDefault();
                bool isMatch = registerLoginService.TokenMatch(oldPassword, account.password);

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
                return BadRequest();
            }
        }
    }
}
