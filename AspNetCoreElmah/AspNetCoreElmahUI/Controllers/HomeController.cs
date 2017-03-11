using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCoreElmahUI.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            HttpClient _client = new HttpClient();
            var response = await _client.GetAsync("http://localhost:37209/api/values");
            response.EnsureSuccessStatusCode();
            var responseString = System.Text.Encoding.UTF8.GetString(
                await response.Content.ReadAsByteArrayAsync()
            );
            return View();
        }

        public async Task<IActionResult> About()
        {
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
