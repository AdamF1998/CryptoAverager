using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;

namespace CryptoAverager.FrontEnd.Controllers
{
    [Route("[controller]/[action]")]
    public class CultureController : Controller
    {
        /// <summary>
        /// The app persists the user's selected culture via a redirect to a controller.
        /// The controller sets the user's selected culture into a cookie and redirects the user back to the original URI.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public IActionResult SetCulture(string culture, string redirectUri)
        {
            if (culture != null)
            {
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)), new Microsoft.AspNetCore.Http.CookieOptions {IsEssential = true });
            }

            // Redirects the user back to the same place safely
            return LocalRedirect(redirectUri);
        }
    }
}
