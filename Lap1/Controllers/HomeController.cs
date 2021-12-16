using ASC.Utilities;
using Lap1.Models;
using Lap1.Web.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Lap1.Controllers
{
    public class HomeController : AnonymousController
    {
        private readonly ILogger<HomeController> _logger;
        private IOptions<ApplicationSettings> _settings;
        public HomeController(IOptions<ApplicationSettings> settings)
        {
            _settings= settings;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetSession("Test", _settings.Value);

            var settings = HttpContext.Session.GetSession<ApplicationSettings>("Test");

            ViewBag.Title = _settings.Value.ApplicationTitle;
            return View();
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }
        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
       
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
       
    }
}
