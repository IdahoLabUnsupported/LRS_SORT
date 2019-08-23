namespace LRS.Interfaces
{
    public interface IEmployee
    {
        string EmployeeId { get; }
        string UserNetworkId { get; }
        string FullName { get; }
        string WorkOrginization { get; }
        string HomeOrginization { get; }
        string PhoneNumber { get; }
        string FirstName { get; }
        string MiddleName { get; }
        string LastName { get; }
        string NameSuffix { get; }
        string Email { get; }
        string WorkLocation { get; }
        string OrcidId { get; }
        string ManagerId { get; }
        string PreferredName { get; }
    }
}
