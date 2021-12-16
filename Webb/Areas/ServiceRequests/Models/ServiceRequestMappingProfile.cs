using ASC.Models.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webb.Areas.ServiceRequests.Models
{
    public class ServiceRequestMappingProfile:Profile
    {
        public ServiceRequestMappingProfile()
        {
            CreateMap<ServiceRequest, UpdateServiceRequestViewModel>();
            CreateMap<NewServiceRequestViewModel, ServiceRequest>();
            CreateMap<UpdateServiceRequestViewModel, ServiceRequest>();
        }
    }
}
