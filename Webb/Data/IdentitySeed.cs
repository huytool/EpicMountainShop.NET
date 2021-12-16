using ASC.Models.BaseTypes;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Lap1.Models;
using Lap1.Web.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Lap1.Data
{
    public class IdentitySeed : IIdentitySeed
    {


        public async Task Seed(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<ApplicationSettings> options)
        {
            var roles = options.Value.Roles.Split(new char[] { ',' });
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    ApplicationRole storageRole = new ApplicationRole
                    {
                        Name = role
                    };
                    IdentityResult roleResult = await roleManager.CreateAsync(storageRole);
                }
            }
            var admin = await userManager.FindByEmailAsync(options.Value.AdminEmail);
            if (admin == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = options.Value.AdminName,
                    Email = options.Value.AdminEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = false
                };
                IdentityResult result = await userManager.CreateAsync(user, options.Value.AdminPassword);
                await userManager.AddClaimAsync(user, new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", options.Value.AdminEmail));
                await userManager.AddClaimAsync(user, new Claim("IsActive", "True"));

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
                }
            }
            var engineer = await userManager.FindByEmailAsync(options.Value.EngineerEmail);
            if (engineer == null)
            {
                ApplicationUser use = new ApplicationUser
                {
                    UserName = options.Value.EngineerName,
                    Email = options.Value.EngineerEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = false
                };

                IdentityResult result = await userManager.CreateAsync(use, options.Value.EngineerPassword);
                await userManager.AddClaimAsync(use, new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",options.Value.EngineerEmail));
                await userManager.AddClaimAsync(use, new Claim("IsActive", "True"));

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(use, Roles.Engineer.ToString());
                }

            }
        }


    }
}
