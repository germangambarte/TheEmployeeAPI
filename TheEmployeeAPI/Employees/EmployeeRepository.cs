using TheEmployeeAPI.Abstractions;

namespace TheEmployeeAPI;

public class EmployeeRepository : IRepository<Employee>
{
    private readonly List<Employee> _employees = new();

    public Employee? GetById(int id)
    {
        return _employees.FirstOrDefault(e => e.Id == id);
    }

    public IEnumerable<Employee> GetAll()
    {
        return _employees;
    }

    public void Create(Employee entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.Id = _employees.Select(e => e.Id).DefaultIfEmpty(0).Max() + 1;
        _employees.Add(entity);
    }


    public void Update(Employee entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existingEmployee = GetById(entity.Id);
        ArgumentNullException.ThrowIfNull(existingEmployee);
        existingEmployee.FirstName = entity.FirstName;
        existingEmployee.LastName = entity.LastName;
    }

    public void Delete(Employee entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _employees.Remove(entity);
    }
}