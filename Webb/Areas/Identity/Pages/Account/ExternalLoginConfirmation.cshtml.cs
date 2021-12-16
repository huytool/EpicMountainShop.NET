using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ASC.Models.BaseTypes;
using Lap1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;


namespace Webb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginConfirmationModel : PageModel
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        public ExternalLoginConfirmationModel(UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger,
            SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
           
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Nhập email")]
            [EmailAddress]
            public string Email { get; set; }
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user);
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", user.Email));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("IsActive", "True"));
                if (!result.Succeeded)
                {
                    result.Errors.ToList().ForEach(p => ModelState.AddModelError("", p.Description));
                    return RedirectToPage("./ExternalLoginConfirmation", Input);
                }
                var roleResult = await _userManager.AddToRoleAsync(user, Roles.User.ToString());
                if (!roleResult.Succeeded)
                {
                    roleResult.Errors.ToList().ForEach(p => ModelState.AddModelError("", p.Description));
                    return RedirectToPage("./ExternalLoginConfirmation", Input);
                }
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6, "User created an account using {Name} provider.",info.LoginProvider);
                        return RedirectToAction("Dashboard", "Dashboard", new { Area = "ServiceRequests" });
                    }
                }
                ModelState.AddModelError(string.Empty, result.ToString());
               
            }
            ViewData["ReturnUrl"] = returnUrl;
            return Page();

        }
        
    }
}
