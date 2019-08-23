using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class UserOrgsModel
    {
        [Required]
        public string UserEmployeeId { get; set; }

        [Required]
        public List<string> Org { get; set; } = new List<string>();

        public List<UserOrgObject> UserOrgs { get; set; } = new List<UserOrgObject>();

        public void Save() => Org.ForEach(n => new UserOrgObject() { EmployeeId = UserEmployeeId, Org = n }.Save());

        [Display(Name = "Org Manager")]
        public string UserName => UserObject.GetUser(UserEmployeeId)?.FullName;

        public UserOrgsModel() { }

        public UserOrgsModel(string employeeId)
        {
            UserEmployeeId = employeeId;
            UserOrgs = UserOrgObject.GetUserOrgs(employeeId);
        }
    }
}