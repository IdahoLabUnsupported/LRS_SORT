using LRS.Business;

namespace LRS.Mvc.Models
{
    public class UserInfoModel
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        public string Org { get; set; }
        public string OrcidId { get; set; }

        public UserInfoModel() { }

        public UserInfoModel(string employeeId)
        {
            EmployeeId = employeeId;
            var person = EmployeeCache.GetEmployee(employeeId, true);
            if (person != null)
            {
                Name = person.PreferredName;
                Phone = person.PhoneNumber;
                Location = person.WorkLocation;
                Org = person.HomeOrginization;
                OrcidId = person.OrcidId;
            }
        }
    }
}