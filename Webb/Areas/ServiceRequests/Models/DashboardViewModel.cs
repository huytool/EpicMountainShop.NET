using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webb.Areas.ServiceRequests.Models
{
    public class DashboardViewModel
    {
        public List<ServiceRequest> ServiceRequests { get; set; }
        public List<ServiceRequest> AuditServiceRequests { get; set; }
        public Dictionary<string, int> ActiveServiceRequests { get; set; }
    }
}
