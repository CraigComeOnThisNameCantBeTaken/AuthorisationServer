using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.AuthModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthServer.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Compare("Password")]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; }
        }

        [BindProperty]
        public string UserName { get; set; }
        [BindProperty]
        public string Token { get; set; }

        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string token, string userName)
        {
            Token = token;
            UserName = userName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(UserName);

                if(user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, Token, Input.Password);

                    if(!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        return Page();
                    }

                    return RedirectToPage("Login");
                }

                // this shouldnt be possible. To get here you would need a valid token, but invalid username
                ModelState.AddModelError("", "Invalid request");
            }

            return Page();
        }
    }
}
