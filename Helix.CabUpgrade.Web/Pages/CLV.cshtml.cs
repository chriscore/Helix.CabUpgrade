using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Helix.CabUpgrade.Web.Pages
{
    public class CLVModel : PageModel
    {
        private readonly ILogger<CLVModel> _logger;

        public CLVModel(ILogger<CLVModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}