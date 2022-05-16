using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SIGD.Data;
using SIGD.Helper;
using SIGD.Models;
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

        [HttpPost("newregister")]
        public IActionResult GetClientList([FromBody] ActivationAccount activationAccount)
        {
            EmailSender emailSender = new EmailSender();

            if (activationAccount != null)
            {
                emailSender.SendEmail(activationAccount.Email, $"Bem vindo ao SIGD {activationAccount.UserName},Sua senha de primeiro acesso: ");
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
