using System.Collections.Generic;

namespace Sort.Interfaces
{
    public interface IEmployeeManager
    {
        IEnumerable<IEmployee> GetAllEmployees();
        IEmployee GetEmployeeById(string id, bool includeTerminated);
        IEmployee GetEmployeeByName(string fullName, bool includeTerminated);
        IEmployee GetEmployeeByName(string firstName, string lastName, bool includeTerminated);
        IEmployee GetEmployeeByUserName(string userName, bool includeTerminated);
        IEnumerable<IEmployee> SearchEmployee(string text, bool includeTerminated);
        IEnumerable<IOrganization> GetAllOrganizations();
        IOrganization GetOrganization(string id);
    }
}
