using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webb.Areas.ServiceRequests.Models
{
    public class NewServiceRequestViewModel
    {
        [Required(ErrorMessage ="Chọn loại giày")]
        [Display(Name = "Giày")]
        public string VehicleName { get; set; }
        [Required(ErrorMessage = "Chọn màu")]
        [Display(Name = "Màu")]
        public string VehicleType { get; set; }
        [Required(ErrorMessage = "Chọn hình thức thanh toán")]
        [Display(Name = "Thanh toán")]
        public string RequestedServices { get; set; }
        [Display(Name = "Ngày đặt hàng")]
        public DateTime? RequestedDate { get; set; }
    }
}
