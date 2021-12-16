using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webb.Areas.Configuration.Models
{ 
    public class MasterDataValueViewModel
    {
        [Display(Name = "ID")]
        public string RowKey { get; set; }
        [Required(ErrorMessage = "Chọn phân loại")]
        [Display(Name = "Phân loại")]
        public string PartitionKey { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Nhập tên")]
        [Display(Name = "Tên")]
        public string Name { get; set; }
    }
}
