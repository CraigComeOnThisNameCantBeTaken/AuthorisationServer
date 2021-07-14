using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using AuthServer.AuthModels;

namespace AuthServer.Pages
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsPrincipalFactory;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsPrincipalFactory,
            SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger)
        {
            _userManager = userManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public async Task OnGetAsync()
        {
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                #region Signing in a user
                //var user = await _userManager.FindByNameAsync(Input.UserName);

                //if(user != null && await _userManager.CheckPasswordAsync(user, Input.Password))
                //{
                //    #region manually setting claims principle
                //    //var identity = new ClaimsIdentity("cookies");
                //    //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                //    //identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                //    //await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));
                //    #endRegion

                //    #region manually setting claims principle SIMPLIFIED
                //    var principle = await _claimsPrincipalFactory.CreateAsync(user);
                //    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principle);
                //    #endregion n 

                //    return RedirectToPage("Profile");
                //}
                #endregion

                #region Signing in a user SIMPLIFIED
                var signInResult = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, false, false);

                if(signInResult.Succeeded)
                    return RedirectToPage("Profile");
                #endregion

                ModelState.AddModelError("", "Invalid name or password");
            }

            return Page();
        }
    }
}
