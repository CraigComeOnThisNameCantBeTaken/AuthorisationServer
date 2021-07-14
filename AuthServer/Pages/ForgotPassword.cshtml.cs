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
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public InputModel Input { get; set; }

        public string ResetLink { get; set; }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }
        }

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // note: probably should be email
                var user = await _userManager.FindByNameAsync(Input.UserName);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var protocol = Request.Scheme;
                    var resetUrl = Url.Page(
                        "ResetPassword",
                        "OnGetAsync",
                        new
                        {
                            token = token,
                            userName = Input.UserName
                        },
                        protocol
                    );

                    // normally this would be emailed to the user
                    ResetLink = resetUrl;
                }
                else
                {
                    // could email the user here to tell them they dont have an account for their username / email
                    // so they could try another email. Doing nothing looks broken. Showing on screen tells malicious users if
                    // its a hit or not
                }
            }

            return Page();
        }
    }
}
