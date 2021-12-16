using ASC.Business.Interface;
using ASC.DataAccess.Interfaces;
using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Business
{
    public class ServiceRequestMessageOperations : IServiceRequestMessageOperations
    {
        private readonly IUnitOfWork _unitOfWork;
        public ServiceRequestMessageOperations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task CreateServiceRequestMessageAsync(ServiceRequestMessage message)
        {
            using (_unitOfWork)
            {
                await _unitOfWork.Responitory<ServiceRequestMessage>().AddAsync(message);
                _unitOfWork.CommitTransaction();
            }
        }
        public async Task<List<ServiceRequestMessage>> GetServiceRequestMessageAsync(string
        serviceRequestId)
        {
            var serviceRequestMessages = await _unitOfWork.Responitory<ServiceRequestMessage>().FindAllByPartitionKeyAsync(serviceRequestId);
            return serviceRequestMessages.ToList();
        }
    }
}
