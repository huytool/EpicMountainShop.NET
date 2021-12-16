using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webb.Areas.Configuration.Models
{
    public class MasterDataKeyViewModel
    {
        public string RowKey { get; set; }
        public string PartitionKey { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Nhập loại giày")]
        public string Name { get; set; }
    }
}
