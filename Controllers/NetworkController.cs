using DualBuffer.Models;
using DualBuffer.Models.Enums;
using DualBuffer.Repositories;
using DualBuffer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DualBuffer.Controllers
{
    public class NetworkController : Controller
    {

        private readonly INetworkService _networkService;


        public NetworkController(INetworkService networkService)
        {
            _networkService = networkService;
        }

        // GET: NetworkController
        public  ActionResult Index()
        {
            List<Call> calls =  _networkService.ListCalls();
            return View(calls);
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
            Call incomingCall;
 
            var isRequestAccepted = _networkService.AcceptRequest(model.Di, model.SNR,model.WRB,model.N, model.Nco);
            if (isRequestAccepted)
            {
                incomingCall = new Call(model.callType);


                incomingCall.NumResourceBlocks = model.Nco;
                incomingCall.signalToNoiseRatio = model.SNR;
                incomingCall.callDuration = model.Di;
                incomingCall.Type = model.callType;
                incomingCall.requiredBandwidth = model.WRB;
                incomingCall.Status = CallStatus.Completed;
                incomingCall.totalChannels = model.N;
                incomingCall.allocatedChannels = model.Nco;
                incomingCall.WaitingTime = 0;

            }
            else
            {
                 incomingCall = new Call(model.callType);

                TimeSpan waitingTime = TimeSpan.FromSeconds(30);
                var accepted = _networkService.AcceptRequestIntoBuffer(incomingCall, waitingTime);

                List<Call> rtCalls = _networkService.GetRTCalls();
                List<Call> nrtCalls = _networkService.GetNRTCalls();

                int numChannels = 7; // Number of available channels

                _networkService.AllocateResourcesToCalls(rtCalls, nrtCalls, numChannels);

                incomingCall.NumResourceBlocks = model.Nco;
                incomingCall.signalToNoiseRatio = model.SNR;
                incomingCall.callDuration = model.Di;
                incomingCall.Type = model.callType;
                incomingCall.requiredBandwidth = model.WRB;
                incomingCall.Status = (accepted == true)? CallStatus.Completed:CallStatus.Failed;
                incomingCall.totalChannels = model.N;
                incomingCall.allocatedChannels = model.Nco;
            }

            var response = _networkService.AddCall(incomingCall);
            if (response != null)
            { 
                TempData["Successful"] = "Created Successful";

                return RedirectToAction("Index","Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CalculateMetrics()
        {
            var fairnessIndex = _networkService.CalculateFairnessIndex();
            var throughPut = _networkService.CalculateThroughput();
            var avgWaitingTime = _networkService.CalculateAverageWaitingTime();
            var systemUtil = _networkService.CalculateSystemUtilization();
            var packetLossRate =  _networkService.CalculatePacketLossRate();
            var rtProb = _networkService.CalculateRTBlockingProbability();
            var nrtProb = _networkService.CalculateNRTBlockingProbability();

            ViewBag.FairnessIndex = fairnessIndex;
            ViewBag.Throughput = throughPut;
            ViewBag.WaitingTime = avgWaitingTime;
            ViewBag.SystemUtilization = systemUtil;
            ViewBag.PacketLossRate = packetLossRate;
            ViewBag.RTProb = rtProb;
            ViewBag.NRTProb = nrtProb;

            return View("~/Views/Home/Index.cshtml", _networkService.ListCalls());
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
            _networkService.DeleteCall(id);
            return View("~/Views/Home/Index.cshtml", _networkService.ListCalls());
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
