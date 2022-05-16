using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Controllers
{
    public class RegisterController : Controller
    {
        // GET: RegisterController
        public ActionResult Register()
        {
            return View();
        }        
    }
}
