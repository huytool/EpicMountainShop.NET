using ASC.Models.BaseTypes;
using ASC.Utilities;
using Lap1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webb.Areas.Accounts.Models;
using Webb.Service;

namespace Webb.Areas.Accounts.Controllers
{

    [Area("Accounts")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;

        }
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ServiceEngineers()
        {
            var serviceEngineers = await _userManager.GetUsersInRoleAsync(Roles.Engineer.ToString());
            // Hold all service engineers in session
            HttpContext.Session.SetSession("ServiceEngineers", serviceEngineers);
            return View(new ServiceEngineerViewModel
            {
                ServiceEngineers = serviceEngineers == null ? null : serviceEngineers.ToList(),
                Registration = new ServiceEngineerRegistrationViewModel() { IsEdit = false }
            });
        }
         [HttpGet]
        public async Task<IActionResult> ServiceUsers()
        {
            var serviceUsers = await _userManager.GetUsersInRoleAsync(Roles.Engineer.ToString());
            // Hold all service engineers in session
            HttpContext.Session.SetSession("ServiceUsers", serviceUsers);
            return View(new ServiceEngineerViewModel
            {
                ServiceEngineers = serviceUsers == null ? null : serviceUsers.ToList(),
                Registration = new ServiceEngineerRegistrationViewModel() { IsEdit = false }
            });
        }
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceEngineers(ServiceEngineerViewModel serviceEngineer)
        {
            serviceEngineer.ServiceEngineers = HttpContext.Session.GetSession<List<ApplicationUser>>
           ("ServiceEngineers");
            if (!ModelState.IsValid)
            {
                return View(serviceEngineer);
            }
            if (serviceEngineer.Registration.IsEdit)
            {
                // Update User
                var user = await _userManager.FindByEmailAsync(serviceEngineer.Registration.Email);
                user.UserName = serviceEngineer.Registration.UserName;
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    result.Errors.ToList().ForEach(p => ModelState.AddModelError("", p.Description));
                    return View(serviceEngineer);
                }
                // Update Password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                IdentityResult passwordResult = await _userManager.ResetPasswordAsync(user, token,
               serviceEngineer.Registration.Password);
                if (!passwordResult.Succeeded)
                {
                    passwordResult.Errors.ToList().ForEach(p => ModelState.AddModelError("",
                   p.Description));
                    return View(serviceEngineer);
                }
                // Update claims

                user = await _userManager.FindByEmailAsync(serviceEngineer.Registration.Email);
                var identity = await _userManager.GetClaimsAsync(user);
                var isActiveClaim = identity.SingleOrDefault(p => p.Type == "IsActive");
                var removeClaimResult = await _userManager.RemoveClaimAsync(user,
                new System.Security.Claims.Claim(isActiveClaim.Type, isActiveClaim.Value));
                var addClaimResult = await _userManager.AddClaimAsync(user,
                new System.Security.Claims.Claim(isActiveClaim.Type, serviceEngineer.Registration.IsActive.ToString()));
            }
            else
            {
                // Create User
                ApplicationUser user = new ApplicationUser
                {
                    UserName = serviceEngineer.Registration.UserName,
                    Email = serviceEngineer.Registration.Email,
                    EmailConfirmed = true
                };
                IdentityResult result = await _userManager.CreateAsync(user, serviceEngineer.
               Registration.Password);
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", serviceEngineer.
               Registration.Email));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("IsActive", serviceEngineer.Registration.IsActive.ToString()));
                if (!result.Succeeded)
                {
                    result.Errors.ToList().ForEach(p => ModelState.AddModelError("",
                   p.Description));
                    return View(serviceEngineer);
                }
                // Assign user to Engineer Role
                var roleResult = await _userManager.AddToRoleAsync(user, Roles.Engineer.
               ToString());
                if (!roleResult.Succeeded)
                {
                    roleResult.Errors.ToList().ForEach(p => ModelState.AddModelError("",
                   p.Description));
                    return View(serviceEngineer);
                }
            }
            if (serviceEngineer.Registration.IsActive)
            {
                await _emailSender.SendEmailAsync(serviceEngineer.Registration.Email,
                "Account Created/Modified",
                $"Email : {serviceEngineer.Registration.Email} /n Passowrd : {serviceEngineer.Registration.Password}");
            }
            else
            {
                await _emailSender.SendEmailAsync(serviceEngineer.Registration.Email,
                "Account Deactivated",
                $"Your account has been deactivated.");
            }
            return RedirectToAction("ServiceEngineers");

        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = HttpContext.User.GetCurrentUserDetails();

            return View(new ProfileModel() { UserName = user.Name });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileModel profile)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            // Update UserName
            var user = await _userManager.FindByEmailAsync
           (HttpContext.User.GetCurrentUserDetails().Email);
            user.UserName = profile.UserName;
            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(p => ModelState.AddModelError
               ("", p.Description));
                return View();
            }
            await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction("Dashboard", "Dashboard", new { Area = "ServiceRequests" });
        }
        [HttpGet]
        public async Task<IActionResult> Customers()
        {
            var customers = await _userManager.GetUsersInRoleAsync(Roles.User.ToString());
            // Hold all service engineers in session
            HttpContext.Session.SetSession("Customers", customers);
            return View(new ServiceUserViewModel
            {
                Customers = customers == null ? null : customers.ToList(),
                Registration = new ServiceUsersRegistrationViewModel()
                {
                    IsEdit = false
                }
            }) ;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Customers(ServiceUserViewModel customer)
        {
            customer.Customers = HttpContext.Session.GetSession<List<ApplicationUser>>("Customers");
            if (!ModelState.IsValid)
            {
                return View(customer);
            }
            if (customer.Registration.IsEdit)
            {
                // Update User
                // Update claims IsActive
                var user = await _userManager.FindByEmailAsync(customer.Registration.Email);
                var identity = await _userManager.GetClaimsAsync(user);
                var isActiveClaim = identity.SingleOrDefault(p => p.Type == "IsActive");
                var removeClaimResult = await _userManager.RemoveClaimAsync
               (user, new System.Security.Claims.Claim(isActiveClaim.Type,isActiveClaim.Value));
               var addClaimResult = await _userManager.AddClaimAsync(user,
               new System.Security.Claims.Claim(isActiveClaim.Type,customer.Registration.IsActive.ToString()));
            } 

            if (customer.Registration.IsActive)
            {
                await _emailSender.SendEmailAsync(customer.Registration.Email,
               "Account Modified", $"Your account has been activated, Email: { customer.Registration.Email}");
            }
            else
            {
                await _emailSender.SendEmailAsync(customer.Registration.Email,"Account Deactivated", $"Your account has been deactivated.");
 }
            return RedirectToAction("Customers");
        }


    }

}
