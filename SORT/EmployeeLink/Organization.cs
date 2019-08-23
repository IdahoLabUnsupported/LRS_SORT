using Sort.Interfaces;

namespace EmployeeLink
{
    public class Organization : IOrganization
    {
        public string OrgId { get; }

        public string Description { get; }

        public string DisplayName => $"{OrgId} {Description}";

        internal Organization() { }
    }
}
