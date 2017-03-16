using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCoreElmahUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("HomeController Index called");

            HttpClient _client = new HttpClient();
            var response = await _client.GetAsync("http://localhost:37209/api/values");
            response.EnsureSuccessStatusCode();
            var responseString = System.Text.Encoding.UTF8.GetString(
                await response.Content.ReadAsByteArrayAsync()
            );

            _logger.LogInformation("HomeController Index complete");
            return View();
        }

        public async Task<IActionResult> About()
        {
            _logger.LogInformation("HomeController About called");
            // throws exception
            HttpClient _client = new HttpClient();
            var response = await _client.GetAsync("http://localhost:37209/api/values/1");
            response.EnsureSuccessStatusCode();
            var responseString = System.Text.Encoding.UTF8.GetString(
                await response.Content.ReadAsByteArrayAsync()
            );
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
