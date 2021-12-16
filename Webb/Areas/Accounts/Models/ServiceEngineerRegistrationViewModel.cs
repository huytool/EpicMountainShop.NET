using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Webb.Areas.Identity.Pages.Account;

namespace Webb.Areas.Accounts.Models
{
    public class ServiceEngineerRegistrationViewModel:RegisterModel.InputModel
    {
        [Display(Name = "Tài khoản")]
        public string UserName { get; set; }
        public bool IsEdit { get; set; }
        public bool IsActive { get; set; }
    }
}
