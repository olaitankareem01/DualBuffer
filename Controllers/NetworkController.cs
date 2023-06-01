using DualBuffer.Models;
using DualBuffer.Models.Enums;
using DualBuffer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DualBuffer.Controllers
{
    public class NetworkController : Controller
    {

        private readonly NetworkService _networkService = new NetworkService();

    
        
       /* public NetworkController(INetworkService aircraftService)
        {
            
        }*/

        // GET: NetworkController
        public ActionResult Index()
        {
            return View();
        }

        // GET: NetworkController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NetworkController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NetworkController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult MakeCall()
        {
            return View();
        }


        [HttpPost]
        public IActionResult MakeCall(MakeCallViewModel model)
        {
            var response = _networkService.AcceptRequest(model.Di, model.SNR,model.WRB,model.N, model.Nco);
            if (response)
            {
                TempData["Successful"] = "Created Successful";

                return RedirectToAction("List");
            }
            return View();
        }




        // GET: NetworkController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NetworkController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NetworkController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NetworkController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
