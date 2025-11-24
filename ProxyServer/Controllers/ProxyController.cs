using Microsoft.AspNetCore.Mvc;
using ProxyServer.Services;
using Shared.Models;

namespace ProxyServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProxyController : ControllerBase
    {
        private readonly LoadBalancerService _lb;
        private readonly RedisCacheService _cache;
        private readonly HttpClient _http;

        public ProxyController(LoadBalancerService lb, RedisCacheService cache)
        {
            _lb = lb;
            _cache = cache;
            _http = new HttpClient();
        }

        [HttpGet("employee/{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            string cacheKey = $"emp_{id}";

            var cached = await _cache.GetAsync<Employee>(cacheKey);
            if (cached != null)
                return Ok(cached);

            var server = _lb.GetNextServer();
            var response = await _http.GetAsync($"{server}/employee/{id}");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var emp = await response.Content.ReadFromJsonAsync<Employee>();

            await _cache.SetAsync(cacheKey, emp);

            return Ok(emp);
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees([FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            string cacheKey = $"emp_list_{offset}_{limit}";

            // 1️⃣ Check Redis cache
            var cached = await _cache.GetAsync<List<Employee>>(cacheKey);
            if (cached != null)
                return Ok(cached);

            // 2️⃣ No cache → forward to DW using load balancer
            var server = _lb.GetNextServer();
            var response = await _http.GetAsync($"{server}/employee?offset={offset}&limit={limit}");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var list = await response.Content.ReadFromJsonAsync<List<Employee>>();

            // 3️⃣ Save to cache
            await _cache.SetAsync(cacheKey, list);

            return Ok(list);
        }


    }

}
