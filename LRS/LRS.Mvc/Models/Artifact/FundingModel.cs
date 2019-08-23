using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class FundingModel : IValidatableObject
    {
        #region Properties

        public int? FundingId { get; set; }
        [Required]
        public int MainId { get; set; }
        [Required, Display(Name = "Funding Source")]
        public int FundingTypeId { get; set; }
        [Required, Display(Name = "Fiscal Year"), StringLength(4)]
        public string Year { get; set; }
        [Required, Display(Name = "Percent"), StringLength(6)]
        public string Percent { get; set; }

        [Required, Display(Name = "Contract Number"), MaxLength(250), StringLength(250)]
        public string ContractNumber { get; set; } = Business.Config.DefaultFundingContractNumber;
        [StringLength(10), Display(Name = "Funding Org"), MaxLength(10)]
        public string Org { get; set; }
        [StringLength(250), Display(Name = "Project Area"), MaxLength(250)]
        public string ProjectArea { get; set; }
        [Display(Name = "DOE Program")]
        public int? DoeFundingCategoryId { get; set; }
        [StringLength(250), Display(Name = "Grant Number"), MaxLength(250)]
        public string GrantNumber { get; set; }
        [StringLength(250), Display(Name = "LDRD Tracking Number"), MaxLength(250)]
        public string TrackingNumber { get; set; }
        [StringLength(250), Display(Name = "Project Number"), MaxLength(250)]
        public string ProjectNumber { get; set; }
        [StringLength(6), Display(Name = "Principal Investigator")]
        public string PrincipalInvEmployeeId { get; set; }
        [Display(Name = "SPP Category")]
        public int? SppCategoryId { get; set; }
        [Display(Name = "Federal Agency")]
        public int? FederalAgencyId { get; set; }
        [Display(Name = "Description")]
        public string OtherDescription { get; set; }
        [Display(Name = "Country")]
        public int? CountryId { get; set; }
        [Display(Name = "Additional Info")]
        public string AdditionalInfo { get; set; }
        [Display(Name = "Approved")]
        public bool SppApproved { get; set; }
        [Display(Name = "Not SPP Approved Reason")]
        public string ApproveNoReason { get; set; }
        [Display(Name = "Milestone Tracking Number")]
        public string MilestoneTrackingNumber { get; set; }
        public List<FundingObject> Funding { get; set; }

        public string DefaultContractNumber => Business.Config.DefaultFundingContractNumber;
        #endregion

        #region Constructor

        public FundingModel() { }
        public FundingModel(int mainId, int? fundingId)
        {
            MainId = mainId;
            Funding = FundingObject.GetFundings(MainId);

            if (fundingId.HasValue)
            {
                var funding = FundingObject.GetFunding(fundingId.Value);
                if (funding != null)
                {
                    FundingId = funding.FundingId;
                    FundingTypeId = funding.FundingTypeId;
                    Year = funding.Year;
                    Percent = funding.Percent;
                    ContractNumber = funding.ContractNumber;
                    Org = funding.Org;
                    ProjectArea = funding.ProjectArea;
                    DoeFundingCategoryId = funding.DoeFundingCategoryId;
                    GrantNumber = funding.GrantNumber;
                    TrackingNumber = funding.TrackingNumber;
                    ProjectNumber = funding.ProjectNumber;
                    PrincipalInvEmployeeId = funding.PrincipalInvEmployeeId;
                    SppCategoryId = funding.SppCategoryId;
                    FederalAgencyId = funding.FederalAgencyId;
                    OtherDescription = funding.OtherDescription;
                    CountryId = funding.CountryId;
                    AdditionalInfo = funding.AdditionalInfo;
                    SppApproved = funding.SppApproved ?? false;
                    ApproveNoReason = funding.ApproveNoReason;
                    MilestoneTrackingNumber = funding.MilestoneTrackingNumber;
                }
            }
        }
        #endregion

        #region Functions

        public void Save()
        {
            FundingObject o = new FundingObject();
            if (FundingId.HasValue && FundingId.Value > 0)
            {
                o = FundingObject.GetFunding(FundingId.Value);
            }

            o.MainId = MainId;
            o.Year = Year;
            o.FundingTypeId = FundingTypeId;
            o.ContractNumber = ContractNumber;
            o.Percent = Percent.Replace("%", "").Trim();
            o.Org = Org;
            o.ProjectArea = ProjectArea;
            o.DoeFundingCategoryId = DoeFundingCategoryId;
            o.GrantNumber = GrantNumber;
            o.TrackingNumber = TrackingNumber;
            o.ProjectNumber = ProjectNumber;
            o.PrincipalInvEmployeeId = PrincipalInvEmployeeId;
            o.SppCategoryId = SppCategoryId;
            o.FederalAgencyId = FederalAgencyId;
            o.OtherDescription = OtherDescription;
            o.CountryId = CountryId;
            o.AdditionalInfo = AdditionalInfo;
            o.SppApproved = SppApproved;
            o.ApproveNoReason = ApproveNoReason;
            o.MilestoneTrackingNumber = MilestoneTrackingNumber;
            o.Save();
        }

        #endregion

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            switch (FundingTypeId)
            {
                case 1: //DOE
                    if (!DoeFundingCategoryId.HasValue)
                    {
                        yield return new ValidationResult("DOE Program is required", new[] { "DoeFundingCategoryId" });
                    }
                    break;
                case 2://LDRD
                    if (string.IsNullOrWhiteSpace(TrackingNumber))
                    {
                        yield return new ValidationResult("Tracking Number is required", new[] { "TrackingNumber" });
                    }
                    break;
                case 4://Other
                    if (string.IsNullOrWhiteSpace(OtherDescription))
                    {
                        yield return new ValidationResult("Description is required", new[] { "OtherDescription" });
                    }
                    break;
                case 5://SPP
                    if (!SppApproved && string.IsNullOrWhiteSpace(ApproveNoReason))
                    {
                        yield return new ValidationResult("Not SPP Approved Reason is required", new[] { "ApproveNoReason" });
                    }
                    if (!SppCategoryId.HasValue)
                    {
                        yield return new ValidationResult("SPP Category is required", new[] { "SppCategoryId" });
                    }
                    else
                    {
                        switch (SppCategoryId.Value)
                        {
                            case 1: //Federal Agency
                                if (!FederalAgencyId.HasValue)
                                {
                                    yield return new ValidationResult("Federal Agency is required", new[] { "FederalAgencyId" });
                                }
                                break;
                            case 6: //Other
                                if (string.IsNullOrWhiteSpace(OtherDescription))
                                {
                                    yield return new ValidationResult("Description is required", new[] { "OtherDescription" });
                                }
                                break;
                            case 7: //Foreign
                                if (!CountryId.HasValue)
                                {
                                    yield return new ValidationResult("Country is required", new[] { "CountryId" });
                                }
                                break;
                        }
                    }
                    break;
                case 6: //Grant
                case 7: //NEUP
                    if (string.IsNullOrWhiteSpace(GrantNumber))
                    {
                        yield return new ValidationResult("Grant Number is required", new[] { "GrantNumber" });
                    }
                    break;
            }
        }

        #endregion
    }
}