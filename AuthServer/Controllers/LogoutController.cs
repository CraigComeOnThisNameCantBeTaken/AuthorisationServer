using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthServer.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("cookies");

            return Redirect("/");
        }
    }
}
