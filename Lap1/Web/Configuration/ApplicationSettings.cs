using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lap1.Web.Configuration
{
    public class ApplicationSettings
    {
        public string ApplicationTitle { get; set; }
        public string AdminEmail { get; set; }
        public string AdminName { get; set; }
        public string AdminPassword { get; set; }
        public string Roles { get; set; }
    }
}
