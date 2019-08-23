using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using Dapper;
using Sort.Interfaces;

namespace Sort.Business
{
    public class SortMainObject : ISort
    {
        #region Properties
        public int? SortMainId { get; set; }

        public int? SharePointId { get; set; } //Also LRS ID
        public string OwnerEmployeeId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string ReportNumber { get; set; }
        public string ContractNumber { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public string ReviewStatus { get; set; }
        public int? ReviewProgress { get; set; }
        public string StiNumber { get; set; }
        public int? Revision { get; set; }
        public bool HasTechWriter { get; set; }
        [Display(Name = "Technical Writer")]
        public string TechWriterEmployeeId { get; set; }
        public bool CoverPageRequired { get; set; }

        #region Required
        [Display(Name = "OSTI ID", ShortName = "osti_id")]
        public string OstiId { get; set; }
        [Display(Name = "OSTI Publish Status", ShortName = "status")]
        public string OstiStatus { get; set; }
        [Display(Name = "OSTI Error Message", ShortName = "status_message")]
        public string OstiStatusMsg { get; set; }
        [Display(Name = "OSTI Publish Time")]
        public DateTime? OstiDate { get; set; }
        [Display(Name = "DOI Number")]
        public string DoiNum { get; set; }

        [Display(Name = "Status", ShortName = "status")]
        public string Status { get; set; }
        [Display(Name = "Artifact Type", ShortName = "product_type")]
        public string ProductType { get; set; }
        
        // Conference Info
        [Display(Name = "Conference Name")]
        public string ConferenceName { get; set; }
        [Display(Name = "Conference Sponsor")]
        public string ConferenceSponsor { get; set; }
        [Display(Name = "Conference Location")]
        public string ConferenceLocation { get; set; }
        [Display(Name = "Conference Start Date")]
        public DateTime? ConferenceBeginDate { get; set; }
        [Display(Name = "Conference End Date")]
        public DateTime? ConferenceEndDate { get; set; }
        // End Conference Info

        // Journal Info
        [Display(Name = "Journal Type", ShortName = "journal_type")]
        public string JournalType { get; set; }
        [Display(Name = "Name", ShortName = "journal_name")]
        public string JournalName { get; set; }
        [Display(Name = "Volume", ShortName = "journal_volume")]
        public string JournalVolume { get; set; }
        [Display(Name = "Issue", ShortName = "journal_issue")]
        public string JournalIssue { get; set; }
        [Display(Name = "Serial", ShortName = "journal_serial_id")]
        public string JournalSerial { get; set; }
        [Display(Name = "Beginning On Page")]
        public int? JournalStartPage { get; set; }
        [Display(Name = "Ending On Page")]
        public int? JournalEndPage { get; set; }
        [Display(Name = "DOI", ShortName = "doi")]
        public string JournalDoi { get; set; }
        // End Journal Info

        // Publisher
        [Display(Name = "Publisher Name")]
        public string PublisherName { get; set; }
        [Display(Name = "City")]
        public string PublisherCity { get; set; }
        [Display(Name = "State")]
        public string PublisherState { get; set; }
        [Display(Name = "Country", Description = "Only if other than U.S.")]
        public string PublisherCountry { get; set; }
        // End Publisher

        [Display(Name = "Patent Assignee", ShortName = "patent_assignee")]
        public string PatentAssignee { get; set; }
        [Display(Name = "STI Number and Rev", ShortName = "title")]
        public string Title { get; set; }
        [Display(Name = "Title", ShortName = "")]
        public string PublishTitle { get; set; }

        [Display(Name = "Authors", ShortName = "authors")]
        public List<AuthorObject> Authors => AuthorObject.GetAuthors(SortMainId ?? 0);
        public string Authorstr => Authors == null ? string.Empty : string.Join("; ", Authors.Select(n => $"{n.FirstName} {n.MiddleName} {n.LastName}"));

        [Display(Name = "Report/Product Number(s)", ShortName = "report_nos")]
        public string ReportNumbers { get; set; }
        [Display(Name = "Publication/Issue Date", ShortName = "publication_date")]
        public DateTime? PublicationDate { get; set; }
        [Display(Name = "Publication Language", ShortName = "language")]
        public string Language { get; set; }
        [Display(Name = "Country of Origin/Publication", ShortName = "country_publication_code")]
        public string Country { get; set; }

        [Display(Name = "Sponsoring Organization(s)", ShortName = "sponsor_org")]
        public List<SortMetaDataObject> SponsoringOrgs => SortMetaDataObject.GetSortMetaDatas(SortMainId ?? 0, MetaDataTypeEnum.SponsoringOrgs);
        public string SponsoringOrgStr => string.Join("; ", SponsoringOrgs.Select(n => n.Data).ToArray());

        [Display(Name = "Access Limitation", ShortName = "access_limitation")]
        public string AccessLimitation { get; set; }

        [Display(ShortName = "released_date")]
        public DateTime? ReleasedDate { get; set; }
        public string UrlInt { get; set; }
        public int? StiSpId { get; set; }

        [Display(Name = "Contacts")]
        public List<ContactObject> Contacts => ContactObject.GetContacts(SortMainId ?? 0);
        [Display(Name = "Funding")]
        public List<FundingObject> Funding => FundingObject.GetFundings(SortMainId ?? 0);

        public bool PublicRelease { get; set; }
        public bool IsFromLrs { get; set; }
        public bool ForceEdms { get; set; }
        public DateTime? OneYearReminderDate { get; set; }
        public bool OneYearReminderSent { get; set; }
        public string RelatedSti { get; set; }
        public DateTime? DelayToDate { get; set; }
        public string DelayReason { get; set; }
        public bool DelayReminderSent { get; set; }
        #endregion

        #region Conditional

        [Display(Name = "OpenNet")]
        public OpenNetObject OpenNetData => OpenNetObject.GetOpenNetData(SortMainId ?? 0);

        [Display(Name = "Protected")]
        public ProtectedDataObject ProtectedData => ProtectedDataObject.GetProtectedDataForSortMain(SortMainId ?? 0);

        [Display(Name = "Exemption Number", ShortName = "exemption_number")]
        public string ExemptionNumber { get; set; }
        [Display(Name = "Access Limitation Release Date", ShortName = "access_limitation_rel_date")]
        public DateTime? AccessReleaseDate { get; set; }
        #endregion

        #region Optional

        [Display(Name = "Keyword(s)", ShortName = "keywords")]
        public List<SortMetaDataObject> KeyWordList => SortMetaDataObject.GetSortMetaDatas(SortMainId ?? 0, MetaDataTypeEnum.Keywords);
        [Display(Name = "Description / Abstract", ShortName = "description"), MaxLength(5000)]
        [StringLength(5000)]
        public string Abstract { get; set; }
        [Display(Name = "Releated Document Information", ShortName = "related_doc_info")]
        public string RelatedDocInfo { get; set; }
        [Display(Name = "Further Information Contact", ShortName = "availability")]
        public string FurtherInfoContact { get; set; }
        [Display(Name = "R&D Project ID(s)", ShortName = "rd_project_ids")]
        public List<SortMetaDataObject> RdProjectId => SortMetaDataObject.GetSortMetaDatas(SortMainId ?? 0, MetaDataTypeEnum.RdProjectId);

        [Display(Name = "Work Proposal Number(s)", ShortName = "work_proposal_number")]
        public List<SortMetaDataObject> WorkProposalNum => SortMetaDataObject.GetSortMetaDatas(SortMainId ?? 0, MetaDataTypeEnum.WorkProposalNum);

        [Display(Name = "Work Authorization Number(s)", ShortName = "work_authorization_number")]
        public List<SortMetaDataObject> WorkAuthNum => SortMetaDataObject.GetSortMetaDatas(SortMainId ?? 0, MetaDataTypeEnum.WorkAuthNum);

        [Display(Name = "Other Identifying Number(s)", ShortName = "other_identifying_nos")]
        public List<SortMetaDataObject> OtherNum => SortMetaDataObject.GetSortMetaDatas(SortMainId ?? 0, MetaDataTypeEnum.OtherNum);

        [Display(Name = "Subject Category Codes", ShortName = "subject_category_code")]
        public List<SortMetaDataObject> SubjectCategories => SortMetaDataObject.GetSortMetaDatas(SortMainId ?? 0, MetaDataTypeEnum.SubjectCategories);

        [Display(Name = "Core Capabilities")]
        public List<SortMetaDataObject> CoreCapabilities => SortMetaDataObject.GetSortMetaDatas(SortMainId ?? 0, MetaDataTypeEnum.CoreCapabilities);

        [Display(Name = "Product Size", ShortName = "product_size")]
        public string ProductSize { get; set; }
        [Display(Name = "Publisher Name and Location", ShortName = "publisher_information")]
        public string PublisherInfo { get; set; }
        [Display(Name = "Contributors (Organizations)", ShortName = "constributor_organizations")]
        public List<SortMetaDataObject> Contributors => SortMetaDataObject.GetSortMetaDatas(SortMainId ?? 0, MetaDataTypeEnum.Contributors);
        #endregion

        #endregion

        #region Extended Properties

        public bool IsExternalType => StiNumber.IndexOf("EXT", StringComparison.InvariantCultureIgnoreCase) > -1;
        public string AbstractWithAccessCheck => UserHasReadAccess() ? Abstract : string.Empty;

        [Display(Name = "Revision")]
        public string RevisionStr => $"{Revision:D3}";
        public string DisplayTitle => $"{StiNumber} Rev:{RevisionStr}";

        public string ReviewProgressStr
        {
            get
            {
                if(ReviewProgress.HasValue)
                {
                    double percent = ReviewProgress.Value;

                    return $"<div class=\"progress\"><div class=\"progress-bar\" role\"progressbar\" aria-valuenow=\"70\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width:{percent:f0}%;\"><span>{percent:f0}%</span></div></div>";
                }
                
                return string.Empty;
            }
        }

        public ProductTypeEnum ProductTypeEnum
        {
            get{return ProductType.ToEnum<ProductTypeEnum>();}
            set { ProductType = value.ToString(); }
        }

        public string ProductTypeDisplayName => string.IsNullOrWhiteSpace(ProductType) ? string.Empty : ProductTypeEnum.GetEnumDisplayName();

        public JournalTypeEnum JournalTypeEnum
        {
            get { return JournalType.ToEnum<JournalTypeEnum>(); }
            set { JournalType = value.ToString(); }
        }

        public string JournalTypeDisplayName => JournalTypeEnum.GetEnumDisplayName();

        public AccessLimitationEnum AccessLimitationEnum
        {
            get { return AccessLimitation.ToEnum<AccessLimitationEnum>(); }
            set { AccessLimitation = value.ToString(); }
        }

        public string AccessLimitationDisplayName => string.IsNullOrWhiteSpace(AccessLimitation) ? string.Empty : AccessLimitationEnum.GetEnumDisplayName();

        public StatusEnum StatusEnum
        {
            get { return Status.ToEnum<StatusEnum>(); }
            set { Status = value.ToString(); }
        }

        public string StatusDisplayName
        {
            get
            {
                if (DelayToDate.HasValue && DelayToDate.Value > DateTime.Now)
                {
                    return "Delayed";
                }

                return StatusEnum.GetEnumDisplayName();
            }
        }

        [Display(Name ="Conference", ShortName = "conference_information")]
        public string ConferenceInfo => $"{ConferenceName}, {ConferenceLocation}, {ConferenceBeginDate:MM/dd/yyyy} - {ConferenceEndDate:MM/dd/yyyy}";

        [Display(Name = "Publisher", ShortName = "publisher_information")]
        public string PublisherInformation => string.IsNullOrWhiteSpace(PublisherName) ? string.Empty : $"{PublisherName} - {PublisherCity}, {PublisherState} {PublisherCountry}";

        [Display(Name = "Journal")]
        public string JournalInformation => $"Type: {JournalTypeEnum.GetEnumDisplayName()}\nName: {JournalName}\nVolume: {JournalVolume}\nIssue: {JournalIssue}\nSerial: {JournalSerial}\nPages: {JournalPageRage}\nDOI:{JournalDoi}";

        [Display(Name = "Official Use")]
        public string OfficialUseInformation => $"Access Limitation Release Date: {AccessReleaseDate:MM/dd/yyyy}\nExemption Number: {ExemptionNumber}";

        [Display(ShortName = "product_size")]
        public string JournalPageRage => $"p. {JournalStartPage} - {JournalEndPage}";

        [Display(ShortName = "keywords")]
        public string Keywords => string.Join("; ", KeyWordList?.Select(n => n.Data));

        [Display(Name = "Reviewer(s)")]
        public List<ReviewObject> Reviewers => ReviewObject.GetReviews(SortMainId.Value);

        public List<MissingData> MissingDatas { get; private set; }

        public string GridButtons
        {
            get
            {
                StringBuilder returnStr = new StringBuilder();
                returnStr.Append("<a href=\"" + UrlHelper.Action("Index", "Artifact", new { id = SortMainId }) + "\" title=\"View\" class=\"btn btn-xs btn-primary\" style=\"margin-left: 3px;\"><i class=\"fa fa-list\"></i></a>");
                if (UserHasWriteAccess() && (StatusEnum == StatusEnum.Imported || 
                    StatusEnum == StatusEnum.Complete ||
                    (StatusEnum == StatusEnum.Published && UserObject.CurrentUser.IsInAnyRole("Admin,ReleaseOfficial"))))
                {
                    returnStr.Append("<a href=\"" + UrlHelper.Action("Edit", "Artifact", new { id = SortMainId }) + "\" title=\"Edit\" class=\"btn btn-xs btn-success\" style=\"margin-left: 3px;\"><i class=\"fa fa-pencil\"></i></a>");
                }
                if (CanUpdateDueDate)
                {
                    returnStr.Append("<a href=\"" + UrlHelper.Action("SetDueDate", "Artifact", new { id = SortMainId }) + "\" title=\"Update Due Date\" class=\"btn btn-xs btn-warning\" style=\"margin-left: 3px;\"><i class=\"fa fa-clock-o\"></i></a>");
                }
                if ((StatusEnum == StatusEnum.Complete || StatusEnum == StatusEnum.Published) && UserObject.CurrentUser.IsInAnyRole("Admin,ReleaseOfficial"))
                {
                    if (SortAttachmentObject.GetOstiAttachment(SortMainId ?? 0) != null)
                    {
                        returnStr.Append("<a href=\"" + UrlHelper.Action("GetOstiDocument", "Edms", new { id = SortMainId }) + "\" title=\"Download OSTI Document\" class=\"btn btn-xs btn-default\" style=\"margin-left: 3px;\"><i class=\"fa fa-download\"></i></a>");
                        if (StatusEnum != StatusEnum.Published)
                        {
                            returnStr.Append("<a href=\"" + UrlHelper.Action("DeleteOstiDocument", "Artifact", new { id = SortMainId }) + "\" title=\"Delete OSTI Document\" class=\"btn btn-xs btn-danger\" style=\"margin-left: 3px;\"><i class=\"fa fa-trash\"></i></a>");
                        }
                    }
                    else
                    {
                        if (CoverPageRequired)
                        {
                            returnStr.Append("<a href=\"" + UrlHelper.Action("GetCoverPage", "Edms", new {id = SortMainId}) + "\" title=\"Download Cover Page\" class=\"btn btn-xs btn-default\" style=\"margin-left: 3px;\"><i class=\"fa fa-file-text-o\"></i></a>");
                        }
                        returnStr.Append("<a href=\"" + UrlHelper.Action("CreateOstiDocument", "Artifact", new { id = SortMainId }) + "\" title=\"Generate OSTI Document through Adlib\" class=\"btn btn-xs btn-default\" style=\"margin-left: 3px;\"><i class=\"fa fa-cogs\"></i></a>");
                    }
                }
                
                return returnStr.ToString();
            }
        }

        public bool CanUpdateDueDate => (StatusEnum == StatusEnum.Imported || (StatusEnum == StatusEnum.Complete && ReviewStatus != "Approved")) && DueDate < DateTime.Now && UserObject.CurrentUser.IsInAnyRole("Admin,ReleaseOfficial");

        public string PublicationDateTxt => $"{PublicationDate:MM/dd/yyyy}";

        public string TitleStr
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(StiNumber) && Revision.HasValue)
                {
                    return $"{StiNumber}-Rev{Revision:D3}";
                }

