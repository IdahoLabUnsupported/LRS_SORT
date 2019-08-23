using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public interface IFunding
    {
        #region Properties

        int? FundingId { get; }
        int SortMainId { get; }

        [Display(Name = "Funding Source")]
        int FundingTypeId { get; }

        [Display(Name = "Fiscal Year")]
        [StringLength(4)]
        string Year { get; }

        [Display(Name = "Percent"), MaxLength(3)]
        [StringLength(6)]
        string Percent { get; }

        [Display(Name = "Contract Number"), MaxLength(250)]
        [StringLength(250)]
        string ContractNumber { get; }

        [Display(Name = "Funding Org"), MaxLength(10)]
        [StringLength(10)]
        string Org { get; }

        [Display(Name = "Project Area"), MaxLength(250)]
        [StringLength(250)]
        string ProjectArea { get; }

        [Display(Name = "DOE Program")]
        int? DoeFundingCategoryId { get; }

        [Display(Name = "Grant Number"), MaxLength(250)]
        [StringLength(250)]
        string GrantNumber { get; }

        [Display(Name = "Tracking Number"), MaxLength(250)]
        [StringLength(250)]
        string TrackingNumber { get; }

        [Display(Name = "Project Number"), MaxLength(250)]
        [StringLength(250)]
        string ProjectNumber { get; }

        [Display(Name = "Principal Investigator")]
        [StringLength(6)]
        string PrincipalInvEmployeeId { get; }

        [Display(Name = "SPP Category")]
        int? SppCategoryId { get; }

        [Display(Name = "Federal Agency")]
        int? FederalAgencyId { get; }

        [Display(Name = "Description")]
        string OtherDescription { get; }

        [Display(Name = "Country")]
        int? CountryId { get; }

        [Display(Name = "Additional Info")]
        string AdditionalInfo { get; }

        [Display(Name = "Approved")]
        bool SppApproved { get; }

        [Display(Name = "Not SPP Approved Reason")]
        string ApproveNoReason { get; }

        [Display(Name = "Milestone Tracking Number")]
        string MilestoneTrackingNumber { get; }

        List<FundingObject> Funding { get; }

        string DoeFundingNumber { get; }
        #endregion
    }
}
