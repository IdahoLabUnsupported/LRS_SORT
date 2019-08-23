using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Sort.Business
{
    public class FundingObject
    {
        #region Properties

        public int FundingId { get; set; }
        public int SortMainId { get; set; }
        public int FundingTypeId { get; set; }
        public int? DoeFundingCategoryId { get; set; }
        public string GrantNumber { get; set; }
        [Display(Name = "Fiscal Year")]
        public string Year { get; set; }
        public string Org { get; set; }
        public string Percent { get; set; }
        [Display(Name = "Contract Number")]
        public string ContractNumber { get; set; }
        public string ProjectArea { get; set; }
        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; }
        [Display(Name = "Project Number")]
        public string ProjectNumber { get; set; }
        public string PrincipalInvEmployeeId { get; set; }
        public int? SppCategoryId { get; set; }
        public int? FederalAgencyId { get; set; }
        [Display(Name = "Description")]
        public string OtherDescription { get; set; }
        public int? CountryId { get; set; }
        public string AdditionalInfo { get; set; }
        public bool? SppApproved { get; set; }
        [Display(Name = "Not SPP Approved Reason")]
        public string ApproveNoReason { get; set; }
        [Display(Name = "Milestone Tracking Number")]
        public string MilestoneTrackingNumber { get; set; }
        #endregion

        #region Extended Properties
        [Display(Name="Funding Source")]
        public string FundingType => FundingTypeObject.GetFundingType(FundingTypeId)?.FundingType;

        [Display(Name = "DOE Program")]
        public string DoeFundingCategory => MemoryCache.GetDoeFunding(DoeFundingCategoryId??0)?.Description;

        [Display(Name = "Funding Org")]
        public string FundingOrgName => Config.EmployeeManager.GetOrganization(Org)?.DisplayName;

        public string FederalAgency => MemoryCache.GetSppFundingFederalAgency(FederalAgencyId ?? 0)?.FederalAgency;

        public string SppCountry => MemoryCache.GetCountry(CountryId ?? 0)?.Country;

        public string DetailInfo
        {
            get
            {
                string details = String.Empty;
                switch (FundingTypeId)
                {
                    case 1: //DOE
                        details = MemoryCache.GetDoeFunding(DoeFundingCategoryId ?? 0)?.Description; 
                        break;
                    case 2: //LDRD
                        details = $"{TrackingNumber} - {ProjectNumber}";
                        break;
                    case 4: //Other
                        details = $"{OtherDescription}";
                        break;
                    case 5: //spp
                        details = MemoryCache.GetSppFunding(SppCategoryId ?? 0)?.Category;
                        switch (SppCategoryId)
                        {
                            case 1://Federal
                                details += " - " + MemoryCache.GetSppFundingFederalAgency(FederalAgencyId ?? 0)?.FederalAgency;
                                break;
                            case 6://Other
                                details += " - " + OtherDescription;
                                break;
                            case 7://Foreign
                                details += " - " + MemoryCache.GetCountry(CountryId ?? 0)?.Country;
                                break;
                        }
                        break;
                    case 6: //Grant
                    case 7: //NEUP
                        details = $"{GrantNumber}";
                        break;
                }

                return details;
            }
        }

        [Display(Name = "Percent")]
        public string PercentFmt => $"{Percent} %";

        [Display(Name = "Principal Investigator")]
        public string PrincipalInvName => UserObject.GetUser(PrincipalInvEmployeeId)?.FullName;

        [Display(Name = "SPP Category")]
        public string SppFundingName => MemoryCache.GetSppFunding(SppCategoryId ?? 0)?.Category;

        public string SppApprovedDisplay => (SppApproved ?? false) ? "True" : "False";

        public bool IsValid
        {
            get
            {
                bool valid = true;
                try
                {
                    int num = 0;
                    double dbl = 0.0;
                    if (string.IsNullOrWhiteSpace(Year) || !int.TryParse(Year, out num) || num < 1900 || num > 2500) valid = false;
                    if (string.IsNullOrWhiteSpace(ContractNumber)) valid = false;
                    if (string.IsNullOrWhiteSpace(Percent) || !double.TryParse(Percent, out dbl) || dbl <= 0 || dbl > 100) valid = false;
                    if (string.IsNullOrWhiteSpace(Org) || string.IsNullOrWhiteSpace(FundingOrgName)) valid = false;
                    if (FundingTypeId <= 0) { valid = false; }
                    else
                    {
                        switch (FundingType.ToUpper(CultureInfo.InvariantCulture))
                        {
                            case "DOE":
                                if (!DoeFundingCategoryId.HasValue) valid = false; 
                                break;
                            case "LDRD":
                                if (string.IsNullOrWhiteSpace(TrackingNumber)) valid = false;
                                break;
                            case "GRANT":
                            case "NEUP":
                                if (string.IsNullOrWhiteSpace(GrantNumber)) valid = false;
                                break;
                            case "OTHER":
                                if (string.IsNullOrWhiteSpace(OtherDescription)) valid = false;
                                break;
                            case "SPP":
                                if (!SppCategoryId.HasValue) { valid = false; }
                                else
                                {
                                    if (!SppApproved.HasValue || (!SppApproved.Value && string.IsNullOrWhiteSpace(ApproveNoReason))) valid = false;
                                    switch (SppCategoryId)
                                    {
                                        case 1://Federal
                                            if (!FederalAgencyId.HasValue) valid = false;
                                            break;
                                        case 6://Other
                                            if (string.IsNullOrWhiteSpace(OtherDescription)) valid = false;
                                            break;
                                        case 7://Foreign
                                            if (!CountryId.HasValue) valid = false;
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogObject.LogError("FundingObject::IsValid", ex);
                    valid = false;
                }

                return valid;
            }
        }

        public string SortTitle { get; set; }

        public string TitleFundingSourceName
        {
            get
            {
                if (MemoryCache.GetFundingType(FundingTypeId)?.FundingType.Equals("DOE", StringComparison.OrdinalIgnoreCase)??false)
                {
                    return MemoryCache.GetDoeFunding(DoeFundingCategoryId??0)?.CoverSheetDescription;
                }
                else
                {
                    return "Office of Nuclear Energy";
                }
            }
        }

        #endregion

        #region Constructor

        public FundingObject() { }

        #endregion

        #region Repository

        private static IFundingRepository repo => new FundingRepository();

        #endregion

        #region Static Methods

        public static List<FundingObject> GetFundings(int sortMainId) => repo.GetFundings(sortMainId);

        public static FundingObject GetFunding(int fundingId) => repo.GetFunding(fundingId);

        public static void Add(int sortMainId, int fiscalYear, int fundingTypeId, string fundingOrg, int fundingPercent)
        {
            FundingObject o = new FundingObject();
            o.SortMainId = sortMainId;
            o.Year = fiscalYear.ToString();
            o.FundingTypeId = fundingTypeId;
            o.Org = fundingOrg;
            o.Percent = fundingPercent.ToString();
            o.Save();
        }

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveFunding(this);
            SortMainObject.CheckStatusUpdate(SortMainId, true);
        }

        public void Delete()
        {
            repo.DeleteFunding(this);
            SortMainObject.CheckStatusUpdate(SortMainId, true);
        }

        #endregion
    }

    public interface IFundingRepository
    {
        List<FundingObject> GetFundings(int sortMainId);
        FundingObject GetFunding(int fundingId);
        FundingObject SaveFunding(FundingObject funding);
        bool DeleteFunding(FundingObject funding);
    }

    public class FundingRepository : IFundingRepository
    {
        public List<FundingObject> GetFundings(int sortMainId)
        {
            string sql = @" SELECT f.*, s.Title as 'SortTitle' 
                            FROM dat_Funding f 
                            inner join dat_SortMain s on s.SortMainId = f.SortMainId
                            WHERE f.SortMainId = @SortMainId";

            return Config.Conn.Query<FundingObject>(sql, new { SortMainId = sortMainId }).ToList();
        }

        public FundingObject GetFunding(int fundingId)
        {
            string sql = @" SELECT f.*, s.Title as 'SortTitle' 
                            FROM dat_Funding f 
                            inner join dat_SortMain s on s.SortMainId = f.SortMainId
                            WHERE f.FundingId = @FundingId";

            return Config.Conn.Query<FundingObject>(sql, new { FundingId = fundingId }).FirstOrDefault();
        } 

        public FundingObject SaveFunding(FundingObject funding)
        {
            if (funding.FundingId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_Funding
                    SET     SortMainId = @SortMainId,
                            FundingTypeId = @FundingTypeId,
                            DoeFundingCategoryId = @DoeFundingCategoryId,
                            GrantNumber = @GrantNumber,
                            [Year] = @Year,
                            Org = @Org,
                            [Percent] = @Percent,
                            ContractNumber = @ContractNumber,
                            ProjectArea = @ProjectArea,
                            TrackingNumber = @TrackingNumber,
                            ProjectNumber = @ProjectNumber,
                            PrincipalInvEmployeeId = @PrincipalInvEmployeeId,
                            SppCategoryId = @SppCategoryId,
                            FederalAgencyId = @FederalAgencyId,
                            OtherDescription = @OtherDescription,
                            CountryId = @CountryId,
                            AdditionalInfo = @AdditionalInfo,
                            SppApproved = @SppApproved,
                            ApproveNoReason = @ApproveNoReason,
                            MilestoneTrackingNumber = @MilestoneTrackingNumber
                    WHERE   FundingId = @FundingId";
                Config.Conn.Execute(sql, funding);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_Funding (
                        SortMainId,
                        FundingTypeId,
                        DoeFundingCategoryId,
                        GrantNumber,
                        [Year],
                        Org,
                        [Percent],
                        ContractNumber,
                        ProjectArea,
                        TrackingNumber,
                        ProjectNumber,
                        PrincipalInvEmployeeId,
                        SppCategoryId,
                        FederalAgencyId,
                        OtherDescription,
                        CountryId,
                        AdditionalInfo,
                        SppApproved,
                        ApproveNoReason,
                        MilestoneTrackingNumber
                    )
                    VALUES (
                        @SortMainId,
                        @FundingTypeId,
                        @DoeFundingCategoryId,
                        @GrantNumber,
                        @Year,
                        @Org,
                        @Percent,
                        @ContractNumber,
                        @ProjectArea,
                        @TrackingNumber,
                        @ProjectNumber,
                        @PrincipalInvEmployeeId,
                        @SppCategoryId,
                        @FederalAgencyId,
                        @OtherDescription,
                        @CountryId,
                        @AdditionalInfo,
                        @SppApproved,
                        @ApproveNoReason,
                        @MilestoneTrackingNumber
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                funding.FundingId = Config.Conn.Query<int>(sql, funding).Single();
            }
            return funding;
        }

        public bool DeleteFunding(FundingObject funding)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_Funding WHERE FundingId = @FundingId", funding);
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("FundingObject::DeleteFunding", ex);
                return false;
            }
            return true;
        }
    }
}