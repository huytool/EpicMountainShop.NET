using ASC.Models.BaseTypes;
using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Business.Interface
{
   public interface IServiceRequestOperations
    {
        Task CreateServiceRequestAsync(ServiceRequest request);
        Task<ServiceRequest> UpdateServiceRequestAsync(ServiceRequest request);
        Task<ServiceRequest> UpdateServiceRequestStatusAsync(string rowKey, string partitionKey, string status);
        Task<List<ServiceRequest>> GetServiceRequestsByRequestedDateAndStatus
            (DateTime? requestedDate,
              List<string> status = null,
              string email = "",
              string serviceEngineerEmail = "");
        Task<List<ServiceRequest>> GetServiceRequestsFormAudit(string serviceEngineerEmail = "");
        Task<List<ServiceRequest>> GetActiveServiceRequests(List<string> status);
        Task<ServiceRequest> GetServiceRequestByRowKey(string id);
        Task<List<ServiceRequest>> GetServiceRequestAuditByPartitionKey(string id);

    }

}
