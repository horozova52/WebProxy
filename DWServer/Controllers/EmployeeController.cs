using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using DWServer.Repositories;

namespace DWServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _repo;

        public EmployeeController(IEmployeeRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee?>> GetById(Guid id)
        {
            var emp = await _repo.GetByIdAsync(id);
            if (emp == null)
                return NotFound();

            return Ok(emp);
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAll([FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            return Ok(await _repo.GetAllAsync(offset, limit));
        }

        [HttpPost]
        public async Task<IActionResult> Add(Employee employee)
        {
            employee.Id = Guid.NewGuid();
            await _repo.AddAsync(employee);
            return Ok(employee);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Employee employee)
        {
            await _repo.UpdateAsync(employee);
            return Ok("Updated");
        }
    }
}
