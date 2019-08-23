using LRS.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LRS.Mvc.Models
{
	public class EditUserModel
	{
		#region Properties

		[Required, Display(Name = "Employee")]
		public string EmployeeId { get; set; }

	    public List<string> Roles { get; set; } = new List<string>();

	    public string Name { get; set; }

        #endregion

        #region List

        public SelectList GetRoles()
		{
			var roles = Enum.GetNames(typeof(UserRole)).ToList();
			roles.Insert(0, "- none -");
			return new SelectList(roles);
		}
        
		#endregion

		#region Constructor

		public EditUserModel() { }

		public EditUserModel(string employeeId)
		{
			if (!string.IsNullOrEmpty(employeeId))
			{
				UserObject o = UserObject.GetUser(employeeId);
				if (o != null)
				{
					EmployeeId = o.EmployeeId;
                    Roles.AddRange(o.Roles);
				    Name = o.FullName;
                }
			}
		}

        #endregion

        #region Object Methods

        public void Save()
        {
            UserObject o = UserObject.GetUser(EmployeeId) ?? new UserObject();
            o.EmployeeId = EmployeeId;
            o.Roles.Clear();
            o.Roles.AddRange(Roles);
            o.Save();
        }

	    public void Delete() => UserObject.GetUser(EmployeeId)?.Delete();

		#endregion
	}
}