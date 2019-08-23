using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public interface IAuthor
    {
        int? AuthorId { get; }
        int SortMainId { get; }
        string Affiliation { get; }
        AffiliationEnum AffiliationType { get; } 
        string AffiliateEmployeeId { get; }
        string AuthorName { get; }
        [Display(Name = "Orcid ID")]
        string AuthorOrcidId { get; }
        bool IsPrimary { get; }
        List<AuthorObject> Authors { get; }

        [Display(Name = "Affiliation Country")]
        int? CountryId { get; }
        [Display(Name = "Affiliation State")]
        int? StateId { get; }
    }
}
