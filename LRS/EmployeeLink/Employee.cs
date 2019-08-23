using LRS.Interfaces;

namespace EmployeLink
{
    public class Employee : IEmployee
    {
        public string EmployeeId { get; }

        public string UserNetworkId { get; }

        public string FullName { get; }

        public string WorkOrginization { get; }

        public string HomeOrginization { get; }

        public string PhoneNumber { get; }

        public string FirstName { get; }

        public string MiddleName { get; }

        public string LastName { get; }

        public string NameSuffix { get; }

        public string Email { get; }

        public string WorkLocation { get; }

        public string OrcidId { get; }

        public string ManagerId { get; }

        public string PreferredName { get; }

        internal Employee() { }
    }
}