                return Title;
            }
        }

        public string OstiTitleStr
        {
            get
            {
                if (Title.IndexOf("HST", StringComparison.OrdinalIgnoreCase) > -1 && !string.IsNullOrWhiteSpace(RelatedSti))
                {
                    return RelatedSti;
                }

                return TitleStr;
            }
        }

        public string OwnerWorkOrg => EmployeeCache.GetEmployee(OwnerEmployeeId, true)?.WorkOrginization;

        public string DueDateStr => $"{DueDate:MM/dd/yyyy}";

        public string DelayToDateStr => $"{DelayToDate:MM/dd/yyyy}";
        #endregion

        #region Private Properties
        private UrlHelper _urlHelper;
        private UrlHelper UrlHelper
        {
            get
            {
                if (_urlHelper == null)
                {
                    _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                }

                return _urlHelper;
            }
        }
        #endregion

        #region Constructor

        public SortMainObject(){}
        #endregion

        #region Repository

        private static ISortMainRepository repo => new SortMainRepository();

        #endregion

        #region Static Methods

        public static SortMainObject GetSortMain(int sortMainId, IDbConnection conn = null)
        {
            try
            {
                var cacheKey = $"SortMainObj_{sortMainId}";
                var cache = HttpContext.Current.Cache;
                var mob = (SortMainObject) cache[cacheKey];
                if (mob == null)
                {
                    mob = repo.GetSortMain(sortMainId, conn);
                    if (mob != null)
                    {
                        cache.Insert(cacheKey, mob, null, DateTime.UtcNow.AddMinutes(Config.ApplicationMode == ApplicationMode.Production ? 5 : 1), Cache.NoSlidingExpiration);
                    }
                }

                return mob;
            }
            catch { }

            return repo.GetSortMain(sortMainId, conn);
        } 
        public static SortMainObject GetSortMainForSharpointId(int sharepointId) => repo.GetSortMainForSharpointId(sharepointId);
        public static SortMainObject GetSortMainForLrsId(int lrsId) => repo.GetSortMainForLrsId(lrsId);

        public static SortMainObject GetSortMainForStiNumber(string stiNumber, int revision) => repo.GetSortMainForStiNumber(stiNumber, revision);

        public static List<SortMainObject> GetSortForUser(string employeeId) => repo.GetSortForUser(employeeId);
        public static int GetSortForUserCount(string employeeId) => repo.GetSortForUserCount(employeeId);


        public static List<SortMainObject> GetSortForUserOrg(string employeeId, string orgOption) => repo.GetSortForUserOrg(employeeId, orgOption);
        public static int GetSortForOrgCount(string employeeId, string orgOption) => repo.GetSortForOrgCount(employeeId, orgOption);


        public static List<SortMainObject> GetAllSorts(ViewModeOptionEnum viewMode) => repo.GetAllSorts(viewMode);
        public static int GetAllSortsCount(ViewModeOptionEnum viewMode) => repo.GetAllSortsCount(viewMode);

        /// <summary>
        /// Check to see if the status should change due to changing data.
        /// </summary>
        /// <param name="sortMainId">Sort Main Id</param>
        /// <param name="changePublishedState">Check to see if we are in a published state, if so change state to complete since some data was changes</param>
        public static void CheckStatusUpdate(int sortMainId, bool changePublishedState = false)
        {
            var sort = GetSortMain(sortMainId);
            if (sort != null)
            {
                if (changePublishedState)
                {
                    if (sort.StatusEnum == StatusEnum.Published)
                    {
                        sort.StatusEnum = StatusEnum.Complete;
                        // Since this is no longer published, we need to remove the OSTI Upload file
                        // That way it can get regenerated with the correct cover letter.
                        SortAttachmentObject.GetOstiAttachment(sortMainId)?.PerminateDelete();
                    }
                }
                sort.Save(); //Save will check the status changes
            }
        }

        public static bool CheckUserHasWriteAccess(int sortMainId)
        {
            if (UserObject.CurrentUser.IsInAnyRole("Admin,ReleaseOfficial")) return true;

            return GetSortMain(sortMainId)?.UserHasWriteAccess() ?? false;
        }

        public static bool CheckUserHasReadAccess(int sortMainId)
        {
            if (UserObject.CurrentUser.IsInAnyRole("Admin,ReleaseOfficial,ReadAll")) return true;

            return GetSortMain(sortMainId)?.UserHasReadAccess() ?? false;
        }

        public static List<CountData> GetCounts() => repo.GetCounts();

        public static List<SortMainObject> GetOneYearReminders() => repo.GetOneYearReminders();

        public static List<SortMainObject> GetDeleyedReminders() => repo.GetDeleyedReminders();

        public static void UpdateReminderDate(int sortMainId, DateTime reminderDate)
        {
            repo.UpdateReminderDate(sortMainId, reminderDate);
            ClearMainFromCache(sortMainId);
        }

        public static void UpdateDelayToDate(int sortMainId, DateTime delayToDate, string delayReason)
        {
            repo.UpdateDelayToDate(sortMainId, delayToDate, delayReason);
            ClearMainFromCache(sortMainId);
        }

        public static void ResetDelayToDate(int sortMainId)
        {
            repo.ResetDelayToDate(sortMainId);
            ClearMainFromCache(sortMainId);
        }

        public static List<SortMainObject> GetSortNeedingAdlibDocument() => repo.GetSortNeedingAdlibDocument();

        public static void UpdateCoverPageRequired(int sortMainId, bool isRequired)
        {
            repo.UpdateCoverPageRequired(sortMainId, isRequired);
            SortAttachmentObject.GetOstiAttachment(sortMainId)?.PerminateDelete();
            ClearMainFromCache(sortMainId);
        }

        public static List<SortMainObject> SearchForUser(string searchData, bool includeTitle, bool includeAbstract, bool includeReviewer) => repo.SearchForUser(searchData, includeTitle, includeAbstract, includeReviewer);

        public static void SetForceEdms(int sortMainId)
        {
            repo.SetForceEdms(sortMainId);
            ClearMainFromCache(sortMainId);
        }

        public static List<SortMainObject> GetNeedReminder() => repo.GetNeedReminder();
        #endregion

        #region Public Functions
        public void Save()
        {
            if (string.IsNullOrWhiteSpace(OwnerEmployeeId) && (!string.IsNullOrWhiteSpace(OwnerName) || !string.IsNullOrWhiteSpace(OwnerEmail)))
            {
                var owner = Config.EmployeeManager.GetEmployeeByName(OwnerName, true);
                if (owner != null)
                {
                    OwnerEmployeeId = owner.EmployeeId;
                    OwnerEmail = owner.Email;
                }
            }

            if (StatusEnum == StatusEnum.Published)
            {
                // If we are published and we are saving, check to see if any data changed.
                // If it did, change the status to complete
                CheckPublishedForChangedData();
            }

            // only switch the state if we are imported or complete.  Otherwise the state stays the same.
            if (StatusEnum == StatusEnum.Imported || StatusEnum == StatusEnum.Complete)
            {
                StatusEnum = CheckForMissingData() ? StatusEnum.Imported : StatusEnum.Complete;
            }
            
            repo.SaveSortMain(this);
            ClearMainFromCache(SortMainId.Value);
        }

        public bool UserHasWriteAccess()
        {
            var user = UserObject.CurrentUser;

            return user.EmployeeId == OwnerEmployeeId ||
                   user.IsInAnyRole("Admin,ReleaseOfficial") ||
                   Authors.Exists(n => n.EmployeeId == user.EmployeeId) ||
                   Contacts.Exists(n => n.EmployeeId == user.EmployeeId);
        }

        public bool UserHasReadAccess(IDbConnection conn = null)
        {
            // Test this one piece at a time to try and make it faster

            var user = UserObject.CurrentUser;

            //Owner or Admin
            if (user.EmployeeId == OwnerEmployeeId || user.IsInAnyRole("Admin,ReleaseOfficial,ReadAll")) return true;

            //Author
            var authors = AuthorObject.GetAuthors(SortMainId.Value, conn) ?? new List<AuthorObject>();
            if (authors.Exists(n => n.EmployeeId == user.EmployeeId)) return true;

            //Contact
            var contacts = ContactObject.GetContacts(SortMainId.Value, conn) ?? new List<ContactObject>();
            if (contacts.Exists(n => n.EmployeeId == user.EmployeeId)) return true;

            //Org Manager
            if (user.IsInAnyRole("OrgManager"))
            {
                var userOrgs = user.ManagerOrgs;
                if (userOrgs.Exists(m => m.Equals(OwnerWorkOrg, StringComparison.InvariantCultureIgnoreCase)) ||
                    authors.Exists(n => userOrgs.Exists(m => m.Equals(n.WorkOrg, StringComparison.InvariantCultureIgnoreCase))) ||
                    contacts.Exists(n => userOrgs.Exists(m => m.Equals(n.WorkOrg, StringComparison.InvariantCultureIgnoreCase)))) return true;
            }

            return false;
        }

        public void Delete()
        {
            repo.DeleteSortMain(this);
            ClearMainFromCache(SortMainId.Value);
        }

        public bool UploadToOstiServer(bool publicRelease)
        {
            if (SortMainId.HasValue)
            {
                PublicRelease = publicRelease;
                if (SortAttachmentObject.GetOstiAttachment(SortMainId.Value) == null && SortAttachmentObject.GetFinalDocAttachment(SortMainId.Value) != null)
                {
                    // Need to generate the OSTI document.
                    bool success = false;
                    byte[] file = Config.DigitalLibraryManager.GenerateExportFile(SortMainId.Value, CoverPageRequired, ref success);
                    if (success)
                    {
                        SortAttachmentObject.AddOstiAttachment(SortMainId.Value, $"Sort_{SortMainId}.pdf", UserObject.CurrentUser.EmployeeId, file.Length, file);
                    }
                    else
                    {
                        OstiStatusMsg = "FAILED - Generating the OSTI File with AdLib failed. Please try again.";
                        OstiStatus = "FAILED";
                        return false;
                    }
                }
                if (PublicRelease)
                {
                    var file = SortAttachmentObject.GetOstiAttachment(SortMainId.Value);
                    if (file != null)
                    {
                        UrlInt = file.FileName;
                        try
                        {
                            StiSpId = Config.DigitalLibraryManager.UploadFile(file.FileName, (MemoryStream)file.Contents);
                        }
                        catch (Exception ex)
                        {
                            ErrorLogObject.LogError("SortMainObject::UploadToOstiServer", ex);
                            OstiStatusMsg = $"Error occurred when uploading Final Document / Accepted Manuscript to INL Digital Library. : {ex.Message}";
                            OstiStatus = "FAILED";
                            return false;
                        }
                    }
                    else
                    {
                        OstiStatusMsg = "FAILED - Unable to find the Final Document / Accepted Manuscript attachment.";
                        OstiStatus = "FAILED";
                        return false;
                    }
                }
                return OstiUploadObject.UploadToOsti(this);
            }
            else
            {
                OstiStatusMsg = "Error: Save Sort Main Object first.";
                OstiStatus = "FAILED";
                return false;
            }
            
        }

        public string GetXml()
        {
            if (!ReleasedDate.HasValue)
            {
                ReleasedDate = DateTime.Now;
            }

            string xmlForOsti = string.Empty;
            using (var sw = new StringWriter())
            using (var xw = XmlWriter.Create(sw))
            {
                this.Xml.WriteTo(xw);
                xw.Flush();
                xmlForOsti = sw.GetStringBuilder().ToString();
            }

            return xmlForOsti;
        }

        public bool CheckForMissingData()
        {
            MissingDatas = new List<MissingData>();
            try
            {
                if (StatusEnum != StatusEnum.Cancelled)
                {
                    if (string.IsNullOrWhiteSpace(PublishTitle)) MissingDatas.Add(new MissingData("Main Info", "Title Missing"));
                    if (!PublicationDate.HasValue) MissingDatas.Add(new MissingData("Main Info", "Publication / Issue Date Missing"));
                    if (string.IsNullOrWhiteSpace(Country)) MissingDatas.Add(new MissingData("Main Info", "Country of Origin / Publication Missing"));
                    if (string.IsNullOrWhiteSpace(Language)) MissingDatas.Add(new MissingData("Main Info", "Publication Language Missing"));
                    if (string.IsNullOrWhiteSpace(Abstract)) MissingDatas.Add(new MissingData("Abstract", "Description / Abstract Missing"));
                    if (string.IsNullOrWhiteSpace(ProductType))
                    {
                        MissingDatas.Add(new MissingData("Main Info", "Artifact Type Missing"));
                    }
                    else
                    {
                        switch (ProductTypeEnum)
                        {
                            case ProductTypeEnum.BookMonograph:
                                if (string.IsNullOrWhiteSpace(PublisherName)) MissingDatas.Add(new MissingData("Publisher Info", "Publisher Name Missing"));
                                if (string.IsNullOrWhiteSpace(PublisherCity)) MissingDatas.Add(new MissingData("Publisher Info", "City Missing"));
                                if (string.IsNullOrWhiteSpace(PublisherState)) MissingDatas.Add(new MissingData("Publisher Info", "State Missing"));
                                if (string.IsNullOrWhiteSpace(PublisherCountry)) MissingDatas.Add(new MissingData("Publisher Info", "Country Missing"));
                                break;
                            case ProductTypeEnum.ConfEvent:
                                if (string.IsNullOrWhiteSpace(ConferenceName)) MissingDatas.Add(new MissingData("Conference Info", "Conference Name Missing"));
                                if (string.IsNullOrWhiteSpace(ConferenceLocation)) MissingDatas.Add(new MissingData("Conference Info", "Conference Location Missing"));
                                if (!ConferenceBeginDate.HasValue) MissingDatas.Add(new MissingData("Conference Info", "Conference Start Date Missing"));
                                if (!ConferenceEndDate.HasValue) MissingDatas.Add(new MissingData("Conference Info", "Conference End Date Missing"));
                                break;
                            case ProductTypeEnum.JournalArticle:
                                if (string.IsNullOrWhiteSpace(JournalType)) MissingDatas.Add(new MissingData("Journal Info", "Journal Type Missing"));
                                if (string.IsNullOrWhiteSpace(JournalName)) MissingDatas.Add(new MissingData("Journal Info", "Name Missing"));
                                if (string.IsNullOrWhiteSpace(JournalVolume)) MissingDatas.Add(new MissingData("Journal Info", "Volume Missing"));
                                if (string.IsNullOrWhiteSpace(JournalIssue)) MissingDatas.Add(new MissingData("Journal Info", "Issue Missing"));
                                if (string.IsNullOrWhiteSpace(JournalSerial)) MissingDatas.Add(new MissingData("Journal Info", "Serial Missing"));
                                if (!JournalStartPage.HasValue) MissingDatas.Add(new MissingData("Journal Info", "Beginning On Page Missing"));
                                if (!JournalEndPage.HasValue) MissingDatas.Add(new MissingData("Journal Info", "Ending On Page Missing"));
                                break;
                            case ProductTypeEnum.Patent:
                                if (string.IsNullOrWhiteSpace(PatentAssignee)) MissingDatas.Add(new MissingData("Pattent Info", "Patent Assignee Missing"));
                                break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(AccessLimitation))
                    {
                        MissingDatas.Add(new MissingData("Main Info", "Access Limitation Missing"));
                    }
                    else
                    {
                        switch (AccessLimitationEnum)
                        {
                            case AccessLimitationEnum.OpenNet:
                                var on = OpenNetData;
                                if (on == null)
                                {
                                    MissingDatas.Add(new MissingData("OpenNet Data", "All Data Missing"));
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(on.AccessNumber)) MissingDatas.Add(new MissingData("OpenNet Data", "Accession Number Missing"));
                                    if (string.IsNullOrWhiteSpace(on.DeclassificationStatus)) MissingDatas.Add(new MissingData("OpenNet Data", "Declassification Status Missing"));
                                    if (!on.DeclassificationDate.HasValue && (on.DeclassStatusEnum == DeclassStatusEnum.Declassified || on.DeclassStatusEnum == DeclassStatusEnum.Sanitized)) MissingDatas.Add(new MissingData("OpenNet Data", "Declassification Date Missing"));
                                }
                                break;
                            case AccessLimitationEnum.ProgramDeterminedOfficialUseOnly:
                                if (!AccessReleaseDate.HasValue) MissingDatas.Add(new MissingData("Official Use Data", "Access Limitation Release Date Missing"));
                                if (string.IsNullOrWhiteSpace(ExemptionNumber)) MissingDatas.Add(new MissingData("Official Use Data", "Exemption Number Missing"));
                                break;
                            case AccessLimitationEnum.ProtectedData:
                                var pd = ProtectedData;
                                if (pd == null)
                                {
                                    MissingDatas.Add(new MissingData("Protected Data", "All Data Missing"));
                                }
                                else
                                {
                                    if (!pd.Crada)
                                    {
                                        if (string.IsNullOrWhiteSpace(pd.Description)) MissingDatas.Add(new MissingData("Protected Data", "Not CRADA Description Missing"));
                                    }
                                    if (!pd.ReleaseDate.HasValue) MissingDatas.Add(new MissingData("Protected Data", "Access Limitation Release Date Missing"));
                                    if (string.IsNullOrWhiteSpace(pd.ExemptNumber)) MissingDatas.Add(new MissingData("Protected Data", "Exemption Number Missing"));
                                }
                                break;
                        }
                    }
                    if (Contacts == null || Contacts.Count(n => n.ContactType.Equals("POC", StringComparison.OrdinalIgnoreCase)) == 0) MissingDatas.Add(new MissingData("Contact(s)", "POC Contact Missing"));
                    if (Contacts != null && Contacts.Exists(n => !n.IsValid)) MissingDatas.Add(new MissingData("Contact(s)", "Contact with Invalid data exists"));
                    if (Authors == null || Authors.Count(n => n.IsPrimary) == 0) MissingDatas.Add(new MissingData("Author(s)", "Primary Author Missing"));
                    if (Authors != null && Authors.Exists(n => !n.IsValid)) MissingDatas.Add(new MissingData("Author(s)", "Author with Invalid data exists"));
                    if (Funding == null || Funding.Count == 0) MissingDatas.Add(new MissingData("Funding", "Funding Missing"));
                    if (Funding != null && Funding.Exists(n => !n.IsValid)) MissingDatas.Add(new MissingData("Funding", "Funding with Invalid data exists"));

                    if (SortMainId.HasValue)
                    {
                        var sub = SortMetaDataObject.GetSortMetaDatas(SortMainId.Value, MetaDataTypeEnum.SubjectCategories);
                        if (sub == null || sub.Count == 0) MissingDatas.Add(new MissingData("Subject", "Subject Missing"));

                        var orgs = SortMetaDataObject.GetSortMetaDatas(SortMainId.Value, MetaDataTypeEnum.SponsoringOrgs);
                        if (orgs == null || orgs.Count == 0) MissingDatas.Add(new MissingData("Sponsoring Org(s)", "Sponsor Missing"));

                        var keys = SortMetaDataObject.GetSortMetaDatas(SortMainId.Value, MetaDataTypeEnum.Keywords);
                        if (keys == null || keys.Count == 0) MissingDatas.Add(new MissingData("Keyword(s)", "Keyword Missing"));

                        var att = SortAttachmentObject.GetSortAttachments(SortMainId.Value)?.FirstOrDefault(n => n.AttachmentTypeEnum == AttachmentTypeEnum.FinalDoc);
                        if (att == null) MissingDatas.Add(new MissingData("Attachment(s)", "Final Document / Accepted Manuscript is Missing"));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("SortMainObject::CheckForMissingData", ex);
            }

            return MissingDatas.Count > 0;
        }

        #endregion

        #region Private Functions
        private XmlDocument Xml
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                var rec = AddElement(AddElement(doc, "records"), "record");
                
                #region Required
                // R1, R2
                if (!string.IsNullOrWhiteSpace(OstiId))
                {
                    AddElement(rec, Extensions.GetPropertyShortName(() => OstiId), OstiId);
                    AddElement(rec, "revprod").SetAttribute(Extensions.GetPropertyShortName(() => OstiId), OstiId);
                }
                else
                {
                    AddElement(rec, "new", "");
                }

                // R3
                AddElement(rec, Extensions.GetPropertyShortName(() => Config.SiteCode), Config.SiteCode);
                
                // R4
                AddElement(rec, Extensions.GetPropertyShortName(() => ProductType), ProductTypeEnum.GetEnumShortName());
                switch (ProductTypeEnum)
                {
                    case ProductTypeEnum.AccomplishmentReport:
                    case ProductTypeEnum.FactSheet:
                    case ProductTypeEnum.ProgramDocument:
                    case ProductTypeEnum.TechReport:
                    case ProductTypeEnum.ThesisDissertation:
                        break;
                    case ProductTypeEnum.BookMonograph:
                        AddElement(rec, Extensions.GetPropertyShortName(() => PublisherInformation), PublisherInformation);
                        break;
                    case ProductTypeEnum.ConfEvent:
                        AddElement(rec, Extensions.GetPropertyShortName(() => ConferenceInfo), ConferenceInfo);
                        break;
                    case ProductTypeEnum.JournalArticle:
                        AddElement(rec, Extensions.GetPropertyShortName(() => JournalType), JournalTypeEnum.GetEnumShortName());
                        AddElement(rec, Extensions.GetPropertyShortName(() => JournalName), JournalName);
                        AddElement(rec, Extensions.GetPropertyShortName(() => JournalVolume), JournalVolume);
                        AddElement(rec, Extensions.GetPropertyShortName(() => JournalIssue), JournalIssue);
                        AddElement(rec, Extensions.GetPropertyShortName(() => JournalSerial), JournalSerial);
                        AddElement(rec, Extensions.GetPropertyShortName(() => JournalPageRage), JournalPageRage);
                        AddElement(rec, Extensions.GetPropertyShortName(() => JournalDoi), JournalDoi);
                        break;
                    case ProductTypeEnum.Patent:
                        AddElement(rec, Extensions.GetPropertyShortName(() => PatentAssignee), PatentAssignee);
                        break;
                }
                
                // R5
                AddElement(rec, Extensions.GetPropertyShortName(() => Title), PublishTitle);

                // R6
                if (Authors != null && Authors.Count > 0)
                {
                    // R6.1
                    var authors = AddElement(rec, Extensions.GetPropertyShortName(() => Authors));
                    foreach (var a in Authors)
                    {
                        var authorDetail = AddElement(authors, "authors_detail");
                        AddElement(authorDetail, Extensions.GetPropertyShortName(() => a.FirstName), a.FirstName);
                        AddElement(authorDetail, Extensions.GetPropertyShortName(() => a.MiddleName), a.MiddleName);
                        AddElement(authorDetail, Extensions.GetPropertyShortName(() => a.LastName), a.LastName);
                        AddElement(authorDetail, Extensions.GetPropertyShortName(() => a.Affiliation), a.Affiliation);
                        AddElement(authorDetail, Extensions.GetPropertyShortName(() => a.Email), a.Email);
                        AddElement(authorDetail, Extensions.GetPropertyShortName(() => a.OrcidId), a.OrcidId);
                    }
                }
                else
                {
                    // R6.2
                    AddElement(rec, "author");
                }

                // R7
                AddElement(rec, Extensions.GetPropertyShortName(() => ReportNumbers), OstiTitleStr);

                // R8
                AddElement(rec, Extensions.GetPropertyShortName(() => Config.DoeContractNumber), Config.DoeContractNumber);
                
                // R9
                AddElement(rec, Extensions.GetPropertyShortName(() => Config.OriginatingResearchOrg), Config.OriginatingResearchOrg);
                
                // R10
                AddElement(rec, Extensions.GetPropertyShortName(() => PublicationDate), PublicationDate.HasValue ? PublicationDate.Value.ToString("MM/dd/yyyy") : string.Empty);

                // R11
                AddElement(rec, Extensions.GetPropertyShortName(() => Language), string.IsNullOrWhiteSpace(Language) ? "English" : Language);

                // R12
                AddElement(rec, Extensions.GetPropertyShortName(() => Country), string.IsNullOrWhiteSpace(Country) ?  "US" : Country);

                // R13
                if (SponsoringOrgs != null && SponsoringOrgs.Count > 0)
                    AddElement(rec, Extensions.GetPropertyShortName(() => SponsoringOrgs), string.Join("; ", SponsoringOrgs.Select(n => n.Data).ToArray()));
                else
                    AddElement(rec, Extensions.GetPropertyShortName(() => SponsoringOrgs));

                // R14
                var accessLimit = AddElement(AddElement(rec, Extensions.GetPropertyShortName(() => AccessLimitation)), AccessLimitationEnum.GetEnumShortName());
                switch (AccessLimitationEnum)
                {
                    case AccessLimitationEnum.OpenNet:
                        accessLimit.SetAttribute(Extensions.GetPropertyShortName(() => OpenNetData.AccessNumber), OpenNetData.AccessNumber);
                        accessLimit.SetAttribute(Extensions.GetPropertyShortName(() => OpenNetData.DocLocation), OpenNetData.DocLocation);
                        accessLimit.SetAttribute(Extensions.GetPropertyShortName(() => OpenNetData.FieldOfficeAym), OpenNetData.FieldOfficeAym);
                        accessLimit.SetAttribute(Extensions.GetPropertyShortName(() => OpenNetData.DeclassificationStatus), OpenNetData.DeclassStatusEnum.GetEnumShortName());
                        accessLimit.SetAttribute(Extensions.GetPropertyShortName(() => OpenNetData.DeclassificationDate), OpenNetData.DeclassificationDate.HasValue ? OpenNetData.DeclassificationDate.Value.ToString("MM/dd/yyyy") : string.Empty);
                        accessLimit.SetAttribute(Extensions.GetPropertyShortName(() => OpenNetData.Keywords), OpenNetData.Keywords);
                        break;
                    case AccessLimitationEnum.SecuritySensitiveInfo:
                        AddElement(rec, "access_limitation_other", "proprietary");
                        break;
                    case AccessLimitationEnum.ProtectedData:
                        accessLimit.SetAttribute(Extensions.GetPropertyShortName(() => ProtectedData.Crada), ProtectedData.Crada ? "Y" : "N");
                        accessLimit.SetAttribute(Extensions.GetPropertyShortName(() => ProtectedData.Description), ProtectedData.Description);
                        accessLimit.SetAttribute(Extensions.GetPropertyShortName(() => ProtectedData.ReleaseDate), ProtectedData.ReleaseDate.HasValue ? ProtectedData.ReleaseDate.Value.ToString("MM/dd/yyyy") : string.Empty);
                        accessLimit.SetAttribute(Extensions.GetPropertyShortName(() => ProtectedData.ExemptNumber), ProtectedData.ExemptNumber);
                        break;
                    case AccessLimitationEnum.ProgramDeterminedOfficialUseOnly:
                        accessLimit.SetAttribute(Extensions.GetPropertyShortName(() => ExemptionNumber), ExemptionNumber);
                        accessLimit.SetAttribute(Extensions.GetPropertyShortName(() => AccessReleaseDate), AccessReleaseDate.HasValue ? AccessReleaseDate.Value.ToString("MM/dd/yyyy") : string.Empty);
                        AddElement(rec, "access_limitation_other", "proprietary");
                        AddElement(rec, Extensions.GetPropertyShortName(() => AccessReleaseDate), AccessReleaseDate.HasValue ? AccessReleaseDate.Value.ToString("MM/dd/yyyy") : string.Empty);
                        break;
                }

                // R15
                AddElement(rec, "medium_code", "ED");
                if (PublicRelease && !string.IsNullOrWhiteSpace(UrlInt))
                {
                    string filename = System.IO.Path.GetFileName(UrlInt);
                    AddElement(rec, Extensions.GetPropertyShortName(() => Config.DigitalLibraryUrl), Config.DigitalLibraryUrl + "sti/" + filename);
                }
                AddElement(rec, "file_format", "PDF/A");

                // R16 - 18
                AddElement(rec, Extensions.GetPropertyShortName(() => Config.ReleaseOfficial), Config.ReleaseOfficial);
                AddElement(rec, Extensions.GetPropertyShortName(() => ReleasedDate), ReleasedDate.HasValue ? ReleasedDate.Value.ToString("MM/dd/yyyy") : string.Empty);
                AddElement(rec, Extensions.GetPropertyShortName(() => Config.ReleaseOfficialEmail), Config.ReleaseOfficialEmail);
                AddElement(rec, Extensions.GetPropertyShortName(() => Config.ReleaseOfficialPhone), Config.ReleaseOfficialPhone);
                #endregion

                #region Optional
                
                //O2
                if(!string.IsNullOrWhiteSpace(RelatedDocInfo))
                    AddElement(rec, Extensions.GetPropertyShortName(() => RelatedDocInfo), RelatedDocInfo);

                //O3
                if (!string.IsNullOrWhiteSpace(FurtherInfoContact))
                    AddElement(rec, Extensions.GetPropertyShortName(() => FurtherInfoContact), FurtherInfoContact);

                //O4
                if(RdProjectId != null && RdProjectId.Count > 0)
                    AddElement(rec, Extensions.GetPropertyShortName(() => RdProjectId), string.Join("; ", RdProjectId.Select(n => n.Data).ToArray()));

                //O5
                if (WorkProposalNum != null && WorkProposalNum.Count > 0)
                    AddElement(rec, Extensions.GetPropertyShortName(() => WorkProposalNum), string.Join("; ", WorkProposalNum.Select(n => n.Data).ToArray()));

                //O7
                if (WorkAuthNum != null && WorkAuthNum.Count > 0)
                    AddElement(rec, Extensions.GetPropertyShortName(() => WorkAuthNum), string.Join("; ", WorkAuthNum.Select(n => n.Data).ToArray()));

                //O9
                if (OtherNum != null && OtherNum.Count > 0)
                    AddElement(rec, Extensions.GetPropertyShortName(() => OtherNum), string.Join("; ", OtherNum.Select(n => n.Data).ToArray()));

                //10
                if (SubjectCategories != null && SubjectCategories.Count > 0)
                    AddElement(rec, Extensions.GetPropertyShortName(() => SubjectCategories), string.Join("; ", SubjectCategories.Select(n => n.Data).ToArray()));

                // O11
                if (!string.IsNullOrWhiteSpace(Keywords))
                    AddElement(rec, Extensions.GetPropertyShortName(() => Keywords), Keywords);

                // O12
                if(!string.IsNullOrWhiteSpace(Abstract))
                    AddElement(rec, Extensions.GetPropertyShortName(() => Abstract), Abstract);

                // O13
                if(!string.IsNullOrWhiteSpace(ProductSize))
                    AddElement(rec, Extensions.GetPropertyShortName(() => ProductSize), ProductSize);

                // O14
                if(!string.IsNullOrWhiteSpace(PublisherInfo))
                    AddElement(rec, Extensions.GetPropertyShortName(() => PublisherInfo), PublisherInfo);
                

                //15
                if (Contributors != null && Contributors.Count > 0)
                    AddElement(rec, Extensions.GetPropertyShortName(() => Contributors), string.Join("; ", Contributors.Select(n => n.Data).ToArray()));

                #endregion

                return doc;
            }
        }

        private XmlElement AddElement(XmlNode node, string name)
        {
            return AddElement(node, name, string.Empty);
        }

        private XmlElement AddElement(XmlNode node, string name, string value)
        {
            XmlElement n = node is XmlDocument ? ((XmlDocument)node).CreateElement(name) : node.OwnerDocument.CreateElement(name);
            n.InnerText = value;
            node.AppendChild(n);

            return n;
        }

        private void CheckPublishedForChangedData()
        {
            bool changed = false;
            if (SortMainId.HasValue)
            {
                var sort = GetSortMain(SortMainId.Value);
                if (!Extensions.StringsAreEqual(ReportNumber, sort.ReportNumber)) changed = true;
                if (!Extensions.StringsAreEqual(ContractNumber, sort.ContractNumber)) changed = true;
                if (!Extensions.StringsAreEqual(ProductType, sort.ProductType)) changed = true;
                if (!Extensions.StringsAreEqual(TitleStr, sort.TitleStr)) changed = true;
                if (!Extensions.StringsAreEqual(PublishTitle, sort.PublishTitle)) changed = true;
                if (!Extensions.StringsAreEqual(ReportNumbers, sort.ReportNumbers)) changed = true;
                if (PublicationDate != sort.PublicationDate) changed = true;
                if (!Extensions.StringsAreEqual(Language, sort.Language)) changed = true;
                if (!Extensions.StringsAreEqual(Country, sort.Country)) changed = true;
                if (!Extensions.StringsAreEqual(AccessLimitation, sort.AccessLimitation)) changed = true;
                if (ReleasedDate != sort.ReleasedDate) changed = true;
                if (!Extensions.StringsAreEqual(ExemptionNumber, sort.ExemptionNumber)) changed = true;
                if (AccessReleaseDate != sort.AccessReleaseDate) changed = true;
                if (!Extensions.StringsAreEqual(ConferenceName, sort.ConferenceName)) changed = true;
                if (!Extensions.StringsAreEqual(ConferenceLocation, sort.ConferenceLocation)) changed = true;
                if (ConferenceBeginDate != sort.ConferenceBeginDate) changed = true;
                if (ConferenceEndDate != sort.ConferenceEndDate) changed = true;
                if (!Extensions.StringsAreEqual(JournalType, sort.JournalType)) changed = true;
                if (!Extensions.StringsAreEqual(JournalName, sort.JournalName)) changed = true;
                if (!Extensions.StringsAreEqual(JournalVolume, sort.JournalVolume)) changed = true;
                if (!Extensions.StringsAreEqual(JournalIssue, sort.JournalIssue)) changed = true;
                if (!Extensions.StringsAreEqual(JournalSerial, sort.JournalSerial)) changed = true;
                if (JournalStartPage != sort.JournalStartPage) changed = true;
                if (JournalEndPage != sort.JournalEndPage) changed = true;
                if (!Extensions.StringsAreEqual(JournalDoi, sort.JournalDoi)) changed = true;
                if (!Extensions.StringsAreEqual(PublisherName, sort.PublisherName)) changed = true;
                if (!Extensions.StringsAreEqual(PublisherCity, sort.PublisherCity)) changed = true;
                if (!Extensions.StringsAreEqual(PublisherState, sort.PublisherState)) changed = true;
                if (!Extensions.StringsAreEqual(PublisherCountry, sort.PublisherCountry)) changed = true;
                if (!Extensions.StringsAreEqual(PatentAssignee, sort.PatentAssignee)) changed = true;
                if (!Extensions.StringsAreEqual(Abstract, sort.Abstract)) changed = true;
                if (!Extensions.StringsAreEqual(RelatedDocInfo, sort.RelatedDocInfo)) changed = true;
                if (!Extensions.StringsAreEqual(FurtherInfoContact, sort.FurtherInfoContact)) changed = true;
                if (!Extensions.StringsAreEqual(ProductSize, sort.ProductSize)) changed = true;
                if (!Extensions.StringsAreEqual(PublisherInfo, sort.PublisherInfo)) changed = true;
                if (!Extensions.StringsAreEqual(DoiNum, sort.DoiNum)) changed = true;

                if (changed)
                {
                    StatusEnum = StatusEnum.Complete;
                    // Since this is no longer published, we need to remove the OSTI Upload file
                    // That way it can get regenerated with the correct cover letter.
                    SortAttachmentObject.GetOstiAttachment(SortMainId.Value)?.PerminateDelete();
                }
            }
        }

        private static void ClearMainFromCache(int mainId)
        {
            try
            {
                var cache = HttpContext.Current.Cache;
                var cacheKey = $"SortMainObj_{mainId}";
                cache.Remove(cacheKey);
            }
            catch { }
        }
        #endregion
    }

    public interface ISortMainRepository
    {
        List<SortMainObject> GetSortForUser(string employeeId);
        List<SortMainObject> GetSortForUserOrg(string employeeId, string orgOption);
        List<SortMainObject> GetAllSorts(ViewModeOptionEnum viewMode);
        List<SortMainObject> GetOneYearReminders();
        List<SortMainObject> GetDeleyedReminders();
        List<SortMainObject> GetSortNeedingAdlibDocument();
        List<SortMainObject> SearchForUser(string searchData, bool includeTitle, bool includeAbstract, bool includeReviewer);
        List<SortMainObject> GetNeedReminder();
        List<CountData> GetCounts();
        SortMainObject GetSortMainForSharpointId(int sharepointId);
        SortMainObject GetSortMainForLrsId(int lrsId);
        SortMainObject GetSortMain(int sortMainId, IDbConnection conn = null);
        SortMainObject SaveSortMain(SortMainObject sortMain);
        SortMainObject GetSortMainForStiNumber(string stiNumber, int revision);
        bool DeleteSortMain(SortMainObject sortMain);
        int GetSortForUserCount(string employeeId);
        int GetSortForOrgCount(string employeeId, string orgOption);
        int GetAllSortsCount(ViewModeOptionEnum viewMode);
        void UpdateReminderDate(int sortMainId, DateTime reminderDate);
        void UpdateDelayToDate(int sortMainId, DateTime delayToDate, string delayReason);
        void ResetDelayToDate(int sortMainId);
        void UpdateCoverPageRequired(int sortMainId, bool isRequired);
        void SetForceEdms(int sortMainId);
    }

    public class SortMainRepository : ISortMainRepository
    {
        public SortMainObject GetSortMainForSharpointId(int sharepointId) => Config.Conn.Query<SortMainObject>("SELECT * FROM dat_SortMain WHERE SharePointId = @SharePointId AND IsFromLrs = 0", new {SharePointId = sharepointId}).FirstOrDefault();

        public SortMainObject GetSortMainForLrsId(int lrsId) => Config.Conn.Query<SortMainObject>("SELECT * FROM dat_SortMain WHERE SharePointId = @LrsId AND IsFromLrs = 1", new { LrsId = lrsId }).FirstOrDefault();

        public SortMainObject GetSortMain(int sortMainId, IDbConnection conn = null) => (conn ?? Config.Conn).Query<SortMainObject>("SELECT * FROM dat_SortMain WHERE SortMainId = @SortMainId", new {SortMainId = sortMainId}).FirstOrDefault();

        public SortMainObject GetSortMainForStiNumber(string stiNumber, int revision) => Config.Conn.QueryFirstOrDefault<SortMainObject>("SELECT * FROM dat_SortMain WHERE StiNumber = @StiNumber AND Revision = @Revision", new { StiNumber = stiNumber, Revision = revision});

        public List<SortMainObject> GetOneYearReminders() => Config.Conn.Query<SortMainObject>("SELECT * FROM dat_SortMain WHERE OneYearReminderDate <= GETDATE() AND OneYearReminderSent = 0 AND Status = 'Published'").ToList();

        public List<SortMainObject> GetDeleyedReminders() => Config.Conn.Query<SortMainObject>("SELECT * FROM dat_SortMain WHERE DelayToDate <= GETDATE() AND DelayReminderSent = 0").ToList();

        public void UpdateReminderDate(int sortMainId, DateTime reminderDate) => Config.Conn.Execute("Update dat_SortMain SET OneYearReminderDate = @ReminderDate, OneYearReminderSent = 0 WHERE SortMainId = @SortMainId", new {SortMainId = sortMainId, ReminderDate = reminderDate});

        public void UpdateDelayToDate(int sortMainId, DateTime delayToDate, string delayReason) => Config.Conn.Execute("Update dat_SortMain SET DelayToDate = @DelayToDate, DelayReason = @DelayReason, DelayReminderSent = 0 WHERE SortMainId = @SortMainId", new { SortMainId = sortMainId, DelayToDate = delayToDate, DelayReason = delayReason });

        public void ResetDelayToDate(int sortMainId) => Config.Conn.Execute("Update dat_SortMain SET DelayToDate = null, DelayReason = null, DelayReminderSent = 0 WHERE SortMainId = @SortMainId", new { SortMainId = sortMainId });

        public void UpdateCoverPageRequired(int sortMainId, bool isRequired) => Config.Conn.Execute("Update dat_SortMain SET CoverPageRequired = @IsRequired WHERE SortMainId = @SortMainId", new { SortMainId = sortMainId, IsRequired = isRequired });

        public void SetForceEdms(int sortMainId) => Config.Conn.Execute("Update dat_SortMain SET ForceEdms = 1 WHERE SortMainId = @SortMainId", new { SortMainId = sortMainId });

        public List<SortMainObject> GetSortForUser(string employeeId)
        {
            string sql = @"select distinct s.* 
                            From dat_SortMain s
                            where s.OwnerEmployeeId = @EmployeeId
                            and Status <> 'Cancelled'
                            union
                            select s.*
                            from dat_SortMain s
                            inner join dat_Author a on a.SortMainId = s.SortMainId
	                            and a.EmployeeId = @EmployeeId
                            where Status <> 'Cancelled'
                            union
                            select s.*
                            from dat_SortMain s
                            inner join dat_Contact c on c.SortMainId = s.SortMainId
	                            and c.EmployeeId = @EmployeeId
                            where Status <> 'Cancelled'
                            order by SortMainId";

            return Config.Conn.Query<SortMainObject>(sql, new { EmployeeId = employeeId }).ToList();
        }

        public List<SortMainObject> SearchForUser(string searchData, bool includeTitle, bool includeAbstract, bool includeReviewer)
        {
            string sql = @" select distinct m.*
                            from dat_SortMain m
                            left join dat_Author a on a.SortMainId = m.SortMainId
                            left join dat_Contact c on c.SortMainId = m.SortMainId";

            if (includeReviewer)
            {
                sql += @"   left join dat_Review r on r.SortMainId = m.SortMainId";
            }

            sql += @"       where m.StiNumber like '%' + @SearchData + '%'
                            or a.[FirstName] like '%' + @SearchData + '%'
                            or a.[LastName] like '%' + @SearchData + '%'
                            or c.[FirstName] like '%' + @SearchData + '%'
                            or c.[LastName] like '%' + @SearchData + '%'
                            or m.OwnerName like '%' + @SearchData + '%'";

            if (includeReviewer)
            {
                sql += @" or r.Reviewer like '%' + @SearchData + '%'";
            }

            if (includeTitle)
            {
                sql += " or m.PublishTitle like '%' + @SearchData + '%'";
            }

            if (includeAbstract)
            {
                sql += "or m.Abstract like '%' + @SearchData + '%'";
            }

            return Config.Conn.Query<SortMainObject>(sql, new { SearchData = searchData }).ToList();
        }

        public int GetSortForUserCount(string employeeId)
        {
            string sql = @"select count(*) from (
                            select distinct s.* 
                            From dat_SortMain s
                            where s.OwnerEmployeeId = @EmployeeId
                            and Status <> 'Cancelled'
                            union
                            select s.*
                            from dat_SortMain s
                            inner join dat_Author a on a.SortMainId = s.SortMainId
	                            and a.EmployeeId = @EmployeeId
                            where Status <> 'Cancelled'
                            union
                            select s.*
                            from dat_SortMain s
                            inner join dat_Contact c on c.SortMainId = s.SortMainId
	                            and c.EmployeeId = @EmployeeId
                            where Status <> 'Cancelled'
                            ) as dt";

            return Config.Conn.QueryFirstOrDefault<int>(sql, new { EmployeeId = employeeId });
        }

        public List<SortMainObject> GetSortForUserOrg(string employeeId, string orgOption)
        {
            var employee = EmployeeCache.GetEmployee(employeeId);

            string sql = @"select distinct s.* 
                            From dat_SortMain s
                            inner join dat_UserOrg o on o.EmployeeId = s.OwnerEmployeeId
		                        and o.Org = @WorkOrg
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where s.OwnerEmployeeId = @EmployeeId
                            AND Status <> 'Cancelled'
                            union
                            select s.*
                            from dat_SortMain s
                            inner join dat_Author a on a.SortMainId = s.SortMainId
	                        inner join dat_UserOrg o on o.Org = a.WorkOrg
		                        and o.EmployeeId = @EmployeeId
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where Status <> 'Cancelled'
                            union
                            select s.*
                            from dat_SortMain s
                            inner join dat_Contact c on c.SortMainId = s.SortMainId
	                        inner join dat_UserOrg o on o.Org = c.WorkOrg
		                        and o.EmployeeId = @EmployeeId
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where Status <> 'Cancelled'
                            order by SortMainId";

            return Config.Conn.Query<SortMainObject>(sql, new { EmployeeId = employeeId, WorkOrg = employee?.WorkOrginization, OrgOption = orgOption }).ToList();
        }

        public int GetSortForOrgCount(string employeeId, string orgOption)
        {
            var employee = EmployeeCache.GetEmployee(employeeId);

            string sql = @"select count(*) from (
                            select distinct s.* 
                            From dat_SortMain s
                            inner join dat_UserOrg o on o.EmployeeId = s.OwnerEmployeeId
		                        and o.Org = @WorkOrg
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where s.OwnerEmployeeId = @EmployeeId
                            and Status <> 'Cancelled'
                            union
                            select s.*
                            from dat_SortMain s
                            inner join dat_Author a on a.SortMainId = s.SortMainId
	                        inner join dat_UserOrg o on o.Org = a.WorkOrg
		                        and o.EmployeeId = @EmployeeId
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where Status <> 'Cancelled'
                            union
                            select s.*
                            from dat_SortMain s
                            inner join dat_Contact c on c.SortMainId = s.SortMainId
	                        inner join dat_UserOrg o on o.Org = c.WorkOrg
		                        and o.EmployeeId = @EmployeeId
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where Status <> 'Cancelled'
                            ) as dt";

            return Config.Conn.QueryFirstOrDefault<int>(sql, new { EmployeeId = employeeId, WorkOrg = employee?.WorkOrginization, OrgOption = orgOption });
        }

        public List<SortMainObject> GetAllSorts(ViewModeOptionEnum viewMode)
        {
            string sql = string.Empty;
            switch (viewMode)
            {
                case ViewModeOptionEnum.Cancelled:
                    sql = @"SELECT * FROM dat_SortMain WHERE Status = 'Cancelled'";
                    break;
                case ViewModeOptionEnum.CompletedNeedsApproved:
                    sql = @"SELECT * FROM dat_SortMain WHERE Status = 'Complete' and ReviewStatus <> 'Approved'";
                    break;
                case ViewModeOptionEnum.CompletedNeedsPublished:
                    sql = @"SELECT * FROM dat_SortMain WHERE Status = 'Complete' and ReviewStatus = 'Approved'";
                    break;
                case ViewModeOptionEnum.InProcess:
                    sql = @"SELECT * FROM dat_SortMain WHERE Status in ('Approved', 'Imported') and isnull(DueDate, '12/31/2050') >= GETDATE()";
                    break;
                case ViewModeOptionEnum.PastDue:
                    sql = @"SELECT * FROM dat_SortMain WHERE Status in ('Approved', 'Imported') and isnull(DueDate, '12/31/2050') < GETDATE()";
                    break;
                case ViewModeOptionEnum.Published:
                    sql = @"SELECT * FROM dat_SortMain WHERE Status = 'Published'";
                    break;
                default:
                    sql = @"SELECT * FROM dat_SortMain WHERE Status not in ('Cancelled', 'Published')";
                    break;
            }

            return Config.Conn.Query<SortMainObject>(sql).ToList();
        }

        public int GetAllSortsCount(ViewModeOptionEnum viewMode)
        {
            string sql = string.Empty;
            switch (viewMode)
            {
                case ViewModeOptionEnum.Cancelled:
                    sql = @"SELECT Count(*) FROM dat_SortMain WHERE Status = 'Cancelled'";
                    break;
                case ViewModeOptionEnum.CompletedNeedsApproved:
                    sql = @"SELECT Count(*) FROM dat_SortMain WHERE Status = 'Complete' and ReviewStatus <> 'Approved'";
                    break;
                case ViewModeOptionEnum.CompletedNeedsPublished:
                    sql = @"SELECT Count(*) FROM dat_SortMain WHERE Status = 'Complete' and ReviewStatus = 'Approved'";
                    break;
                case ViewModeOptionEnum.InProcess:
                    sql = @"SELECT Count(*) FROM dat_SortMain WHERE Status in ('Approved', 'Imported') and isnull(DueDate, '12/31/2050') >= GETDATE()";
                    break;
                case ViewModeOptionEnum.PastDue:
                    sql = @"SELECT Count(*) FROM dat_SortMain WHERE Status in ('Approved', 'Imported') and isnull(DueDate, '12/31/2050') < GETDATE()";
                    break;
                case ViewModeOptionEnum.Published:
                    sql = @"SELECT Count(*) FROM dat_SortMain WHERE Status = 'Published'";
                    break;
                default:
                    sql = @"SELECT Count(*) FROM dat_SortMain WHERE Status not in ('Cancelled', 'Published')";
                    break;
            }

            return Config.Conn.QueryFirstOrDefault<int>(sql);
        }

        public SortMainObject SaveSortMain(SortMainObject sortMain)
        {
            if (sortMain.SortMainId.HasValue) // Update
            {
                string sql = @"
                    UPDATE  dat_SortMain
                    SET     SharePointId = @SharePointId,
                            ReportNumber = @ReportNumber,
                            OwnerEmployeeId = @OwnerEmployeeId,
                            OwnerName = @OwnerName,
                            OwnerEmail = @OwnerEmail,
                            ContractNumber = @ContractNumber,
                            OstiId = @OstiId,
                            Status = @Status,
                            OstiStatus = @OstiStatus,
                            OstiStatusMsg = @OstiStatusMsg,
                            OstiDate = @OstiDate,
                            ProductType = @ProductType,
                            Title = @Title,
                            StiNumber = @StiNumber,
                            Revision = @Revision,
                            PublishTitle = @PublishTitle,
                            ReportNumbers = @ReportNumbers,
                            PublicationDate = @PublicationDate,
                            Language = @Language,
                            Country = @Country,
                            AccessLimitation = @AccessLimitation,
                            ReleasedDate = @ReleasedDate,
                            UrlInt = @UrlInt,
                            StiSpId = @StiSpId,
                            ExemptionNumber = @ExemptionNumber,
                            AccessReleaseDate = @AccessReleaseDate,
                            ConferenceName = @ConferenceName,
                            ConferenceSponsor = @ConferenceSponsor,
                            ConferenceLocation = @ConferenceLocation,
                            ConferenceBeginDate = @ConferenceBeginDate,
                            ConferenceEndDate = @ConferenceEndDate,
                            JournalType = @JournalType,
                            JournalName = @JournalName,
                            JournalVolume = @JournalVolume,
                            JournalIssue = @JournalIssue,
                            JournalSerial = @JournalSerial,
                            JournalStartPage = @JournalStartPage,
                            JournalEndPage = @JournalEndPage,
                            JournalDoi = @JournalDoi,
                            PublisherName = @PublisherName,
                            PublisherCity = @PublisherCity,
                            PublisherState = @PublisherState,
                            PublisherCountry = @PublisherCountry,
                            PatentAssignee = @PatentAssignee,
                            Abstract = @Abstract,
                            RelatedDocInfo = @RelatedDocInfo,
                            FurtherInfoContact = @FurtherInfoContact,
                            ProductSize = @ProductSize,
                            PublisherInfo = @PublisherInfo,
                            CreateDate = @CreateDate,
                            ModifiedDate = @ModifiedDate,
                            ApprovedDate = @ApprovedDate,
                            DueDate = @DueDate,
                            PublishDate = @PublishDate,
                            ReviewStatus = @ReviewStatus,
                            ReviewProgress = @ReviewProgress,
                            DoiNum = @DoiNum,
                            OneYearReminderDate = @OneYearReminderDate,
                            OneYearReminderSent = @OneYearReminderSent,
                            HasTechWriter = @HasTechWriter,
                            TechWriterEmployeeId = @TechWriterEmployeeId,
                            IsFromLrs = @IsFromLrs,
                            DelayToDate = @DelayToDate,
                            DelayReason = @DelayReason,
                            DelayReminderSent = @DelayReminderSent
                    WHERE   SortMainId = @SortMainId";
                Config.Conn.Execute(sql, sortMain);
            }
            else
            {
                sortMain.CoverPageRequired = !sortMain.IsExternalType;
                string sql = @"
                    INSERT INTO dat_SortMain (
                        SharePointId,
                        ReportNumber,
                        OwnerEmployeeId,
                        OwnerName,
                        OwnerEmail,
                        ContractNumber,
                        OstiId,
                        Status,
                        OstiStatus,
                        OstiStatusMsg,
                        OstiDate,
                        ProductType,
                        Title,
                        StiNumber,
                        Revision,
                        PublishTitle,
                        ReportNumbers,
                        PublicationDate,
                        Language,
                        Country,
                        AccessLimitation,
                        ReleasedDate,
                        UrlInt,
                        StiSpId,
                        ExemptionNumber,
                        AccessReleaseDate,
                        ConferenceName,
                        ConferenceSponsor, 
                        ConferenceLocation,
                        ConferenceBeginDate,
                        ConferenceEndDate,
                        JournalType,
                        JournalName,
                        JournalVolume,
                        JournalIssue,
                        JournalSerial,
                        JournalStartPage,
                        JournalEndPage,
                        JournalDoi,
                        PublisherName,
                        PublisherCity,
                        PublisherState,
                        PublisherCountry,
                        PatentAssignee,
                        Abstract,
                        RelatedDocInfo,
                        FurtherInfoContact,
                        ProductSize,
                        PublisherInfo,
                        CreateDate,
                        ModifiedDate,
                        ApprovedDate,
                        DueDate,
                        PublishDate,
                        ReviewStatus,
                        ReviewProgress,
                        DoiNum,
                        OneYearReminderDate,
                        OneYearReminderSent,
                        HasTechWriter,
                        TechWriterEmployeeId,
                        IsFromLrs,
                        CoverPageRequired,
                        RelatedSti                        
                    )
                    VALUES (
                        @SharePointId,
                        @ReportNumber,
                        @OwnerEmployeeId,
                        @OwnerName,
                        @OwnerEmail,
                        @ContractNumber,
                        @OstiId,
                        @Status,
                        @OstiStatus,
                        @OstiStatusMsg,
                        @OstiDate,
                        @ProductType,
                        @Title,
                        @StiNumber,
                        @Revision,
                        @PublishTitle,
                        @ReportNumbers,
                        @PublicationDate,
                        @Language,
                        @Country,
                        @AccessLimitation,
                        @ReleasedDate,
                        @UrlInt,
                        @StiSpId,
                        @ExemptionNumber,
                        @AccessReleaseDate,
                        @ConferenceName,
                        @ConferenceSponsor,
                        @ConferenceLocation,
                        @ConferenceBeginDate,
                        @ConferenceEndDate,
                        @JournalType,
                        @JournalName,
                        @JournalVolume,
                        @JournalIssue,
                        @JournalSerial,
                        @JournalStartPage,
                        @JournalEndPage,
                        @JournalDoi,
                        @PublisherName,
                        @PublisherCity,
                        @PublisherState,
                        @PublisherCountry,
                        @PatentAssignee,
                        @Abstract,
                        @RelatedDocInfo,
                        @FurtherInfoContact,
                        @ProductSize,
                        @PublisherInfo,
                        @CreateDate,
                        @ModifiedDate,
                        @ApprovedDate,
                        @DueDate,
                        @PublishDate,
                        @ReviewStatus,
                        @ReviewProgress,
                        @DoiNum,
                        @OneYearReminderDate,
                        @OneYearReminderSent,
                        @HasTechWriter,
                        @TechWriterEmployeeId,
                        @IsFromLrs,
                        @CoverPageRequired,
                        @RelatedSti
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                sortMain.SortMainId = Config.Conn.Query<int>(sql, sortMain).Single();
            }
            return sortMain;
        }

        public bool DeleteSortMain(SortMainObject sortMain)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_Author WHERE SortMainId = @SortMainId", sortMain);
                Config.Conn.Execute("DELETE FROM dat_Contact WHERE SortMainId = @SortMainId", sortMain);
                Config.Conn.Execute("DELETE FROM dat_Funding WHERE SortMainId = @SortMainId", sortMain);
                Config.Conn.Execute("DELETE FROM dat_OpenNet WHERE SortMainId = @SortMainId", sortMain);
                Config.Conn.Execute("DELETE FROM dat_OstiUpload WHERE SortMainId = @SortMainId", sortMain);
                Config.Conn.Execute("DELETE FROM dat_ProtectedData WHERE SortMainId = @SortMainId", sortMain);
                Config.Conn.Execute("DELETE FROM dat_SortMetaData WHERE SortMainId = @SortMainId", sortMain);
                Config.Conn.Execute("DELETE FROM dat_Review WHERE SortMainId = @SortMainId", sortMain);
                Config.Conn.Execute("DELETE FROM dat_SortAttachmentFile WHERE SortAttachmentId in (select SortAttachmentId FROM dat_SortAttachment WHERE SortMainId = @SortMainId)", sortMain);
                Config.Conn.Execute("DELETE FROM dat_SortAttachment WHERE SortMainId = @SortMainId", sortMain);
                Config.Conn.Execute("DELETE FROM dat_SortMain WHERE SortMainId = @SortMainId", sortMain);
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("SortMainObject::DeleteSortMain", ex);
                return false;
            }
            return true;
        }

        public List<CountData> GetCounts()
        {
            string sql = @"select [Status], 
                                  case when isnull(DueDate, '12/31/2050') >= GETDATE() then 0 else 1 end as PastDueDate, 
                                  ReviewStatus,
                                  COUNT(*) as NumItems
                            From dat_SortMain
                            group by [Status], ReviewStatus, case when isnull(DueDate, '12/31/2050') >= GETDATE() then 0 else 1 end";
            return Config.Conn.Query<CountData>(sql).ToList();
        }

        public List<SortMainObject> GetSortNeedingAdlibDocument()
        {
            string sql = @" select sm.* 
                            From dat_SortMain sm
                            left join dat_SortAttachment a on a.SortMainId = sm.SortMainId
	                            and a.AttachmentType = 'OstiDoc'
	                            and a.IsDeleted = 0
                             where sm.Status in ('Complete','Published') 
                             and a.SortAttachmentId is null";

            return Config.Conn.Query<SortMainObject>(sql).ToList();
        }

        public List<SortMainObject> GetNeedReminder() => Config.Conn.Query<SortMainObject>("select * From dat_SortMain where RIGHT(StiNumber, 5) >= 50000 and [Status] in ('Approved', 'Imported')").ToList();
    }

    public class CountData
    {
        public string Status { get; set; }
        public string ReviewStatus { get; set; }
        public bool PastDueDate { get; set; }
        public int NumItems { get; set; }

        public StatusEnum StatusEnum => Status.ToEnum<StatusEnum>();

        public CountData() { }
    }

    public class MissingData
    {
        public string Section { get; set; }
        public string Error { get; set; }

        public MissingData() { }

        public MissingData(string section, string error)
        {
            Section = section;
            Error = error;
        }
    }

    [XmlRoot(ElementName = "records", Namespace = "")]
    public class OstiRecords
    {
        [XmlElement("record", Namespace = "")]
        public OstiRecord[] Record { get; set; }
    }

    public class OstiRecord
    {
        [XmlElement("osti_id")]
        public int? Id { get; set; }
        [XmlElement("product_nos")]
        public string ProductNumbers { get; set; }
        [XmlElement("contract_nos")]
        public string ContractNumbers { get; set; }
        [XmlElement("other_identifying_nos")]
        public string OtherIdentifyingNumbers { get; set; }
        [XmlElement("status")]
        public string Status { get; set; }
        [XmlElement("status_message")]
        public string StatusMessage { get; set; }
    }
}