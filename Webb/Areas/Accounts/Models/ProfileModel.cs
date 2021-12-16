using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webb.Areas.Accounts.Models
{
    public class ProfileModel
    {
        [Display(Name="Tài khoản")]
        public string UserName { get; set; }
    }
}
