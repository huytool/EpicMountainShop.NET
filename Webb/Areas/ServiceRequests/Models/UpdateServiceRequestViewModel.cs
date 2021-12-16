using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webb.Areas.ServiceRequests.Models
{
    public class UpdateServiceRequestViewModel:NewServiceRequestViewModel
    {
        public string RowKey { get; set; }
        public string PartitionKey { get; set; }
        [Required(ErrorMessage = "Chọn nhân viên giao hàng")]
        [Display(Name = "Nhân viên giao hàng")]
        public string ServiceEngineer { get; set; }
        [Required(ErrorMessage = "Chọn trạng thái")]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; }
    }
}
