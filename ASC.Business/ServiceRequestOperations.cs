using ASC.Business.Interface;
using ASC.DataAccess.Interfaces;
using ASC.Models.BaseTypes;
using ASC.Models.Models;
using ASC.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Business
{
   public class ServiceRequestOperations : IServiceRequestOperations
    {
        private readonly IUnitOfWork _unitOfWork;
        public ServiceRequestOperations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task CreateServiceRequestAsync(ServiceRequest request)
        {
            using (_unitOfWork)
            {
                await _unitOfWork.Responitory<ServiceRequest>().AddAsync(request);
                _unitOfWork.CommitTransaction();
            }
        }
        public async Task<ServiceRequest> UpdateServiceRequestAsync(ServiceRequest request)
        {
            using (_unitOfWork)
            {
                await _unitOfWork.Responitory<ServiceRequest>().UpdateAsync(request);
                _unitOfWork.CommitTransaction();
                return request;
            }
        }
        public async Task<ServiceRequest> UpdateServiceRequestStatusAsync(string rowKey,
       string partitionKey, string status)
        {
            using (_unitOfWork)
            {
                var serviceRequest = await _unitOfWork.Responitory<ServiceRequest>().FindAsync(partitionKey, rowKey);
                if (serviceRequest == null)
                    throw new NullReferenceException();
                serviceRequest.Status = status;
                await _unitOfWork.Responitory<ServiceRequest>().UpdateAsync(serviceRequest);
                _unitOfWork.CommitTransaction();
                return serviceRequest;
            }
        }
        public async Task<List<ServiceRequest>> GetServiceRequestsByRequestedDateAndStatus
                (DateTime? requestedDate,
                 List<string> status = null,
                 string email = "",
                 string serviceEngineerEmail = "")
                        {
            var query = Queries.GetDashboardQuery(requestedDate, status, email,
           serviceEngineerEmail);
            var serviceRequests = await _unitOfWork.Responitory<ServiceRequest>().
           FindAllByQuery(query);
            return serviceRequests.ToList();
        }

        public async Task<List<ServiceRequest>> GetServiceRequestsFormAudit(string serviceEngineerEmail = "")
        {
            var query = Queries.GetDashboardAuditQuery(serviceEngineerEmail);
            var serviceRequests = await _unitOfWork.Responitory<ServiceRequest>().FindAllInAuditByQuery(query);
            return serviceRequests.ToList();
        }

        public async Task<List<ServiceRequest>> GetActiveServiceRequests(List<string> status)
        {
            var query = Queries.GetDashboardServiceEngineersQuery(status);
            var serviceRequests = await _unitOfWork.Responitory<ServiceRequest>().FindAllByQuery(query);
            return serviceRequests.ToList();
        }

        public async Task<ServiceRequest> GetServiceRequestByRowKey(string id)
        {
            var query = Queries.GetServiceRequestDetailsQuery(id);
            var serviceRequests = await _unitOfWork.Responitory<ServiceRequest>().FindAllByQuery(query);
            return serviceRequests.FirstOrDefault();
        }

        public async Task<List<ServiceRequest>> GetServiceRequestAuditByPartitionKey(string id)
        {
            var query = Queries.GetServiceRequestAuditDetailsQuery(id);
            var serviceRequests = await _unitOfWork.Responitory<ServiceRequest>().FindAllInAuditByQuery(query);
            return serviceRequests.ToList();
        }
    }
}
