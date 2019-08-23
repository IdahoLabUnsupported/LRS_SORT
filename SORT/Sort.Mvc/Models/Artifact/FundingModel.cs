using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class FundingModel : IFunding, IValidatableObject
    {
        #region Properties

        public int? FundingId { get; set; }
        [Required]
        public int SortMainId { get; set; }
        [Required]
        public int FundingTypeId { get; set; }
        [Required]
        [StringLength(4)]
        public string Year { get; set; }
        [Required]
        [StringLength(6)]
        public string Percent { get; set; }

        [Required, Display(Name = "Contract Number"), MaxLength(250), StringLength(250)]
        public string ContractNumber { get; set; }
        [StringLength(10)]
        public string Org { get; set; }
        [StringLength(250)]
        public string ProjectArea { get; set; }
        public int? DoeFundingCategoryId { get; set; }
        [StringLength(250)]
        public string GrantNumber { get; set; }
        [StringLength(250)]
        public string TrackingNumber { get; set; }
        [StringLength(250)]
        public string ProjectNumber { get; set; }
        [StringLength(6)]
        public string PrincipalInvEmployeeId { get; set; }
        public int? SppCategoryId { get; set; }
        public int? FederalAgencyId { get; set; }
        public string OtherDescription { get; set; }
        public int? CountryId { get; set; }
        public string AdditionalInfo { get; set; }
        public bool SppApproved { get; set; }
        public string ApproveNoReason { get; set; }
        public string MilestoneTrackingNumber { get; set; }
        public List<FundingObject> Funding { get; set; }
        #endregion

        #region Extended Properties
        public string DoeFundingNumber => Config.DoeContractNumber;
        #endregion

        #region Constructor

        public FundingModel() {}
        public FundingModel(FundingObject funding)
        {
            if (funding != null)
            {
                SortMainId = funding.SortMainId;
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
                SppApproved = funding.SppApproved??false;
                ApproveNoReason = funding.ApproveNoReason;
                MilestoneTrackingNumber = funding.MilestoneTrackingNumber;
                Funding = FundingObject.GetFundings(SortMainId);
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

            o.SortMainId = SortMainId;
            o.Year = Year;
            o.FundingTypeId = FundingTypeId;
            o.ContractNumber = ContractNumber;
            o.Percent = Percent.Replace("%","").Trim();
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
                    if(!SppApproved && string.IsNullOrWhiteSpace(ApproveNoReason))
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