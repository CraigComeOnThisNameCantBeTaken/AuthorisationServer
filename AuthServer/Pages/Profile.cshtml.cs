using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthServer.Pages
{
    [Authorize]
    //[Authorize("MFA")]
    public class ProfileModel : PageModel
    {
        public IActionResult OnGet()
        {
            //var loggedIn = User.Identity.IsAuthenticated;

            //if (!loggedIn)
            //    return RedirectToPage("Login");

            return Page();
        }
    }
}
