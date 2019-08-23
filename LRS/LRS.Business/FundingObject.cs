using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace LRS.Business
{
    public class FundingObject
    {
        #region Properties

        public int FundingId { get; set; }
        public int MainId { get; set; }
        public string Year { get; set; }
        public int FundingTypeId { get; set; }
        public string Org { get; set; }
        public string ContractNumber { get; set; }
        public string Percent { get; set; }
        public int? DoeFundingCategoryId { get; set; }
        public string GrantNumber { get; set; }
        public string TrackingNumber { get; set; }
        public int? SppCategoryId { get; set; }
        public bool? SppApproved { get; set; }
        public int? FederalAgencyId { get; set; }
        public string ApproveNoReason { get; set; }
        public string OtherDescription { get; set; }
        public int? CountryId { get; set; }
        public string AdditionalInfo { get; set; }
        public string ProjectArea { get; set; }
        public string ProjectNumber { get; set; }
        public string PrincipalInvEmployeeId { get; set; }
        public string MilestoneTrackingNumber { get; set; }

        #endregion

        #region Extended Properties
        [Display(Name = "Funding Source")]
        public string FundingType => FundingTypeObject.GetFundingType(FundingTypeId)?.FundingType;

        [Display(Name = "DOE Program")]
        public string DoeFundingCategory => MemoryCache.GetDoeFunding(DoeFundingCategoryId ?? 0)?.Description;

        [Display(Name = "Funding Org")]
        public string FundingOrgName => OrgListObject.GetOrg(Org)?.ExtendedName;

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
                catch
                {
                    valid = false;
                }

                return valid;
            }
        }

        public string MainTitle => MainObject.GetMain(MainId)?.DisplayTitle;

        #endregion

        #region Constructor

        public FundingObject() { }

        #endregion

        #region Repository

        private static IFundingRepository repo => new FundingRepository();

        #endregion

        #region Static Methods

        public static List<FundingObject> GetFundings(int mainId) => repo.GetFundings(mainId);

        public static FundingObject GetFunding(int fundingId) => repo.GetFunding(fundingId);

        public static bool CopyData(int fromMainId, int toMainId) => repo.CopyData(fromMainId, toMainId);

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveFunding(this);
            MainObject.UpdateActivityDateToNow(MainId);
        }

        public void Delete()
        {
            repo.DeleteFunding(this);
            MainObject.UpdateActivityDateToNow(MainId);
        }

        #endregion
    }

    public interface IFundingRepository
    {
        List<FundingObject> GetFundings(int mainId);
        FundingObject GetFunding(int fundingId);
        FundingObject SaveFunding(FundingObject funding);
        bool DeleteFunding(FundingObject funding);
        bool CopyData(int fromMainId, int toMainId);
    }

    public class FundingRepository : IFundingRepository
    {
        public List<FundingObject> GetFundings(int mainId) => Config.Conn.Query<FundingObject>("SELECT * FROM dat_Funding WHERE MainId = @MainId", new { MainId = mainId }).ToList();

        public FundingObject GetFunding(int fundingId) => Config.Conn.Query<FundingObject>("SELECT * FROM dat_Funding WHERE FundingId = @FundingId", new { FundingId = fundingId }).FirstOrDefault();

        public FundingObject SaveFunding(FundingObject funding)
        {
            if (funding.FundingId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_Funding
                    SET     Year = @Year,
                            FundingTypeId = @FundingTypeId,
                            Org = @Org,
                            ContractNumber = @ContractNumber,
                            [Percent] = @Percent,
                            DoeFundingCategoryId = @DoeFundingCategoryId,
                            GrantNumber = @GrantNumber,
                            TrackingNumber = @TrackingNumber,
                            SppCategoryId = @SppCategoryId,
                            SppApproved = @SppApproved,
                            FederalAgencyId = @FederalAgencyId,
                            ApproveNoReason = @ApproveNoReason,
                            OtherDescription = @OtherDescription,
                            CountryId = @CountryId,
                            AdditionalInfo = @AdditionalInfo,
                            ProjectArea = @ProjectArea,
                            ProjectNumber = @ProjectNumber,
                            PrincipalInvEmployeeId = @PrincipalInvEmployeeId,
                            MilestoneTrackingNumber = @MilestoneTrackingNumber
                    WHERE   FundingId = @FundingId";
                Config.Conn.Execute(sql, funding);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_Funding (
                        MainId,
                        Year,
                        FundingTypeId,
                        Org,
                        ContractNumber,
                        [Percent],
                        DoeFundingCategoryId,
                        GrantNumber,
                        TrackingNumber,
                        SppCategoryId,
                        SppApproved,
                        FederalAgencyId,
                        ApproveNoReason,
                        OtherDescription,
                        CountryId,
                        AdditionalInfo,
                        ProjectArea,
                        ProjectNumber,
                        PrincipalInvEmployeeId,
                        MilestoneTrackingNumber
                    )
                    VALUES (
                        @MainId,
                        @Year,
                        @FundingTypeId,
                        @Org,
                        @ContractNumber,
                        @Percent,
                        @DoeFundingCategoryId,
                        @GrantNumber,
                        @TrackingNumber,
                        @SppCategoryId,
                        @SppApproved,
                        @FederalAgencyId,
                        @ApproveNoReason,
                        @OtherDescription,
                        @CountryId,
                        @AdditionalInfo,
                        @ProjectArea,
                        @ProjectNumber,
                        @PrincipalInvEmployeeId,
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
            catch { return false; }
            return true;
        }

        public bool CopyData(int fromMainId, int toMainId)
        {
            string sql = @" insert into dat_Funding (MainId, [Year], FundingTypeId, Org, ContractNumber, [Percent], DoeFundingCategoryId, GrantNumber,
						                             TrackingNumber, SppCategoryId, SppApproved, FederalAgencyId, ApproveNoReason, OtherDescription,
						                             CountryId, AdditionalInfo, ProjectArea, ProjectNumber, PrincipalInvEmployeeId, MilestoneTrackingNumber)
                            select	@NewMainId, [Year], FundingTypeId, Org, ContractNumber, [Percent], DoeFundingCategoryId, GrantNumber,
		                            TrackingNumber, SppCategoryId, SppApproved, FederalAgencyId, ApproveNoReason, OtherDescription,
		                            CountryId, AdditionalInfo, ProjectArea, ProjectNumber, PrincipalInvEmployeeId, MilestoneTrackingNumber
                            FROM dat_Funding
                            WHERE MainId = @OldMainId";

            try
            {
                Config.Conn.Execute(sql, new { NewMainId = toMainId, OldMainId = fromMainId });
            }
            catch { return false; }
            return true;
        }
    }
}