using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using DWServer.Repositories;

namespace DWServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeController(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        // GET /employee/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee?>> GetById(int id)
        {
            var emp = await _repository.GetByIdAsync(id);

            if (emp == null)
                return NotFound();

            return Ok(emp);
        }

        // GET /employee?offset=0&limit=10
        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAll([FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            var list = await _repository.GetAllAsync(offset, limit);
            return Ok(list);
        }

        // PUT /employee
        [HttpPut]
        public async Task<IActionResult> Add([FromBody] Employee employee)
        {
            await _repository.AddAsync(employee);
            return Ok(employee);
        }

        // POST /employee/update
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Employee employee)
        {
            await _repository.UpdateAsync(employee);
            return Ok("Updated");
        }
    }
}
