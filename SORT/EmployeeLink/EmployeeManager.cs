using System;
using System.Collections.Generic;
using Sort.Interfaces;

namespace EmployeeLink
{
    public class EmployeeManager : IEmployeeManager
    {
        public EmployeeManager(string defaultConnectionString, string employeeConnectionString)
        {
            Config.DefaultConnectionString = defaultConnectionString;
            Config.EmployeeConnectionString = employeeConnectionString;
        }

        public IEnumerable<IEmployee> GetAllEmployees()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IOrganization> GetAllOrganizations()
        {
            throw new NotImplementedException();
        }

        public IEmployee GetEmployeeById(string id, bool includeTerminated)
        {
            throw new NotImplementedException();
        }

        public IEmployee GetEmployeeByName(string fullName, bool includeTerminated)
        {
            throw new NotImplementedException();
        }

        public IEmployee GetEmployeeByName(string firstName, string lastName, bool includeTerminated)
        {
            throw new NotImplementedException();
        }

        public IEmployee GetEmployeeByUserName(string userName, bool includeTerminated)
        {
            throw new NotImplementedException();
        }

        public IOrganization GetOrganization(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEmployee> SearchEmployee(string text, bool includeTerminated)
        {
            throw new NotImplementedException();
        }
    }
}
