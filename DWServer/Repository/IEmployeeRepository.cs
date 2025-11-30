using Shared.Models;

namespace DWServer.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(Guid id);
        Task<List<Employee>> GetAllAsync(int offset, int limit);
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
    }
}
