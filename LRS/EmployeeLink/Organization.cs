using LRS.Interfaces;

namespace EmployeLink
{
    public class Organization : IOrganization
    {
        public string OrgId { get; }

        public string Description { get; }

        internal Organization() { }
    }
}
