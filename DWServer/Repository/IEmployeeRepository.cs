using Shared.Models;

namespace DWServer.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(int id);
        Task<List<Employee>> GetAllAsync(int offset, int limit);
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
    }
}
