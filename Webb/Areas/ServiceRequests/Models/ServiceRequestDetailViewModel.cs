using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webb.Areas.ServiceRequests.Models
{
    public class ServiceRequestDetailViewModel
    {
        public UpdateServiceRequestViewModel ServiceRequest { get; set; }
        public List<ServiceRequest> ServiceRequestAudit { get; set; }
    }
}
