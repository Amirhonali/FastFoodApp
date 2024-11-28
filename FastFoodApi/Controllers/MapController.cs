using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace FastFoodApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class MapController : ControllerBase
    {
        [HttpGet("[action]")]
        [Authorize(Roles = "CalculateDistance")]
        public async Task<IActionResult> CalculateDistance(string origin, string destination)
        {
            var apiKey = "YOUR_GOOGLE_MAPS_API_KEY";
            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origin}&destinations={destination}&key={apiKey}";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            return Ok(content);
        }
    }
}