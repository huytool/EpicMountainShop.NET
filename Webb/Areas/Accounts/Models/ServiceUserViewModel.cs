using Lap1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webb.Areas.Accounts.Models
{
    public class ServiceUserViewModel
    {
        public List<ApplicationUser> Customers { get; set; }
        public ServiceUsersRegistrationViewModel Registration { get; set; }
    }
}
