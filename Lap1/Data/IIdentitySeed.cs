
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Lap1.Models;
using Lap1.Web.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Lap1.Data
{
    public interface IIdentitySeed
    {
            Task Seed(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<ApplicationSettings> options);
     
    }
}
