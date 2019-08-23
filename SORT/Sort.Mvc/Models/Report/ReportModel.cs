using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class ReportModel
    {
        #region Public Properties
        [Required, Display(Name = "Search on Date")]
        public SearchDates SearchDateType { get; set; }
        [Required, Display(Name = "From Date [Greater Than]")]
        public DateTime StartTime { get; set; }
        [Required, Display(Name = "To Date [Less Than]")]
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        [Display(Name = "Artifact Type")]
        public string ProductType { get; set; }
        [Display(Name = "Access Limitation")]
        public string AccessLimitation { get; set; }
        [Display(Name = "Publication Language")]
        public string Language { get; set; }
        [Display(Name = "Country of Origin/Publication")]
        public string Country { get; set; }
        [Display(Name = "Title [Like]")]
        public string PublishTitle { get; set; }
        [Display(Name = "Report/Product Number(s) [Like]")]
        public string ReportNumbers { get; set; }
        public string Owner { get; set; }
        [Display(Name = "Name")]
        public string Contact { get; set; }
        [Display(Name = "Name [Like]")]
        public string Author { get; set; }
        [Display(Name = "Contact Type")]
        public string ContactType { get; set; }

        [Display(Name = "Work Org")]
        public List<string> ContactWorkOrg { get; set; } = new List<string>();
        [Display(Name = "Affiliation [Like]")]
        public string Affiliation { get; set; }
        [Display(Name = "Orcid ID [Like]")]
        public string OrcidId { get; set; }
        [Display(Name = "Work Org")]
        public List<string> AuthorWorkOrg { get; set; } = new List<string>();

        [Display(Name = "Country")]
        public List<string> AuthorCountryId { get; set; } = new List<string>();
        [Display(Name = "State")]
        public List<string> AuthorStateId { get; set; } = new List<string>();

        [Display(Name = "OSTI ID [Exact]")]
        public string OstiId { get; set; }
        [Display(Name = "Subject")]
        public int? SubjectId { get; set; }
        [Display(Name = "Keyword [Like]")]
        public string Keyword { get; set; }

        public bool SearchContact { get; set; }
        public bool SearchAuthor { get; set; }
        public bool SearchFunding { get; set; }
        #region Funding
        [Display(Name = "Fiscal Year")]
        [StringLength(4)]
        public string FundingYear { get; set; }
        [Display(Name = "Funding Source")]
        public int? FundingTypeId { get; set; }
        [Display(Name = "Contract Number")]
        [StringLength(250)]
        public string ContractNumber { get; set; }
        [Display(Name = "Funding Org")]
        public List<string> FundingOrg { get; set; } = new List<string>();
        public SelectList OrgList => new SelectList(Config.EmployeeManager.GetAllOrganizations().OrderBy(x => x.OrgId).ToList(), "OrgId", "DisplayName");
        [Display(Name = "Grant Number [Like]")]
        [StringLength(250)]
        public string GrantNumber { get; set; }
        [Display(Name = "DOE Program")]
        public int? DoeFundingCategoryId { get; set; }
        [Display(Name = "Milestone Tracking Number")]
        public string MilestoneTrackingNumber { get; set; }
        [Display(Name = "Tracking Number [Like]")]
        [StringLength(250)]
        public string TrackingNumber { get; set; }
        [Display(Name = "Project Number [Like]")]
        [StringLength(250)]
        public string ProjectNumber { get; set; }
        [Display(Name = "Principal Investigator")]
        [StringLength(6)]
        public string PrincipalInvEmployeeId { get; set; }
        [Display(Name = "SPP Category")]
        public int? SppCategoryId { get; set; }
        [Display(Name = "Approved")]
        public bool SppApproved { get; set; }
        [Display(Name = "Not SPP Approved Reason [Like]")]
        public string ApproveNoReason { get; set; }
        [Display(Name = "Federal Agency")]
        public int? FederalAgencyId { get; set; }
        [Display(Name = "Description [Like]")]
        public string OtherDescription { get; set; }
        [Display(Name = "Country")]
        public int? SppCountryId { get; set; }
        [Display(Name = "Additional Info [Like]")]
        public string SppAdditionalInfo { get; set; }


        #endregion
        // Publisher
        public bool SearchPublisher { get; set; }
        [Display(Name = "Name [Like]")]
        public string PublisherName { get; set; }
        [Display(Name = "City [Like]")]
        public string PublisherCity { get; set; }
        [Display(Name = "State [Like]")]
        public string PublisherState { get; set; }
        [Display(Name = "Country")]
        public string PublisherCountry { get; set; }
        // Conference
        public bool SearchConference { get; set; }
        [Display(Name = "Name [Like]")]
        public string ConferenceName { get; set; }
        [Display(Name = "Location [Like]")]
        public string ConferenceLocation { get; set; }
        [Display(Name = "Start Date [Greater Than]")]
        public DateTime? ConferenceStart { get; set; }
        [Display(Name = "End Date [Less Than]")]
        public DateTime? ConferenceEnd { get; set; }
        // Journal
        public bool SearchJournal { get; set; }
        [Display(Name = "Name [Like]")]
        public string JournalName { get; set; }
        [Display(Name = "Volume [Like]")]
        public string JournalVolume { get; set; }
        [Display(Name = "Issue [Like]")]
        public string JournalIssue { get; set; }
        [Display(Name = "Serial [Like]")]
        public string JournalSerial { get; set; }
        [Display(Name = "DOI [Like]")]
        public string JournalDoi { get; set; }
        #endregion

        #region Constructor
        public ReportModel() {
            StartTime = DateTime.Today.AddDays(-30);
            EndTime = DateTime.Today.AddDays(1).AddSeconds(-1);
        }
        #endregion

        #region Public Functions
        public List<SortMainObject> ExecuteReport()
        {
            List<SortMainObject> retList = new List<SortMainObject>();

            //so here we do our sp call 
            string conString = Business.Config.SortConnectionString;


            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_SearchSorts", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@DateOn", System.Data.SqlDbType.Int).Value = (int)SearchDateType;
                cmd.Parameters.Add("@StartTime", System.Data.SqlDbType.DateTime).Value = StartTime;
                cmd.Parameters.Add("@EndTime", System.Data.SqlDbType.DateTime).Value = EndTime;
                cmd.Parameters.Add("@SearchContact", System.Data.SqlDbType.Bit).Value = SearchContact;
                cmd.Parameters.Add("@SearchAuthor", System.Data.SqlDbType.Bit).Value = SearchAuthor;
                cmd.Parameters.Add("@SearchFunding", System.Data.SqlDbType.Bit).Value = SearchFunding;
                cmd.Parameters.Add("@Status", System.Data.SqlDbType.VarChar, 25).Value = string.IsNullOrWhiteSpace(Status) ? (object)DBNull.Value : Status;
                cmd.Parameters.Add("@AccessLimitation", System.Data.SqlDbType.VarChar, 50).Value = string.IsNullOrWhiteSpace(AccessLimitation) ? (object)DBNull.Value : AccessLimitation;
                cmd.Parameters.Add("@Country", System.Data.SqlDbType.VarChar, 100).Value = string.IsNullOrWhiteSpace(Country) ? (object)DBNull.Value : Country;
                cmd.Parameters.Add("@Language", System.Data.SqlDbType.VarChar, 100).Value = string.IsNullOrWhiteSpace(Language) ? (object)DBNull.Value : Language;
                cmd.Parameters.Add("@PublishTitle", System.Data.SqlDbType.VarChar, 1024).Value = string.IsNullOrWhiteSpace(PublishTitle) ? (object)DBNull.Value : PublishTitle;
                cmd.Parameters.Add("@OwnerEmployeeId", System.Data.SqlDbType.VarChar, 6).Value = string.IsNullOrWhiteSpace(Owner) ? (object)DBNull.Value : Owner;
                cmd.Parameters.Add("@ReportNumbers", System.Data.SqlDbType.VarChar, 1024).Value = string.IsNullOrWhiteSpace(ReportNumbers) ? (object)DBNull.Value : ReportNumbers;
                cmd.Parameters.Add("@OstiId", System.Data.SqlDbType.VarChar, 250).Value = string.IsNullOrWhiteSpace(OstiId) ? (object)DBNull.Value : OstiId;
                cmd.Parameters.Add("@Subject", System.Data.SqlDbType.VarChar, 1024).Value = SubjectId.HasValue ? MemoryCache.GetSubjectCategory(SubjectId.Value).FullSubject : (object)DBNull.Value;
                cmd.Parameters.Add("@Keyword", System.Data.SqlDbType.VarChar, 1024).Value = string.IsNullOrWhiteSpace(Keyword) ? (object)DBNull.Value : Keyword;

                if (!string.IsNullOrWhiteSpace(ProductType))
                {
                    cmd.Parameters.Add("@ProductType", System.Data.SqlDbType.VarChar, 25).Value = ProductType;
                    switch (ProductType)
                    {
                        case "BookMonograph":
                            if (SearchPublisher)
                            {
                                cmd.Parameters.Add("@SearchPublisher", System.Data.SqlDbType.Bit).Value = SearchPublisher;
                                cmd.Parameters.Add("@PublisherName", System.Data.SqlDbType.VarChar, 1024).Value = string.IsNullOrWhiteSpace(PublisherName) ? (object)DBNull.Value : PublisherName;
                                cmd.Parameters.Add("@PublisherCity", System.Data.SqlDbType.VarChar, 255).Value = string.IsNullOrWhiteSpace(PublisherCity) ? (object)DBNull.Value : PublisherCity;
                                cmd.Parameters.Add("@PublisherState", System.Data.SqlDbType.VarChar, 100).Value = string.IsNullOrWhiteSpace(PublisherState) ? (object)DBNull.Value : PublisherState;
                                cmd.Parameters.Add("@PublisherCountry", System.Data.SqlDbType.VarChar, 100).Value = string.IsNullOrWhiteSpace(PublisherCountry) ? (object)DBNull.Value : PublisherCountry;
                            }
                            break;
                        case "ConfEvent":
                            if (SearchConference)
                            {
                                cmd.Parameters.Add("@SearchConference", System.Data.SqlDbType.Bit).Value = SearchConference;
                                cmd.Parameters.Add("@ConferenceName", System.Data.SqlDbType.VarChar, 1024).Value = string.IsNullOrWhiteSpace(ConferenceName) ? (object)DBNull.Value : ConferenceName;
                                cmd.Parameters.Add("@ConferenceLocation", System.Data.SqlDbType.VarChar, 250).Value = string.IsNullOrWhiteSpace(ConferenceLocation) ? (object)DBNull.Value : ConferenceLocation;
                                cmd.Parameters.Add("@ConferenceStart", System.Data.SqlDbType.DateTime).Value = ConferenceStart ?? (object)DBNull.Value;
                                cmd.Parameters.Add("@ConferenceEnd", System.Data.SqlDbType.DateTime).Value = ConferenceEnd ?? (object)DBNull.Value;
                            }
                            break;
                        case "JournalArticle":
                            if (SearchJournal)
                            {
                                cmd.Parameters.Add("@SearchJournal", System.Data.SqlDbType.Bit).Value = SearchJournal;
                                cmd.Parameters.Add("@JournalName", System.Data.SqlDbType.VarChar, 1024).Value = string.IsNullOrWhiteSpace(JournalName) ? (object)DBNull.Value : JournalName;
                                cmd.Parameters.Add("@JournalVolume", System.Data.SqlDbType.VarChar, 100).Value = string.IsNullOrWhiteSpace(JournalVolume) ? (object)DBNull.Value : JournalVolume;
                                cmd.Parameters.Add("@JournalIssue", System.Data.SqlDbType.VarChar, 100).Value = string.IsNullOrWhiteSpace(JournalIssue) ? (object)DBNull.Value : JournalIssue;
                                cmd.Parameters.Add("@JournalSerial", System.Data.SqlDbType.VarChar, 100).Value = string.IsNullOrWhiteSpace(JournalSerial) ? (object)DBNull.Value : JournalSerial;
                                cmd.Parameters.Add("@JournalDoi", System.Data.SqlDbType.VarChar, 1024).Value = string.IsNullOrWhiteSpace(JournalDoi) ? (object)DBNull.Value : JournalDoi;
                            }
                            break;
                        case "Patent":
                            break;
                    }
                }

                if (SearchContact)
                {
                    cmd.Parameters.Add("@ContactEmployeeId", System.Data.SqlDbType.VarChar, 10).Value = string.IsNullOrWhiteSpace(Contact) ? (object)DBNull.Value : Contact;
                    cmd.Parameters.Add("@ContactType", System.Data.SqlDbType.VarChar, 500).Value = string.IsNullOrWhiteSpace(ContactType) ? (object)DBNull.Value : ContactType;
                    cmd.Parameters.Add("@ContactWorkOrg", System.Data.SqlDbType.VarChar, 4000).Value = ContactWorkOrg.Count > 0 ? string.Join(",", ContactWorkOrg) : (object)DBNull.Value;
                }
                if (SearchAuthor)
                {
                    if (!string.IsNullOrEmpty(Author))
                    {
                        cmd.Parameters.Add("@AuthorFName", System.Data.SqlDbType.VarChar, 250).Value = string.IsNullOrWhiteSpace(Author.FirstName()) ? (object) DBNull.Value : Author.FirstName();
                        cmd.Parameters.Add("@AuthorMName", System.Data.SqlDbType.VarChar, 250).Value = string.IsNullOrWhiteSpace(Author.MiddelName()) ? (object) DBNull.Value : Author.MiddelName();
                        cmd.Parameters.Add("@AuthorLName", System.Data.SqlDbType.VarChar, 250).Value = string.IsNullOrWhiteSpace(Author.LastName()) ? (object) DBNull.Value : Author.LastName();
                    }
                    cmd.Parameters.Add("@Affiliation", System.Data.SqlDbType.VarChar, 250).Value = string.IsNullOrWhiteSpace(Affiliation) ? (object)DBNull.Value : Affiliation;
                    cmd.Parameters.Add("@OrcidId", System.Data.SqlDbType.VarChar, 250).Value = string.IsNullOrWhiteSpace(OrcidId) ? (object)DBNull.Value : OrcidId;
                    cmd.Parameters.Add("@AuthorWorkOrg", System.Data.SqlDbType.VarChar, 4000).Value = AuthorWorkOrg.Count > 0 ? string.Join(",", AuthorWorkOrg) : (object)DBNull.Value;
                    cmd.Parameters.Add("@AuthorCountryId", System.Data.SqlDbType.VarChar, 4000).Value = AuthorCountryId.Count > 0 ? string.Join(",", AuthorCountryId) : (object)DBNull.Value;
                    cmd.Parameters.Add("@AuthorStateId", System.Data.SqlDbType.VarChar, 4000).Value = AuthorStateId.Count > 0 ? string.Join(",", AuthorStateId) : (object)DBNull.Value;
                }
                if (SearchFunding)
                {
                    cmd.Parameters.Add("@FiscalYear", System.Data.SqlDbType.VarChar, 4).Value = string.IsNullOrWhiteSpace(FundingYear) ? (object)DBNull.Value : FundingYear;
                    cmd.Parameters.Add("@ContractNumber", System.Data.SqlDbType.VarChar, 250).Value = string.IsNullOrWhiteSpace(ContractNumber) ? (object)DBNull.Value : ContractNumber;
                    cmd.Parameters.Add("@FundingOrg", System.Data.SqlDbType.VarChar, 4000).Value = FundingOrg.Count > 0 ? string.Join(",", FundingOrg) : (object)DBNull.Value;

                    if (FundingTypeId.HasValue)
                    {
                        cmd.Parameters.Add("@FundingTypeId", System.Data.SqlDbType.Int).Value = FundingTypeId.Value;
                        switch (FundingTypeId.Value)
                        {
                            case 1: //DOE
                                cmd.Parameters.Add("@DoeFundingCategoryId", System.Data.SqlDbType.Int).Value = DoeFundingCategoryId ?? (object)DBNull.Value;
                                cmd.Parameters.Add("@MilestoneTrackingNumber", System.Data.SqlDbType.VarChar, 250).Value = string.IsNullOrWhiteSpace(MilestoneTrackingNumber) ? (object)DBNull.Value : MilestoneTrackingNumber;
                                break;
                            case 6: //Grant
                            case 7: //NEUP
                                cmd.Parameters.Add("@GrantNumber", System.Data.SqlDbType.VarChar, 250).Value = string.IsNullOrWhiteSpace(GrantNumber) ? (object)DBNull.Value : GrantNumber;
                                break;
                            case 2: //LDRD
                                cmd.Parameters.Add("@TrackingNumber", System.Data.SqlDbType.VarChar, 250).Value = string.IsNullOrWhiteSpace(TrackingNumber) ? (object)DBNull.Value : TrackingNumber;
                                cmd.Parameters.Add("@ProjectNumber", System.Data.SqlDbType.VarChar, 250).Value = string.IsNullOrWhiteSpace(TrackingNumber) ? (object)DBNull.Value : TrackingNumber;
                                cmd.Parameters.Add("@PrincipalInvEmployeeId", System.Data.SqlDbType.VarChar, 6).Value = string.IsNullOrWhiteSpace(PrincipalInvEmployeeId) ? (object)DBNull.Value : PrincipalInvEmployeeId;
                                break;
                            case 4: //Other
                                cmd.Parameters.Add("@OtherDescription", System.Data.SqlDbType.VarChar, 8000).Value = string.IsNullOrWhiteSpace(OtherDescription) ? (object)DBNull.Value : OtherDescription;
                                break;
                            case 5: //spp
                                cmd.Parameters.Add("@SppApproved", System.Data.SqlDbType.Bit).Value = SppApproved;
                                if (!SppApproved)
                                {
                                    cmd.Parameters.Add("@NotSppReason", System.Data.SqlDbType.VarChar, 8000).Value = string.IsNullOrWhiteSpace(ApproveNoReason) ? (object)DBNull.Value : ApproveNoReason;
                                }
                                if (SppCategoryId.HasValue)
                                {
                                    cmd.Parameters.Add("@SppCategoryId", System.Data.SqlDbType.Int).Value = SppCategoryId.Value;
                                    switch (SppCategoryId.Value)
                                    {
                                        case 1: //Federal
                                            cmd.Parameters.Add("@FederalAgencyId", System.Data.SqlDbType.Int).Value = FederalAgencyId ?? (object)DBNull.Value;
                                            break;
                                        case 6: //Other
                                            cmd.Parameters.Add("@OtherDesc", System.Data.SqlDbType.VarChar, 8000).Value = string.IsNullOrWhiteSpace(OtherDescription) ? (object)DBNull.Value : OtherDescription;
                                            break;
                                        case 7: //Foreign
                                            cmd.Parameters.Add("@ForeignCountry", System.Data.SqlDbType.Int).Value = SppCountryId ?? (object)DBNull.Value;
                                            cmd.Parameters.Add("@ForeignInfo", System.Data.SqlDbType.VarChar, 8000).Value = string.IsNullOrWhiteSpace(SppAdditionalInfo) ? (object)DBNull.Value : SppAdditionalInfo;
                                            cmd.Parameters.Add("@OtherDescription", System.Data.SqlDbType.VarChar, 8000).Value = string.IsNullOrWhiteSpace(OtherDescription) ? (object)DBNull.Value : OtherDescription;
                                            break;
                                    }
                                }
                                break;   
                        }
                    }
                }

                SqlDataReader rdr = cmd.ExecuteReader();
                using (IDbConnection dataConn = Config.Conn)
                {
                    dataConn.Open();
                    while (rdr.Read())
                    {
                        int id = (int) rdr["SortMainId"];
                        retList.Add(SortMainObject.GetSortMain(id, dataConn));
                    }
                }

                rdr.Close();
                con.Close();
            }

            return retList;
        }
        #endregion
    }
}