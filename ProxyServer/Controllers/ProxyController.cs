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
        private readonly ILogger<ProxyController> _logger;

        public ProxyController(
            LoadBalancerService lb,
            RedisCacheService cache,
            ILogger<ProxyController> logger)
        {
            _lb = lb;
            _cache = cache;
            _logger = logger;
            _http = new HttpClient();
        }

        // ---------------------------------------
        // GET employee/{id}
        // ---------------------------------------
        [HttpGet("employee/{id}")]
        public async Task<IActionResult> GetEmployee(Guid id)
        {
            string cacheKey = $"emp_{id}";

            var cached = await _cache.GetAsync<Employee>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("CACHE HIT → {Key}", cacheKey);
                return Ok(cached);
            }

            _logger.LogInformation("CACHE MISS → {Key}", cacheKey);

            var server = _lb.GetNextServer();
            _logger.LogInformation("LB → Request routed to {Server}", server);

            var response = await _http.GetAsync($"{server}/employee/{id}");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var emp = await response.Content.ReadFromJsonAsync<Employee>();

            await _cache.SetAsync(cacheKey, emp);
            _logger.LogInformation("CACHE SET → {Key}", cacheKey);

            return Ok(emp);
        }

        // ---------------------------------------
        // GET employees
        // ---------------------------------------
        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            string cacheKey = $"emp_list_{offset}_{limit}";

            var cached = await _cache.GetAsync<List<Employee>>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("CACHE HIT → {Key}", cacheKey);
                return Ok(cached);
            }

            _logger.LogInformation("CACHE MISS → {Key}", cacheKey);

            var server = _lb.GetNextServer();
            _logger.LogInformation("LB → Request routed to {Server}", server);

            // DWServer DOES NOT support offset/limit → full list
            var response = await _http.GetAsync($"{server}/employee");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var allEmployees = await response.Content.ReadFromJsonAsync<List<Employee>>();

            var sliced = allEmployees
                .Skip(offset)
                .Take(limit)
                .ToList();

            await _cache.SetAsync(cacheKey, sliced);
            _logger.LogInformation("CACHE SET → {Key}", cacheKey);

            return Ok(sliced);
        }
    }
}
