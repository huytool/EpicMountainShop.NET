using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Business.Interface
{
    public interface IServiceRequestMessageOperations
    {
        Task CreateServiceRequestMessageAsync(ServiceRequestMessage message);
        Task<List<ServiceRequestMessage>> GetServiceRequestMessageAsync(string
        serviceRequestId);
    }
}
