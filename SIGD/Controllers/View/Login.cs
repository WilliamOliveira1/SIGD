using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Controllers.View
{
    public class Login : Controller
    {
        public IActionResult LoginPage()
        {
            return View();
        }
    }
}
