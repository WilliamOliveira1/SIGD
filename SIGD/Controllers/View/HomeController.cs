using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SIGD.Interfaces;
using SIGD.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SIGD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IRegisterLoginService registerLoginService;

        public HomeController(ILogger<HomeController> logger, IRegisterLoginService registerLoginService)
        {
            _logger = logger;
            this.registerLoginService = registerLoginService ?? throw new ArgumentNullException(nameof(registerLoginService));
        }

        public IActionResult Index()
        {
            return View(GetUser());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult MainBoard()
        {
            return View(GetUser());
        }

        [Authorize]
        public IActionResult FileManagementPage()
        {
            return View(GetUser());
        }

        [Authorize]
        public IActionResult FilesList()
        {
            return View(GetUser());
        }

        [Authorize]
        public IActionResult PrincipalFilesList()
        {
            return View(GetUser());
        }

        [Authorize]
        public IActionResult CreateUser()
        {
            return View(GetUser());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private ActivationAccount GetUser()
        {
            string username = User.Identity.Name;
            return registerLoginService.GetUserByUsername(username);
        }
    }
}
