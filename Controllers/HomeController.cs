using DualBuffer.Models;
using DualBuffer.Models.Enums;
using DualBuffer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DualBuffer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private INetworkService _networkService;

        public HomeController(ILogger<HomeController> logger, INetworkService networkService)
        {
            _logger = logger;
            _networkService = networkService;
        }

        public IActionResult Index()
        {
            List<Call> calls = _networkService.ListCalls();
            return View(calls);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}