using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public interface IContact
    {
        int? ContactId { get; }
        int SortMainId { get; }
        [Display(Name = "Contact Name")]
        string EmployeeId { get; }
        [Display(Name = "Contact Type")]
        string ContactType { get; }
        List<ContactObject> Contacts { get; }
    }
}
