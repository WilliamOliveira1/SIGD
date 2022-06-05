using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGD.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SIGD.Controllers.API
{
    [Route("api/usersmanaged")]
    [Produces("application/json")]
    [ApiController]
    public class UsersManaged : Controller
    {
        private IUsersManagedService usersManagedService;

        public UsersManaged(IUsersManagedService usersManagedService)
        {
            this.usersManagedService = usersManagedService ?? throw new ArgumentNullException(nameof(usersManagedService));
        }

        [Authorize]
        [HttpPost("listallprincipals")]
        public IActionResult GetAllPrincipalsList()
        {
            try
            {
                var listOfUsers = usersManagedService.GetAllPrincipalsAccounts();
                return StatusCode((int)HttpStatusCode.OK, listOfUsers);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("listmanaged")]
        public IActionResult GetPrincipalsManagedList()
        {
            try
            {
                var listOfUsersManaged = usersManagedService.GetAllPrincipalsAccountsByAdmin(User.Identity.Name);
                foreach(var user in listOfUsersManaged)
                {
                    user.Password = null;
                }

                return StatusCode((int)HttpStatusCode.OK, listOfUsersManaged);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
