using ASC.Business.Interface;
using ASC.Models.BaseTypes;
using ASC.Models.Models;
using ASC.Utilities;
using AutoMapper;
using Lap1.Controllers;
using Lap1.Models;
using Lap1.Web.Configuration;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ServiceHub;
using Webb.Areas.ServiceRequests.Models;
using Webb.Data;

namespace Webb.Areas.ServiceRequests.Controllers
{

    [Area("ServiceRequests")]
    public class ServiceRequestController : BaseController
    {
        private readonly IServiceRequestOperations _serviceRequestOperations;
        private readonly IMapper _mapper;
        private readonly IMasterDataCacheOperations _masterData;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceRequestMessageOperations _serviceRequestMessageOperations;
        private readonly IOptions<ApplicationSettings> _options;
        private readonly IHubContext<ServiceMessagesHub> _hubContext;
        public ServiceRequestController(IServiceRequestOperations operations, IServiceRequestMessageOperations messageOperations,
            IHubContext<ServiceMessagesHub> hubContext, IMapper mapper, IMasterDataCacheOperations masterData,
            UserManager<ApplicationUser> userManager, IOptions<ApplicationSettings> options)
        {
            _serviceRequestOperations = operations;
            _serviceRequestMessageOperations = messageOperations;
            _hubContext = hubContext;
            _mapper = mapper;
            _masterData = masterData;
            _userManager = userManager;
            _options = options;
        }
        [HttpGet]
        public async Task<IActionResult> ServiceRequest()
        {
            var masterData = await _masterData.GetMasterDataCacheAsync();
            ViewBag.VehicleTypes = masterData.Values.Where(p => p.PartitionKey == MasterKeys.Màu.ToString()).ToList();
            ViewBag.VehicleNames = masterData.Values.Where(p => p.PartitionKey == MasterKeys.Giày.ToString()).ToList();
            return View(new NewServiceRequestViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> ServiceRequest(NewServiceRequestViewModel request)
        {
            if (!ModelState.IsValid)
            {
                var masterData = await _masterData.GetMasterDataCacheAsync();
                ViewBag.VehicleTypes = masterData.Values.Where(p => p.PartitionKey == MasterKeys.Màu.ToString()).ToList();
                ViewBag.VehicleNames = masterData.Values.Where(p => p.PartitionKey == MasterKeys.Giày.ToString()).ToList();
                return View(request);
            }
            // Map the view model to Azure model
            var serviceRequest = _mapper.Map<NewServiceRequestViewModel, ServiceRequest>(request);
            // Set RowKey, PartitionKey, RequestedDate, Status properties
            serviceRequest.PartitionKey = HttpContext.User.GetCurrentUserDetails().Email;
            serviceRequest.RowKey = Guid.NewGuid().ToString();
            serviceRequest.RequestedDate = DateTime.Now;
            serviceRequest.Status = Status.New.ToString();
            await _serviceRequestOperations.CreateServiceRequestAsync(serviceRequest);
            return RedirectToAction("Dashboard", "Dashboard", new { Area = "ServiceRequests" });
        }
        [HttpGet]
        public async Task<IActionResult> ServiceRequestDetails(string id)
        {
            var serviceRequestDetails = await _serviceRequestOperations.GetServiceRequestByRowKey(id);
            // Access Check
            if (HttpContext.User.IsInRole(Roles.Engineer.ToString())
            && serviceRequestDetails.ServiceEngineer != HttpContext.User.
           GetCurrentUserDetails().Email)
            {
                throw new UnauthorizedAccessException();
            }
            if (HttpContext.User.IsInRole(Roles.User.ToString())
            && serviceRequestDetails.PartitionKey != HttpContext.User.
           GetCurrentUserDetails().Email)
            {
                throw new UnauthorizedAccessException();
            }
            var serviceRequestAuditDetails = await _serviceRequestOperations.
           GetServiceRequestAuditByPartitionKey(
            serviceRequestDetails.PartitionKey + "-" + id);
            // Select List Data
            var masterData = await _masterData.GetMasterDataCacheAsync();
            ViewBag.VehicleTypes = masterData.Values.Where(p => p.PartitionKey == MasterKeys.Màu.ToString()).ToList();
            ViewBag.VehicleNames = masterData.Values.Where(p => p.PartitionKey == MasterKeys.Giày.ToString()).ToList();
            ViewBag.Status = Enum.GetValues(typeof(Status)).Cast<Status>().Select(v => v.ToString()).ToList();
            ViewBag.ServiceEngineers = await _userManager.GetUsersInRoleAsync(Roles.Engineer.ToString());
            return View(new ServiceRequestDetailViewModel
            {
                ServiceRequest = _mapper.Map<ServiceRequest, UpdateServiceRequestViewModel>(serviceRequestDetails),
                ServiceRequestAudit = serviceRequestAuditDetails.OrderByDescending(p => p.Timestamp).ToList()
            });

        }
        [HttpPost]
        public async Task<IActionResult> UpdateServiceRequestDetails(UpdateServiceRequestViewModel serviceRequest)
        {
            var originalServiceRequest = await _serviceRequestOperations.GetServiceRequestByRowKey(serviceRequest.RowKey);
            originalServiceRequest.RequestedServices = serviceRequest.RequestedServices;
            // Update Status only if user role is either Admin or Engineer
            // Or Customer can update the status if it is only in Pending Customer Approval.
            if (HttpContext.User.IsInRole(Roles.Admin.ToString()) ||
            HttpContext.User.IsInRole(Roles.Engineer.ToString()) ||
            (HttpContext.User.IsInRole(Roles.User.ToString()) && originalServiceRequest.Status == Status.PendingCustomerApproval.ToString()))
            {
                originalServiceRequest.Status = serviceRequest.Status;
            }

            // Update Service Engineer field only if user role is Admin
            if (HttpContext.User.IsInRole(Roles.Admin.ToString()))
            {
                originalServiceRequest.ServiceEngineer = serviceRequest.ServiceEngineer;
            }
            await _serviceRequestOperations.UpdateServiceRequestAsync(originalServiceRequest);
            return RedirectToAction("ServiceRequestDetails", "ServiceRequest",
            new { Area = "ServiceRequests", Id = serviceRequest.RowKey });
        }
        [HttpGet]
        public async Task<IActionResult> ServiceRequestMessages(string serviceRequestId)
        {
            return Json((await _serviceRequestMessageOperations.GetServiceRequestMessageAsync(serviceRequestId)).OrderBy(p => p.MessageDate));
        }
        [HttpPost]
        public async Task<IActionResult> CreateServiceRequestMessage(ServiceRequestMessage message)
        {
            // Message and Service Request Id (Service request Id is the partition key for a message)
            if (string.IsNullOrWhiteSpace(message.Message) || string.IsNullOrWhiteSpace(message.PartitionKey))
                return Json(false);
            // Get Service Request details
            var serviceRequesrDetails = await _serviceRequestOperations.GetServiceRequestByRowKey(message.PartitionKey);
            // Populate message details
            message.FromEmail = HttpContext.User.GetCurrentUserDetails().Email;
            message.FromDisplayName = HttpContext.User.GetCurrentUserDetails().Name;
            message.MessageDate = DateTime.UtcNow;
            message.RowKey = Guid.NewGuid().ToString();
            // Get Customer and Service Engineer names
            var customerName = (await _userManager.FindByEmailAsync(serviceRequesrDetails.
            PartitionKey)).UserName;
            var serviceEngineerName = string.Empty;
            if (!string.IsNullOrWhiteSpace(serviceRequesrDetails.ServiceEngineer))
            {
                serviceEngineerName = (await _userManager.FindByEmailAsync(serviceRequesrDetails.
                ServiceEngineer)).UserName;
            }
            var adminName = (await _userManager.FindByEmailAsync(_options.Value.AdminEmail)).
            UserName;
            // Save the message to Azure Storage
            await _serviceRequestMessageOperations.CreateServiceRequestMessageAsync(message);
            var users = new List<string> { customerName, adminName };
            if (!string.IsNullOrWhiteSpace(serviceEngineerName))
            {
                users.Add(serviceEngineerName);
            }
            // Broadcast the message to all clients asscoaited with Service Request
            await _hubContext.Clients.Users(users).SendAsync("SendMessage", message);
          
            // Return true
            return Json(true);
        }
    }

}
