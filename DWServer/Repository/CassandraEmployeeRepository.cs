using Cassandra;
using Shared.Models;

namespace DWServer.Repositories
{
    public class CassandraEmployeeRepository : IEmployeeRepository
    {
        private readonly Cassandra.ISession _session;


        public CassandraEmployeeRepository()
        {
            // Conectare la Cassandra
            var cluster = Cluster.Builder()
                .AddContactPoint("cassandra")
                .WithPort(9042)
                .Build();

            _session = cluster.Connect("pad");  // folosim keyspace-ul creat în docker
        }

        public async Task AddAsync(Employee employee)
        {
            var query = "INSERT INTO employees (id, firstname, lastname, position, salary) VALUES (?, ?, ?, ?, ?)";

            await _session.ExecuteAsync(new SimpleStatement(
                query,
                employee.Id,
                employee.FirstName,
                employee.LastName,
                employee.Position,
                employee.Salary
            ));
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM employees WHERE id = ?";

            var result = await _session.ExecuteAsync(new SimpleStatement(query, id));
            var row = result.FirstOrDefault();

            if (row == null)
                return null;

            return new Employee
            {
                Id = row.GetValue<int>("id"),
                FirstName = row.GetValue<string>("firstname"),
                LastName = row.GetValue<string>("lastname"),
                Position = row.GetValue<string>("position"),
                Salary = row.GetValue<double>("salary")
            };
        }

        public async Task<List<Employee>> GetAllAsync(int offset, int limit)
        {
            var query = "SELECT * FROM employees";
            var result = await _session.ExecuteAsync(new SimpleStatement(query));

            return result
                .Select(row => new Employee
                {
                    Id = row.GetValue<int>("id"),
                    FirstName = row.GetValue<string>("firstname"),
                    LastName = row.GetValue<string>("lastname"),
                    Position = row.GetValue<string>("position"),
                    Salary = row.GetValue<double>("salary")
                })
                .Skip(offset)
                .Take(limit)
                .ToList();
        }

        public async Task UpdateAsync(Employee employee)
        {
            var query = "UPDATE employees SET firstname = ?, lastname = ?, position = ?, salary = ? WHERE id = ?";

            await _session.ExecuteAsync(new SimpleStatement(
                query,
                employee.FirstName,
                employee.LastName,
                employee.Position,
                employee.Salary,
                employee.Id
            ));
        }
    }
}
