using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Inl.MvcHelper;

namespace LRS.Business
{
    public class MainObject
    {
        #region Properties

        public int? MainId { get; set; }
        public string OwnerEmployeeId { get; set; }
        public string DocumentType { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        [Display(Name = "STI Number")]
        public string StiNumber { get; set; }
        public string Abstract { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Comments { get; set; }
        public bool ContainsSciInfo { get; set; }
        public bool ContainsTechData { get; set; }
        public bool TechDataPublic { get; set; }
        public DateTime? NeedByDate { get; set; }
        [Display(Name = "Last Activity Date")]
        public DateTime? ActivityDate { get; set; }
        public int? DocNumber { get; set; }
        public string ConferenceName { get; set; }
        public string ConferenceSponsor { get; set; }
        public string ConferenceLocation { get; set; }
        public DateTime? ConferenceBeginDate { get; set; }
        public DateTime? ConferenceEndDate { get; set; }
        [Display(Name = "Journal Name")]
        public string JournalName { get; set; }

        public int Revision { get; set; }
        [Display(Name = "Related STI #")]
        public string RelatedSti { get; set; }
        public int? ParentId { get; set; }
        public int? ChildId { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int? SortId { get; set; }
        public string SortError { get; set; }
        public string ReviewStatus { get; set; }
        public string ReviewState { get; set; }
        public double ReviewPercent { get; set; }
        public bool Ouo3 { get; set; }
        public bool Ouo3b { get; set; }
        public bool Ouo4 { get; set; }
        public bool Ouo5 { get; set; }
        public bool Ouo6 { get; set; }
        public bool Ouo7 { get; set; }
        [Display(Name = "Derivative Classifier")]
        public string Ouo7EmployeeId { get; set; }
        public int? OldLrsId { get; set; }
        public int? OstiId { get; set; }
        public DateTime? DateOsti { get; set; }
        public bool RushFlag { get; set; }
        [Display(Name = "Please note why this item is for limited distribution")]
        public string LimitedExp { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        #endregion

        #region Date Formatting

        public string CreateDateStr => CreateDate?.ToString("MM/dd/yyyy hh:mm tt");
        public string ApprovalDateStr => ApprovalDate?.ToString("MM/dd/yyyy hh:mm tt");
        public string ConferenceBeginDateStr => ConferenceBeginDate?.ToString("MM/dd/yyyy");
        public string ConferenceEndDateStr => ConferenceEndDate?.ToString("MM/dd/yyyy");
        public string ActivityDateStr => $"{ActivityDate:MM/dd/yyyy hh:mm tt}";
        #endregion

        #region Extanded Properties

        public string AbstractWithAccessCheck => UserHasReadAccess() ? Abstract : string.Empty;

        public bool NeedSentToSort => StatusEnum == StatusEnum.Completed && ContainsSciInfo && !SortId.HasValue && DocumentTypeEnum != DocumentTypeEnum.DoeId;

        public string Ouo7Name => EmployeeCache.GetEmployee(Ouo7EmployeeId, true)?.FullName;

        public string DisplayTitle => $"{StiNumber} Rev:{RevisionStr}";
        [Display(Name = "Revision")]
        public string RevisionStr => $"{Revision:D3}";
        public StatusEnum StatusEnum
        {
            get => Status.ToEnum<StatusEnum>();
            set => Status = value.ToString();
        }

        public DocumentTypeEnum DocumentTypeEnum
        {
            get => DocumentType.ToEnum<DocumentTypeEnum>();
            set => DocumentType = value.ToString();
        }

        public ReviewerTypeEnum ReviewStateEnum
        {
            get => ReviewState.ToEnum<ReviewerTypeEnum>();
            set => ReviewState = value.ToString();
        }
        
        public string ReviewProgress
        {
            get
            {
                if (StatusEnum == StatusEnum.InReview)
                {
                    double percent = ReviewPercent;
                    
                    return $"<div class=\"progress\"><div class=\"progress-bar\" role\"progressbar\" aria-valuenow=\"70\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width:{percent:f0}%;\"><span>{percent:f0}%</span></div></div>";
                }
                if (StatusEnum == StatusEnum.Completed)
                {
                    return $"<div class=\"progress\"><div class=\"progress-bar\" role\"progressbar\" aria-valuenow=\"70\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width:100%;\"><span>100%</span></div></div>";
                }

                return string.Empty;
            }
        }

        public string DocumentTypeStr => DocumentTypeEnum.GetEnumDisplayName();

        [Display(Name = "Authors")]
        public List<AuthorObject> Authors => AuthorObject.GetAuthors(MainId??0) ?? new List<AuthorObject>();
        [Display(Name = "Contacts")]
        public List<ContactObject> Contacts => ContactObject.GetContacts(MainId ?? 0) ?? new List<ContactObject>();
        [Display(Name = "Intellectual Property")]
        public List<IntellectualPropertyObject> Intellectuals => IntellectualPropertyObject.GetIntellectualProperties(MainId ?? 0) ?? new List<IntellectualPropertyObject>();
        [Display(Name = "Funding")]
        public List<FundingObject> Funding => FundingObject.GetFundings(MainId ?? 0) ?? new List<FundingObject>();
        [Display(Name = "Subject Category Codes", ShortName = "subject_category_code")]
        public List<MetaDataObject> SubjectCategories => MetaDataObject.GetMetaDatas(MainId ?? 0, MetaDataTypeEnum.SubjectCategories);
        [Display(Name = "Keyword(s)")]
        public List<MetaDataObject> KeyWordList => MetaDataObject.GetMetaDatas(MainId ?? 0, MetaDataTypeEnum.Keywords);
        [Display(Name = "Attachment(s)")]
        public List<AttachmentObject> Attachments => AttachmentObject.GetAttachments(MainId ?? 0);
        [Display(Name = "Sponsoring Organization(s)")]
        public List<MetaDataObject> SponsoringOrgs => MetaDataObject.GetMetaDatas(MainId ?? 0, MetaDataTypeEnum.SponsoringOrgs);
        [Display(Name = "Core Capabilities")]
        public List<MetaDataObject> CoreCapabilities => MetaDataObject.GetMetaDatas(MainId ?? 0, MetaDataTypeEnum.CoreCapabilities);
        [Display(Name = "Reviewer(s)")]
        public List<ReviewObject> Reviewers => ReviewObject.GetReviews(MainId ?? 0) ?? new List<ReviewObject>();

        [Display(Name = "Review History")]
        public List<ReviewHistoryObject> ReviewHistory => ReviewHistoryObject.GetReviewHistories(MainId ?? 0) ?? new List<ReviewHistoryObject>();

        [Display(Name = "Created By")]
        public string OwnerName => EmployeeCache.GetEmployee(OwnerEmployeeId, true)?.PreferredName;

        public string OwnerWorkOrg => EmployeeCache.GetEmployee(OwnerEmployeeId, true)?.WorkOrginization;

        [Display(ShortName = "keywords")]
        public string Keywords => string.Join("; ", KeyWordList?.Select(n => n.Data));

        public List<MissingData> MissingDatas { get; private set; }
        
        public string StatusDisplayName => StatusEnum.GetEnumDisplayName();

        public string NumberPagesStr => Attachments?.Sum(n => n.NumberPages ?? 0).ToString() ?? string.Empty;

        public string OwnerEmail => EmployeeCache.GetEmployee(OwnerEmployeeId, false)?.Email;

        public string GridButtons
        {
            get
            {
                StringBuilder returnStr = new StringBuilder();
                returnStr.Append("<a href=\"" + UrlHelper.Action("Index", "Artifact", new { id = MainId }) + "\" title=\"View\" class=\"btn btn-xs btn-primary\" style=\"margin-left: 3px;\"><i class=\"fa fa-eye\"> View</i></a>");
                //if (StatusEnum == StatusEnum.Imported ||
                //    StatusEnum == StatusEnum.Complete ||
                //    (StatusEnum == StatusEnum.Published && UserObject.CurrentUser.IsInAnyRole("Admin,ReleaseOfficial")))
                //{
                //    returnStr.Append("<a href=\"" + UrlHelper.Action("Edit", "Artifact", new { id = MainId }) + "\" title=\"Edit\" class=\"btn btn-xs btn-success\" style=\"margin-left: 3px;\"><i class=\"fa fa-pencil\"> Edit</i></a>");
                //}
                
                return returnStr.ToString();
            }
        }

        public string ReviewGridButtons
        {
            get
            {
                StringBuilder returnStr = new StringBuilder();
                returnStr.Append("<a href=\"" + UrlHelper.Action("Index", "Artifact", new { id = MainId }) + "\" title=\"View\" class=\"btn btn-xs btn-primary\" style=\"margin-left: 3px;\"><i class=\"fa fa-eye\"> View</i></a>");
                if (MemoryCache.UserIsGenericUser(UserObject.CurrentUser.EmployeeId))
                {
                    bool reviewerAssigned = false;
                    switch (MemoryCache.GetUserGenericType(UserObject.CurrentUser.EmployeeId))
                    {
                        case ReviewerTypeEnum.Classification:
                            reviewerAssigned = !string.IsNullOrWhiteSpace(Reviewers.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Classification)?.ReviewerEmployeeId);
                            break;
                        case ReviewerTypeEnum.ExportControl:
                            reviewerAssigned = !string.IsNullOrWhiteSpace(Reviewers.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl)?.ReviewerName);
                            break;
                        case ReviewerTypeEnum.TechDeployment:
                            reviewerAssigned = !string.IsNullOrWhiteSpace(Reviewers.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment)?.ReviewerName);
                            break;
                    }

                    if (!reviewerAssigned)
                    {
                        returnStr.Append("<a href=\"" + UrlHelper.Action("Claim", "Artifact", new { id = MainId }) + "\" title=\"Claim\" class=\"btn btn-xs btn-default\" style=\"margin-left: 3px;\"><i class=\"fa fa-hand-grab-o\"> Claim</i></a>");
                    }
                }

                return returnStr.ToString();
            }
        }

        public string GridFlags
        {
            get
            {
                StringBuilder returnStr = new StringBuilder();
                if (StatusEnum == StatusEnum.InReview)
                {
                    if ((MemoryCache.UserIsGenericUser(UserObject.CurrentUser.EmployeeId) || UserObject.CurrentUser.IsInAnyRole("Admin,ReleaseOfficial,GenericReleaseUser")))
                    {
                        if (RushFlag)
                        {
                            returnStr.Append("<a href=\"" + UrlHelper.Action("ToggleRush", "Artifact", new {id = MainId}) + "\" title=\"Toggle Rush Flag\" class=\"btn btn-xs\" style=\"margin-left: 3px; color: red;\"><i class=\"fa fa-flag\"><sup>rush</sup></i></a>");
                        }
                        else
                        {
                            returnStr.Append("<a href=\"" + UrlHelper.Action("ToggleRush", "Artifact", new {id = MainId}) + "\" title=\"Toggle Rush Flag\" class=\"btn btn-xs\" style=\"margin-left: 3px; color: black;\"><i class=\"fa fa-flag-o\"></i></a>");
                        }
                    }
                    else
                    {
                        if (RushFlag)
                        {
                            returnStr.Append("<div style=\"margin-left: 3px; color: red;\"><i class=\"fa fa-flag\"><sup>r</sup></i></div>");
                        }
                    }
                }

                return returnStr.ToString();
            }
        }

        public bool IsActiveReviewer
        {
            get
            {
                switch (ReviewStateEnum)
                {
                    case ReviewerTypeEnum.PeerTechnical:
                        return Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.PeerTechnical && n.ReviewStatus == ReviewStatusEnum.Active && n.ReviewerEmployeeId == UserObject.CurrentUser.EmployeeId);
                    case ReviewerTypeEnum.Manager:
                        return Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Manager && n.ReviewStatus == ReviewStatusEnum.Active && n.ReviewerEmployeeId == UserObject.CurrentUser.EmployeeId);
                    case ReviewerTypeEnum.Classification:
                    case ReviewerTypeEnum.ExportControl:
                    case ReviewerTypeEnum.TechDeployment:
                        return Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Classification  && n.ReviewStatus == ReviewStatusEnum.Active && n.SystemReviewerUsers.Exists(m => m.EmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase))) ||
                               Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl   && n.ReviewStatus == ReviewStatusEnum.Active && n.SystemReviewerUsers.Exists(m => m.EmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase))) ||
                               Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment  && n.ReviewStatus == ReviewStatusEnum.Active && n.SystemReviewerUsers.Exists(m => m.EmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase)));
                    default:
                        return false;
                }
            }
        }

        public ReviewObject ActiveReviewer
        {
            get
            {
                if (IsActiveReviewer)
                {
                    return Reviewers.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.PeerTechnical   && n.ReviewStatus == ReviewStatusEnum.Active && n.ReviewerEmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase)) ??
                           Reviewers.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Manager         && n.ReviewStatus == ReviewStatusEnum.Active && n.ReviewerEmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase)) ??
                           Reviewers.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Classification  && n.ReviewStatus == ReviewStatusEnum.Active && n.SystemReviewerUsers.Exists(m => m.EmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase))) ??
                           Reviewers.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl   && n.ReviewStatus == ReviewStatusEnum.Active && n.SystemReviewerUsers.Exists(m => m.EmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase))) ??
                           Reviewers.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment  && n.ReviewStatus == ReviewStatusEnum.Active && n.SystemReviewerUsers.Exists(m => m.EmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase)));
                }

                return null;
            }
        }

        public bool IsSpecialReviewer
        {
            get
            {
                return Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Classification && n.SystemReviewerUsers.Exists(m => m.EmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase))) ||
                       Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl && n.SystemReviewerUsers.Exists(m => m.EmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase))) ||
                       Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment && n.SystemReviewerUsers.Exists(m => m.EmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase)));
            }
        }

        public string OuoEmailText
        {
            get
            {
                string rtn = string.Empty;
                if (Ouo3) { rtn += "OUO - FOIA Exemption 3 may apply. Contains export controlled information.<br/>"; }
                if (Ouo3b) { rtn += "OUO - FOIA Exemption 3 may apply. Information protected by federal laws (CRADA, Export Controlled, Archaeological, etc.)..<br/>"; }
                if (Ouo4) { rtn += "OUO - FOIA Exemption 4 may apply. Contains commercial/proprietary information.<br/>"; }
                if (Ouo5) { rtn += "OUO - FOIA Exemption 5 may apply. Contains predecisional or privileged information.<br/>"; }
                if (Ouo6) { rtn += "OUO - FOIA Exemption 6 may apply. Contains PII information.<br/>"; }
                if (Ouo7) { rtn += $"OUO - FOIA Exemption 7 may apply as determined by derivative classifier ({Ouo7Name}).<br/>"; }

                if (!string.IsNullOrWhiteSpace(rtn))
                {
                    rtn = "<p>" + rtn + "</p>";
                }

                return rtn;
            }
        }

        public string ReviewerClaim
        {
            get
            {
                switch (MemoryCache.GetUserGenericType(UserObject.CurrentUser.EmployeeId))
                {
                    case ReviewerTypeEnum.Classification:
                        return Reviewers.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Classification)?.ReviewerName;
                    case ReviewerTypeEnum.ExportControl:
                        return Reviewers.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl)?.ReviewerName;
                    case ReviewerTypeEnum.TechDeployment:
                        return Reviewers.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment)?.ReviewerName;
                }

                return string.Empty;
            }
        }

        public void ClaimReview()
        {
            ReviewObject review = null;
            switch (MemoryCache.GetUserGenericType(UserObject.CurrentUser.EmployeeId))
            {
                case ReviewerTypeEnum.Classification:
                    review = Reviewers.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Classification);
                    break;
                case ReviewerTypeEnum.ExportControl:
                    review = Reviewers.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl);
                    break;
                case ReviewerTypeEnum.TechDeployment:
                    review = Reviewers.FirstOrDefault(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment);
                    break;
            }

            if (review != null)
            {
                review.ReviewerEmployeeId = UserObject.CurrentUser.EmployeeId;
                review.ReviewerFirstName = EmployeeCache.GetEmployee(review.ReviewerEmployeeId)?.FirstName;
                review.ReviewerLastName = EmployeeCache.GetEmployee(review.ReviewerEmployeeId)?.LastName;
                review.Save();
            }
        }
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

        public MainObject() { }

        #endregion

        #region Repository

        private static IMainRepository repo => new MainRepository();

        #endregion

        #region Static Methods

        public static List<MainObject> GetAllMains(ViewModeOptionEnum viewMode, int viewTime) => repo.GetAllMains(viewMode, viewTime);
        public static int GetAllMainsCount(ViewModeOptionEnum viewMode, int viewTime) => repo.GetAllMainsCount(viewMode, viewTime);

        public static MainObject GetMain(int mainId, IDbConnection conn = null)
        {
            try
            {
                var cacheKey = $"MainObj_{mainId}";
                var cache = HttpContext.Current.Cache;
                var mob = (MainObject) cache[cacheKey];
                if (mob == null)
                {
                    mob = repo.GetMain(mainId, conn);
                    if (mob != null)
                    {
                        cache.Insert(cacheKey, mob, null, DateTime.UtcNow.AddMinutes(Config.ApplicationMode == ApplicationMode.Production ? 5 : 1), Cache.NoSlidingExpiration);
                    }
                }

                return mob;
            }
            catch { }

            return null;
        }

        public static bool CheckUserHasWriteAccess(int mainId)
        {
            if (UserObject.CurrentUser.IsInAnyRole("Admin,ReleaseOfficial")) return true;

            return GetMain(mainId)?.UserHasWriteAccess() ?? false;
        }

        public static bool CheckUserHasReadAccess(int mainId)
        {
            if (UserObject.CurrentUser.IsInAnyRole("Admin,ReleaseOfficial,GenericReleaseUser")) return true;

            return GetMain(mainId)?.UserHasReadAccess() ?? false;
        }

        public static List<CountData> GetCounts(int viewTime) => repo.GetCounts(viewTime);

        public static List<MainObject> GetAllForUser(ViewModeOptionEnum viewMode, int viewTime, string employeeId) => repo.GetAllForUser(viewMode, viewTime, employeeId);

        public static int GetAllForUserCount(ViewModeOptionEnum viewMode, int viewTime, string employeeId) => repo.GetAllForUserCount(viewMode, viewTime, employeeId);

        public static List<MainObject> GetAllForOrg(ViewModeOptionEnum viewMode, int viewTime, string employeeId, string orgOption) => repo.GetAllForOrg(viewMode, viewTime, employeeId, orgOption);
        public static int GetAllForOrgCount(ViewModeOptionEnum viewMode, int viewTime, string employeeId, string orgOption) => repo.GetAllForOrgCount(viewMode, viewTime, employeeId, orgOption);

        public static List<MainObject> GetReviewsForUser(string employeeId) => repo.GetReviewsForUser(employeeId);

        public static List<MainObject> SearchForUser(string searchData, bool includeTitle, bool includeAbstract, bool includeReviewer) => repo.SearchForUser(searchData, includeTitle, includeAbstract, includeReviewer);
        
        public static void FollowOn(int mainId)
        {
            var main = repo.GetMain(mainId);
            if (main != null && main.DocumentTypeEnum == DocumentTypeEnum.Conference && main.StatusEnum == StatusEnum.Completed)
            {
                main.Reviewers.ForEach(n => n.Cancel());
                main.StatusEnum = StatusEnum.New;
                main.Save();
            }
        }

        public static int? CreateRevision(int mainId) => GetMain(mainId)?.CreateRevision()?.MainId;

        public static void UpdateActivityDateToNow(int mainId)
        {
            repo.UpdateActivityDateToNow(mainId);
            ClearMainFromCache(mainId);
        }

        public static void CheckReviewsForChanges(int mainId) => GetMain(mainId)?.DoubleCheckReviews();
        #endregion

        #region Object Methods

        public void Save()
        {
            if (!Config.IsImport)
            {
                SetReviewStatus();
                if (!MainId.HasValue)
                {
                    CreateDate = DateTime.Now;
                    OwnerEmployeeId = OwnerEmployeeId ?? UserObject.CurrentUser.EmployeeId;
                    OwnerFirstName = OwnerFirstName ?? EmployeeCache.GetEmployee(OwnerEmployeeId, true)?.FirstName;
                    OwnerLastName = OwnerLastName ?? EmployeeCache.GetEmployee(OwnerEmployeeId, true)?.LastName;
                    Status = GetNewStatus();
                    // Revision will have set DocNumber and StiNumber so if they already exist, do not change
                    DocNumber = DocNumber ?? (DocumentTypeEnum == DocumentTypeEnum.DoeId ? repo.GetNextDoeDocNumber() : repo.GetNextDocNumber());
                    StiNumber = StiNumber ?? GenerateStiNumber();
                }
                else
                {
                    CheckStatusChange();
                }

                SetReviewStatus();
            }

            ActivityDate = DateTime.Now;
            repo.SaveMain(this);
            if (!Config.IsImport)
            {
                DoubleCheckReviews();
                ClearMainFromCache(MainId.Value);
            }
        }

        public void Delete(string reason)
        {
            Reviewers.ForEach(n => n.Cancel());
            repo.DeleteMain(this.MainId.Value, reason, UserObject.CurrentUser.EmployeeId);
            ClearMainFromCache(MainId.Value);
            Email.SendEmail(this, null, EmailTypeEnum.DeletedArtifact, reason);
            if (SortId.HasValue)
            {
                ApiRequest.RemoveArtifactFromSort(SortId.Value);
            }
        }

        public bool UserHasWriteAccess()
        {
            var user = UserObject.CurrentUser;

            return user.EmployeeId == OwnerEmployeeId ||
                   user.IsInAnyRole("Admin,ReleaseOfficial") ||
                   Authors.Exists(n => n.EmployeeId == user.EmployeeId) ||
                   Contacts.Exists(n => n.EmployeeId == user.EmployeeId) ;
        }

        public bool UserHasReadAccess(IDbConnection conn = null)
        {
            // Test this one piece at a time to try and make it faster
            var user = UserObject.CurrentUser;

            //Owner or Admin or Generic Release User
            if (user.EmployeeId == OwnerEmployeeId || user.IsInAnyRole("Admin,ReleaseOfficial,GenericReleaseUser")) return true;
            
            //Author
            var authors = AuthorObject.GetAuthors(MainId.Value, conn) ?? new List<AuthorObject>();
            if (authors.Exists(n => n.EmployeeId == user.EmployeeId)) return true;

            //Contact
            var contacts = ContactObject.GetContacts(MainId.Value, conn) ?? new List<ContactObject>();
            if (contacts.Exists(n => n.EmployeeId == user.EmployeeId)) return true;

            //Reviewers
            var reviewers = ReviewObject.GetReviews(MainId.Value, conn) ?? new List<ReviewObject>();
            if ((StatusEnum == StatusEnum.InPeerReview && (reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.PeerTechnical && n.ReviewerEmployeeId == user.EmployeeId))) ||
                (StatusEnum == StatusEnum.InReview && (reviewers.Exists(n => n.ReviewerEmployeeId == user.EmployeeId) || MemoryCache.GetGenericReviewUsers().Exists(n => n.EmployeeId == user.EmployeeId)))) return true;

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

        public bool CheckForMissingData()
        {
            MissingDatas = new List<MissingData>();
            try
            {
                if (StatusEnum != StatusEnum.Cancelled)
                {
                    if(string.IsNullOrWhiteSpace(Title)) MissingDatas.Add(new MissingData("Main Info", "Title is missing"));
                    if (string.IsNullOrWhiteSpace(DocumentType)) MissingDatas.Add(new MissingData("Main Info", "Document Type is missing"));
                    if(Ouo7 && string.IsNullOrWhiteSpace(Ouo7EmployeeId)) MissingDatas.Add(new MissingData("Main Info", "Derivative Classifier is missing"));
                    if (string.IsNullOrWhiteSpace(Abstract)) MissingDatas.Add(new MissingData("Abstract", "Description / Abstract Missing"));
                    if (Contacts != null && Contacts.Exists(n => !n.IsValid)) MissingDatas.Add(new MissingData("Contact(s)", "Contact with Invalid data exists"));
                    if (Authors == null || Authors.Count(n => n.IsPrimary) == 0) MissingDatas.Add(new MissingData("Author(s)", "Primary Author Missing"));
                    if (Authors != null && Authors.Exists(n => !n.IsValid)) MissingDatas.Add(new MissingData("Author(s)", "Author with Invalid data exists"));
                    if (Funding == null || Funding.Count == 0) MissingDatas.Add(new MissingData("Funding", "Funding Missing"));
                    if (Funding != null && Funding.Exists(n => !n.IsValid)) MissingDatas.Add(new MissingData("Funding", "Funding with Invalid data exists"));

                    if (MainId.HasValue)
                    {
                        var sub = MetaDataObject.GetMetaDatas(MainId.Value, MetaDataTypeEnum.SubjectCategories);
                        if (sub == null || sub.Count == 0) MissingDatas.Add(new MissingData("Subject", "Subject Missing"));

                        var orgs = MetaDataObject.GetMetaDatas(MainId.Value, MetaDataTypeEnum.SponsoringOrgs);
                        if (orgs == null || orgs.Count == 0) MissingDatas.Add(new MissingData("Sponsoring Org(s)", "Sponsor Missing"));

                        var keys = MetaDataObject.GetMetaDatas(MainId.Value, MetaDataTypeEnum.Keywords);
                        if (keys == null || keys.Count == 0) MissingDatas.Add(new MissingData("Keyword(s)", "Keyword Missing"));
                        
                    }
                }
            }
            catch{}

            return MissingDatas.Count > 0;
        }

        public void StartReviews(string reviewType)
        {
            if (UserHasWriteAccess())
            {
                switch (reviewType.ToEnum<ReviewerTypeEnum>())
                {
                    case ReviewerTypeEnum.PeerTechnical:
                        StartPeerReviews();
                        break;
                    case ReviewerTypeEnum.Manager:
                        StartManagerReviews();
                        break;
                }
            }
        }

        public void Cancel()
        {
            if (StatusEnum != StatusEnum.Cancelled && UserHasWriteAccess())
            {
                Reviewers.ForEach(n => n.Cancel());
                StatusEnum = StatusEnum.Cancelled;
                Save();
                if (SortId.HasValue)
                {
                    ApiRequest.RemoveArtifactFromSort(SortId.Value);
                }
            }
        }

        public void MarkReviewComplete()
        {
            if (StatusEnum == StatusEnum.ReviewNotRequired && UserHasWriteAccess())
            {
                StatusEnum = StatusEnum.Completed;
                Save();
            }
        }

        public void CheckStatus()
        {
            if(StatusEnum == StatusEnum.InPeerReview && !Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.PeerTechnical && n.ReviewStatus == ReviewStatusEnum.Active))
            {
                StatusEnum = StatusEnum.New;
                Save();
            }
        }

        public void ReStart()
        {
            if ((StatusEnum == StatusEnum.Rejected || StatusEnum == StatusEnum.Cancelled) && UserHasWriteAccess())
            {
                if (ReviewObject.MoveReviewsTohistory(MainId.Value))
                {
                    StatusEnum = StatusEnum.New;
                    Save();
                }
            }
        }

        public void ToggleRushFlag()
        {
            RushFlag = !RushFlag;
            Save();
        }

        public void MarkReviewComplete(int reviewId)
        {
            var review = Reviewers.FirstOrDefault(n => n.ReviewId == reviewId);
            if (review != null && (review.ReviewerEmployeeId == UserObject.CurrentUser.EmployeeId || review.SystemReviewerUsers.Exists(n => n.EmployeeId == UserObject.CurrentUser.EmployeeId)))
            {
                var oldstate = ReviewStateEnum;
                review.Complete(this);
                SetReviewStatus();
                CheckReviewStatus(oldstate);
                Save();
            }
        }

        public void MarkReviewApproved(int reviewId, bool? ouo3, bool? ouo7, string Ouo7EmployeeId)
        {
            var review = Reviewers.FirstOrDefault(n => n.ReviewId == reviewId);
            if (review != null && (review.ReviewerEmployeeId == UserObject.CurrentUser.EmployeeId || review.SystemReviewerUsers.Exists(n => n.EmployeeId == UserObject.CurrentUser.EmployeeId)))
            {
                if (review.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl)
                {
                    if (ouo3.HasValue)
                    {
                        if (this.Ouo3 != ouo3.Value)
                        {
                            this.Ouo3 = ouo3.Value;
                        }
                    }
                }
                else if (review.ReviewerTypeEnum == ReviewerTypeEnum.Classification)
                {
                    if (ouo7.HasValue)
                    {
                        if (this.Ouo7 != ouo7.Value)
                        {
                            this.Ouo7 = ouo7.Value;
                        }

                        if (!string.IsNullOrWhiteSpace(Ouo7EmployeeId) && this.Ouo7EmployeeId != Ouo7EmployeeId)
                        {
                            this.Ouo7EmployeeId = Ouo7EmployeeId;
                        }
                    }
                }
                var oldstate = ReviewStateEnum;
                review.Approve(this);
                SetReviewStatus();
                CheckReviewStatus(oldstate);
                Save();
            }
        }

        public void MarkReviewRejected(int reviewId, string reason)
        {
            var review = Reviewers.FirstOrDefault(n => n.ReviewId == reviewId);
            if (review != null && (review.ReviewerEmployeeId == UserObject.CurrentUser.EmployeeId || review.SystemReviewerUsers.Exists(n => n.EmployeeId == UserObject.CurrentUser.EmployeeId)))
            {
                review.Reject(reason, this);
                StatusEnum = StatusEnum.Rejected;
                Save();
                Reviewers.Where(n => n.ReviewStatus == ReviewStatusEnum.Active).ToList().ForEach(n => n.Cancel());
            }
        }

        public void MarkNotReviewing(int reviewId)
        {
            var review = Reviewers.FirstOrDefault(n => n.ReviewId == reviewId);
            if (review != null && (review.ReviewerEmployeeId == UserObject.CurrentUser.EmployeeId || review.SystemReviewerUsers.Exists(n => n.EmployeeId == UserObject.CurrentUser.EmployeeId)))
            {
                var oldstate = ReviewStateEnum;
                review.NotReviewing(this);
                SetReviewStatus();
                CheckReviewStatus(oldstate);
                Save();
            }
        }

        public MainObject CreateRevision()
        {
            if (!UserHasWriteAccess())
            {
                return null;
            }
            MainObject newMain = new MainObject();
            newMain.DocumentType = DocumentType;
            newMain.Title = Title;
            newMain.StiNumber = StiNumber;
            newMain.Abstract = Abstract;
            newMain.Comments = Comments;
            newMain.ContainsSciInfo = ContainsSciInfo;
            newMain.ContainsTechData = ContainsTechData;
            newMain.TechDataPublic = TechDataPublic;
            newMain.DocNumber = DocNumber;
            newMain.ConferenceName = ConferenceName;
            newMain.ConferenceSponsor = ConferenceSponsor;
            newMain.ConferenceLocation = ConferenceLocation;
            newMain.ConferenceBeginDate = ConferenceBeginDate;
            newMain.ConferenceEndDate = ConferenceEndDate;
            newMain.JournalName = JournalName;
            newMain.Revision = ++Revision;
            newMain.RelatedSti = RelatedSti;
            newMain.ParentId = MainId.Value;
            newMain.Ouo4 = Ouo4;
            newMain.Ouo3 = Ouo3;
            newMain.Ouo3b = Ouo3b;
            newMain.Ouo5 = Ouo5;
            newMain.Ouo6 = Ouo6;
            newMain.Ouo7 = Ouo7;
            newMain.Ouo7EmployeeId = Ouo7EmployeeId;
            newMain.Save();

            ChildId = newMain.MainId.Value;
            Save();

            IntellectualPropertyObject.CopyData(MainId.Value, newMain.MainId.Value);
            FundingObject.CopyData(MainId.Value, newMain.MainId.Value);
            ContactObject.CopyData(MainId.Value, newMain.MainId.Value);
            AuthorObject.CopyData(MainId.Value, newMain.MainId.Value);
            MetaDataObject.CopyData(MainId.Value, newMain.MainId.Value);

            return newMain;
        }
        #endregion

        #region Private Functions

        private void CheckStatusChange()
        {
            var old = repo.GetMain(MainId.Value);
            // Check to see if the document type changes
            if (old != null && old.DocumentTypeEnum != DocumentTypeEnum)
            {
                // Switching too or from DOE ID.  If so we need to generate a new document number
                if (old.DocumentTypeEnum == DocumentTypeEnum.DoeId || DocumentTypeEnum == DocumentTypeEnum.DoeId)
                {
                    DocNumber = (DocumentTypeEnum == DocumentTypeEnum.DoeId ? repo.GetNextDoeDocNumber() : repo.GetNextDocNumber());
                }

                // Switched from DOE ID or Internal to another type, and status is Review Not Required. Set status to new.
                if ((old.DocumentTypeEnum == DocumentTypeEnum.DoeId || old.DocumentTypeEnum == DocumentTypeEnum.Internal) &&
                    !(DocumentTypeEnum == DocumentTypeEnum.DoeId || DocumentTypeEnum == DocumentTypeEnum.Internal) &&
                    StatusEnum == StatusEnum.ReviewNotRequired)
                {
                    StatusEnum = StatusEnum.New;
                }

                // Switched from not DOE ID or Internal to DOE ID or Internal and status is new. Set to Review Not Required
                if (!(old.DocumentTypeEnum == DocumentTypeEnum.DoeId && old.DocumentTypeEnum == DocumentTypeEnum.Internal) &&
                    (DocumentTypeEnum == DocumentTypeEnum.DoeId || DocumentTypeEnum == DocumentTypeEnum.Internal) &&
                    StatusEnum == StatusEnum.New)
                {
                    StatusEnum = StatusEnum.ReviewNotRequired;
                }

                // Need to regenerate STI number
                StiNumber = GenerateStiNumber();
            }
        }

        private static void ClearMainFromCache(int mainId)
        {
            try
            {
                var cache = HttpContext.Current.Cache;
                var cacheKey = $"MainObj_{mainId}";
                cache.Remove(cacheKey);
            }
            catch { }
        }

        private string GetNewStatus()
        {
            if (DocumentTypeEnum == DocumentTypeEnum.DoeId || DocumentTypeEnum == DocumentTypeEnum.Internal)
            {
                return Business.StatusEnum.ReviewNotRequired.ToString();
            }

            return Business.StatusEnum.New.ToString();
        }

        private void CheckReviewStatus(ReviewerTypeEnum oldReviewState)
        {
            switch (oldReviewState)
            {
                case ReviewerTypeEnum.PeerTechnical:
                    if (!Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.PeerTechnical && (n.ReviewStatus == ReviewStatusEnum.Active || n.ReviewStatus == ReviewStatusEnum.New)))
                    {
                        StatusEnum = StatusEnum.New;
                        Save();
                    }
                    break;
                case ReviewerTypeEnum.Manager:
                    if (ReviewStateEnum != ReviewerTypeEnum.Manager && StatusEnum == StatusEnum.InReview) // We no longer have managers to review and we are still in the review process
                    {
                        // Make sure we don't have any rejections from the manager.
                        if (!Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Manager && n.ReviewStatus == ReviewStatusEnum.Rejected))
                        {
                            if (!Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Classification))
                            {
                                ReviewObject.CreateNew(this, ReviewerTypeEnum.Classification, ReviewStatusEnum.Active, true);
                            }

                            //if (ContainsSciInfo && ContainsTechData && !TechDataPublic)
                            //{
                                if (!Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl))
                                {
                                    ReviewObject.CreateNew(this, ReviewerTypeEnum.ExportControl, ReviewStatusEnum.Active, true);
                                }
                            //}

                            if (Intellectuals.Count > 0)
                            {
                                if (!Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment))
                                {
                                    ReviewObject.CreateNew(this, ReviewerTypeEnum.TechDeployment, ReviewStatusEnum.Active, true);
                                }
                            }
                        }

                    }
                    break;
                case ReviewerTypeEnum.ExportControl:
                case ReviewerTypeEnum.TechDeployment:
                case ReviewerTypeEnum.Classification:
                    if (!Reviewers.Exists(n => (n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl ||
                                                n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment ||
                                                n.ReviewerTypeEnum == ReviewerTypeEnum.Classification)
                                               && (n.ReviewStatus == ReviewStatusEnum.Active || n.ReviewStatus == ReviewStatusEnum.New)))
                    {
                        MarkComplete();
                    }
                    break;
            }
        }

        private void MarkComplete()
        {
            StatusEnum = StatusEnum.Completed;
            ApprovalDate = DateTime.Now;
            Save();
            Email.SendEmail(this, null, EmailTypeEnum.Complete, null);
            Email.SendEmail(this, null, EmailTypeEnum.ReleaseOfficerComplete, null);
            UploadToSort();
        }

        public string UploadToSort()
        {
            string msg = null;
            if (StatusEnum == StatusEnum.Completed && ContainsSciInfo && DocumentTypeEnum != DocumentTypeEnum.DoeId)
            {
                var response = ApiRequest.UploadArtifactToSort(ArtifactData.GenerateArtifact(this));
                if (response.Success && response.SortId.HasValue)
                {
                    SortId = response.SortId;
                    repo.SetSortId(MainId.Value, response.SortId.Value);
                }
                else
                {
                    msg = response.ErrorMessage;
                    repo.SetSortError(MainId.Value, response.ErrorMessage);
                }
            }
            else
            {
                msg = $"This artifact does not match the criteria to send to SORT.  All reviews need to be completed, and it must be marked as containing scientific information.";
            }

            return msg;
        }

        private void StartPeerReviews()
        {
            if (UserHasWriteAccess())
            {
                if (StatusEnum == StatusEnum.New)
                {
                    if (Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.PeerTechnical && n.ReviewStatus == ReviewStatusEnum.New))
                    {
                        foreach (var reviewer in Reviewers.Where(n => n.ReviewerTypeEnum == ReviewerTypeEnum.PeerTechnical && n.ReviewStatus == ReviewStatusEnum.New))
                        {
                            reviewer.Activate(this);
                        }
                        StatusEnum = StatusEnum.InPeerReview;
                        Save();
                    }
                }
            }
        }

        private void StartManagerReviews()
        {
            if (UserHasWriteAccess())
            {
                if (StatusEnum == StatusEnum.New || StatusEnum == StatusEnum.InPeerReview)
                {
                    if (!Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.PeerTechnical && n.ReviewStatus == ReviewStatusEnum.New) &&
                        Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Manager && n.ReviewStatus == ReviewStatusEnum.New))
                    {
                        StatusEnum = StatusEnum.InReview;
                        foreach (var reviewer in Reviewers.Where(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Manager && n.ReviewStatus == ReviewStatusEnum.New))
                        {
                            reviewer.Activate(this);
                        }
                        Save();
                    }
                }
            }
        }

        private string GenerateStiNumber()
        {
            if (DocumentTypeEnum == DocumentTypeEnum.DoeId)
            {
                return $"DOE/ID-{DocNumber:D5}";
            }

            return $"INL/{DocumentTypeEnum.GetEnumShortName()}-{CreateDate:yy}-{DocNumber:D5}";
        }

        private void DoubleCheckReviews()
        {
            bool change = false;
            if (ReviewStateEnum == ReviewerTypeEnum.Classification || ReviewStateEnum == ReviewerTypeEnum.ExportControl || ReviewStateEnum == ReviewerTypeEnum.TechDeployment)
            {
                if (!Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Manager && n.ReviewStatus == ReviewStatusEnum.Rejected))
                {
                    if (!Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Classification))
                    {
                        ReviewObject.CreateNew(this, ReviewerTypeEnum.Classification, ReviewStatusEnum.Active, true);
                        change = true;
                    }

                    //if (ContainsSciInfo && ContainsTechData && !TechDataPublic)
                    //{
                        if (!Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl))
                        {
                            ReviewObject.CreateNew(this, ReviewerTypeEnum.ExportControl, ReviewStatusEnum.Active, true);
                            change = true;
                        }
                    //}
                    //else if (Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl && n.ReviewStatus == ReviewStatusEnum.Active))
                    //{
                    //    // No longer is required because the data changed, so cancel this review.
                    //    foreach (var rev in Reviewers.Where(n => n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl && n.ReviewStatus == ReviewStatusEnum.Active))
                    //    {
                    //        rev.Cancel();
                    //    }
                    //    change = true;
                    //}

                    if (Intellectuals.Count > 0)
                    {
                        // Need to add in the Tech Review since there is now Intellectual Property and there was not a review for it before.
                        if (!Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment))
                        {
                            ReviewObject.CreateNew(this, ReviewerTypeEnum.TechDeployment, ReviewStatusEnum.Active, true);
                            change = true;
                        }
                    }
                    else if (Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment && n.ReviewStatus == ReviewStatusEnum.Active))
                    {
                        // No longer is required because there is not Intellectual Property, so cancel this review.
                        foreach (var rev in Reviewers.Where(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment && n.ReviewStatus == ReviewStatusEnum.Active))
                        {
                            rev.Cancel();
                        }
                        change = true;
                    }
                }
            }

            if (change)
            {
                CheckReviewStatus(ReviewerTypeEnum.Classification);
            }
        }

        public void SetReviewStatus()
        {
            SetReviewState();
            switch (StatusEnum)
            {
                case StatusEnum.InPeerReview:
                    ReviewStatus = "Peer Review";
                    break;
                case StatusEnum.InReview:
                    switch (ReviewStateEnum)
                    {
                        case ReviewerTypeEnum.Manager:
                            ReviewStatus = "Manager/Line Manager Review";
                            break;
                        case ReviewerTypeEnum.Classification:
                        case ReviewerTypeEnum.ExportControl:
                        case ReviewerTypeEnum.TechDeployment:
                            if (Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl && n.ReviewStatus == ReviewStatusEnum.Active) &&
                                MemoryCache.GetGenericReviewUsers(ReviewerTypeEnum.ExportControl).Exists(n => n.EmployeeId == UserObject.CurrentUser.EmployeeId))
                            {
                                ReviewStatus = "Export Compliance Review";
                            }
                            else if (Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment && n.ReviewStatus == ReviewStatusEnum.Active) &&
                                    MemoryCache.GetGenericReviewUsers(ReviewerTypeEnum.TechDeployment).Exists(n => n.EmployeeId == UserObject.CurrentUser.EmployeeId))
                            {
                                ReviewStatus = "Technical Deployment Review";
                            }
                            else if (Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Classification && n.ReviewStatus == ReviewStatusEnum.Active) &&
                                    MemoryCache.GetGenericReviewUsers(ReviewerTypeEnum.Classification).Exists(n => n.EmployeeId == UserObject.CurrentUser.EmployeeId))
                            {
                                ReviewStatus = "Classification Review";
                            }
                            else
                            {
                                ReviewStatus = $"{ReviewStateEnum.ToString()} Review";
                            }

                            break;
                        default:
                            ReviewStatus = "Complete";
                            break;
                    }
                    break;
                case StatusEnum.Cancelled:
                    ReviewStatus = "Cancelled";
                    break;
                case StatusEnum.Completed:
                case StatusEnum.Rejected:
                    ReviewStatus = "Complete";
                    break;
                default:
                    ReviewStatus = "Unreviewed";
                    break;
            }

            SetReviewPercent();
        }

        private void SetReviewState()
        {
            if (StatusEnum == StatusEnum.InPeerReview)
            {
                ReviewStateEnum = ReviewerTypeEnum.PeerTechnical;
            }
            else if (StatusEnum == StatusEnum.InReview)
            {
                if (Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Manager && n.ReviewStatus == ReviewStatusEnum.Active))
                {
                    ReviewStateEnum = ReviewerTypeEnum.Manager;
                }
                else if (Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.Classification && n.ReviewStatus == ReviewStatusEnum.Active))
                {
                    ReviewStateEnum = ReviewerTypeEnum.Classification;
                }
                else if (Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl && n.ReviewStatus == ReviewStatusEnum.Active))
                {
                    ReviewStateEnum = ReviewerTypeEnum.ExportControl;
                }
                else if (Reviewers.Exists(n => n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment && n.ReviewStatus == ReviewStatusEnum.Active))
                {
                    ReviewStateEnum = ReviewerTypeEnum.TechDeployment;
                }
                else
                {
                    ReviewStateEnum = ReviewerTypeEnum.None;
                }
            }
            else
            {
                ReviewStateEnum = ReviewerTypeEnum.None;
            }
        }

        private void SetReviewPercent()
        {
            if (StatusEnum == StatusEnum.InReview)
            {
                double numReviews = 2.0; //Has at least 2 parts to the review
                double reviewAt = 0.0;
                //if (ContainsSciInfo && ContainsTechData && !TechDataPublic)
                //{
                    numReviews++; //Export Compliance review required
                //}

                if (Intellectuals.Count > 0)
                {
                    numReviews++; //Technical Deployment review required
                }

                switch (ReviewStateEnum)
                {
                    case ReviewerTypeEnum.Manager:
                        reviewAt = 0.0;
                        break;
                    case ReviewerTypeEnum.Classification:
                    case ReviewerTypeEnum.ExportControl:
                    case ReviewerTypeEnum.TechDeployment:
                        reviewAt = 1 + Reviewers.Count(n => (n.ReviewerTypeEnum == ReviewerTypeEnum.Classification ||
                                                             n.ReviewerTypeEnum == ReviewerTypeEnum.ExportControl ||
                                                             n.ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment) &&
                                                            n.ReviewStatus == ReviewStatusEnum.Approved);
                        break;
                    default:
                        reviewAt = numReviews;
                        break;
                }

                double percent = (reviewAt / numReviews) * 100;

                ReviewPercent = percent;
                return;
            }
            if (StatusEnum == StatusEnum.Completed)
            {
                ReviewPercent = 100.0;
                return;
            }

            ReviewPercent = 0.0;
        }
        #endregion
    }

    public interface IMainRepository
    {
        List<CountData> GetCounts(int viewTime);
        List<MainObject> GetAllMains(ViewModeOptionEnum viewMode, int viewTime);
        List<MainObject> GetAllForUser(ViewModeOptionEnum viewMode, int viewTime, string employeeId);
        List<MainObject> GetAllForOrg(ViewModeOptionEnum viewMode, int viewTime, string employeeId, string orgOption);
        List<MainObject> GetReviewsForUser(string employeeId);
        List<MainObject> SearchForUser(string searchData, bool includeTitle, bool includeAbstract, bool includeReviewer);
        int GetAllMainsCount(ViewModeOptionEnum viewMode, int viewTime);
        MainObject GetMain(int mainId, IDbConnection conn = null);
        MainObject SaveMain(MainObject main);
        bool DeleteMain(int mainId, string reason, string employeeId);
        int GetAllForUserCount(ViewModeOptionEnum viewMode, int viewTime, string employeeId);
        int GetAllForOrgCount(ViewModeOptionEnum viewMode, int viewTime, string employeeId, string orgOption);
        int GetNextDocNumber();
        int GetNextDoeDocNumber();
        void UpdateActivityDateToNow(int mainId);

        void SetSortId(int mainId, int sortId);
        void SetSortError(int mainId, string message);
    }

    public class MainRepository : IMainRepository
    {
        public MainObject GetMain(int mainId, IDbConnection conn) => (conn ?? Config.Conn).Query<MainObject>("SELECT * FROM dat_Main WHERE MainId = @MainId", new { MainId = mainId }).FirstOrDefault();

        public int GetNextDocNumber() => Config.Conn.QueryFirst<int>("SELECT ISNULL(max(DocNumber), 50999) + 1 from dat_Main WHERE DocumentType != 'DoeId'");

        public int GetNextDoeDocNumber() => Config.Conn.QueryFirst<int>("SELECT ISNULL(max(DocNumber), 11999) + 1 from dat_Main WHERE DocumentType = 'DoeId'");

        public void SetSortId(int mainId, int sortId) => Config.Conn.Execute("UPDATE dat_Main SET SortId = @SortId, SortError = null WHERE MainId = @MainId", new {MainId = mainId, SortId = sortId});

        public void SetSortError(int mainId, string message) => Config.Conn.Execute("UPDATE dat_Main SET SortError = @SortError, SortId = null WHERE MainId = @MainId", new { MainId = mainId, SortError = message });

        public List<MainObject> GetAllForUser(ViewModeOptionEnum viewMode, int viewTime, string employeeId)
        {
            DateTime targetDate = DateTime.Now.AddMonths((viewTime * -1));
            string vMode1 = string.Empty;
            string vMode2 = string.Empty;
            string vMode3 = string.Empty;
            string sql = string.Empty;
            if (viewMode == ViewModeOptionEnum.All)
            {
                sql = @"select distinct m.* 
                            From dat_Main m
                            where m.OwnerEmployeeId = @EmployeeId
                            and m.Status not in ('Cancelled')
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Author a on a.MainId = m.MainId
	                            and a.EmployeeId = @EmployeeId
                            where m.Status not in ('Cancelled')
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Contact c on c.MainId = m.MainId
	                            and c.EmployeeId = @EmployeeId
                            where m.Status not in ('Cancelled')
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        order by m.MainId";
            }
            else
            {
                switch (viewMode)
                {
                    case ViewModeOptionEnum.Cancelled:
                        vMode1 = "Cancelled";
                        break;
                    case ViewModeOptionEnum.Rejected:
                        vMode1 = "Rejected";
                        break;
                    case ViewModeOptionEnum.Complete:
                        vMode1 = "Completed";
                        vMode2 = "ReviewNotRequired";
                        break;
                    case ViewModeOptionEnum.UnderReview:
                        vMode1 = "InReview";
                        vMode2 = "InPeerReview";
                        break;
                    case ViewModeOptionEnum.InProcess:
                        vMode1 = "New";
                        vMode2 = "Rejected";
                        break;
                }

                sql = @"select distinct m.* 
                            From dat_Main m
                            where m.OwnerEmployeeId = @EmployeeId
                            and m.Status in (@VMode1, @VMode2, @VMode3)
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Author a on a.MainId = m.MainId
	                            and a.EmployeeId = @EmployeeId
                            where m.Status in (@VMode1, @VMode2, @VMode3)
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Contact c on c.MainId = m.MainId
	                            and c.EmployeeId = @EmployeeId
                            where m.Status in (@VMode1, @VMode2, @VMode3)
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        order by m.MainId";
            }

            return Config.Conn.Query<MainObject>(sql, new { EmployeeId = employeeId, VMode1 = vMode1, VMode2 = vMode2, VMode3 = vMode3, TargetDate = targetDate }).ToList();
        }

        public List<MainObject> SearchForUser(string searchData, bool includeTitle, bool includeAbstract, bool includeReviewer)
        {
            string sql = @" select distinct m.*
                            from dat_Main m
                            left join dat_Author a on a.MainId = m.MainId
                            left join dat_Contact c on c.MainId = m.MainId";

            if (includeReviewer)
            {
                sql += @"   left join dat_Review r on r.MainId = m.MainId";
            }

            sql += @"       where m.StiNumber like '%' + @SearchData + '%'
                            or a.[Name] like '%' + @SearchData + '%'
                            or c.[Name] like '%' + @SearchData + '%'
                            or m.OwnerFirstName like '%' + @SearchData + '%'
                            or m.OwnerLastName like '%' + @SearchData + '%'";

            if (includeReviewer)
            {
                sql += @"  or r.ReviewerFirstName like '%' + @SearchData + '%'
                           or r.ReviewerLastName like '%' + @SearchData + '%'";
            }

            if (includeTitle)
            {
                sql += " or m.Title like '%' + @SearchData + '%'";
            }

            if (includeAbstract)
            {
                sql += "or m.Abstract like '%' + @SearchData + '%'";
            }

            return Config.Conn.Query<MainObject>(sql, new { SearchData = searchData }).ToList();
        }

        public int GetAllForUserCount(ViewModeOptionEnum viewMode, int viewTime, string employeeId)
        {
            DateTime targetDate = DateTime.Now.AddMonths((viewTime * -1));
            string vMode1 = string.Empty;
            string vMode2 = string.Empty;
            string vMode3 = string.Empty;
            string sql = string.Empty;
            if (viewMode == ViewModeOptionEnum.All)
            {
                sql = @"select count(*) from (
                        select distinct m.* 
                            From dat_Main m
                            where m.OwnerEmployeeId = @EmployeeId
                            and m.Status not in ('Cancelled')
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Author a on a.MainId = m.MainId
	                            and a.EmployeeId = @EmployeeId
                            where m.Status not in ('Cancelled')
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Contact c on c.MainId = m.MainId
	                            and c.EmployeeId = @EmployeeId
                            where m.Status not in ('Cancelled')
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        ) as dt";
            }
            else
            {
                switch (viewMode)
                {
                    case ViewModeOptionEnum.Cancelled:
                        vMode1 = "Cancelled";
                        break;
                    case ViewModeOptionEnum.Rejected:
                        vMode1 = "Rejected";
                        break;
                    case ViewModeOptionEnum.Complete:
                        vMode1 = "Completed";
                        vMode2 = "ReviewNotRequired";
                        break;
                    case ViewModeOptionEnum.UnderReview:
                        vMode1 = "InReview";
                        vMode2 = "InPeerReview";
                        break;
                    case ViewModeOptionEnum.InProcess:
                        vMode1 = "New";
                        vMode2 = "Rejected";
                        break;
                }

                sql = @"select count(*) from (
                        select distinct m.* 
                            From dat_Main m
                            where m.OwnerEmployeeId = @EmployeeId
                            and m.Status in (@VMode1, @VMode2, @VMode3)
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Author a on a.MainId = m.MainId
	                            and a.EmployeeId = @EmployeeId
                            where m.Status in (@VMode1, @VMode2, @VMode3)
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Contact c on c.MainId = m.MainId
	                            and c.EmployeeId = @EmployeeId
                            where m.Status in (@VMode1, @VMode2, @VMode3)
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        ) as dt";
            }

            return Config.Conn.QueryFirstOrDefault<int>(sql, new { EmployeeId = employeeId, VMode1 = vMode1, VMode2 = vMode2, VMode3 = vMode3, TargetDate = targetDate });
        }

        public List<MainObject> GetAllForOrg(ViewModeOptionEnum viewMode, int viewTime, string employeeId, string orgOption)
        {
            var employee = EmployeeCache.GetEmployee(employeeId);

            DateTime targetDate = DateTime.Now.AddMonths((viewTime * -1));
            string vMode1 = string.Empty;
            string vMode2 = string.Empty;
            string vMode3 = string.Empty;
            string sql = string.Empty;
            if (viewMode == ViewModeOptionEnum.All)
            {
                sql = @"select distinct m.* 
                            From dat_Main m
	                        inner join dat_UserOrg o on o.EmployeeId = m.OwnerEmployeeId
                                and o.Org = @WorkOrg
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where m.OwnerEmployeeId = @EmployeeId 
                            AND m.Status not in ('Cancelled')
	                        AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Author a on a.MainId = m.MainId
	                        inner join dat_UserOrg o on o.Org = a.WorkOrg
		                        and o.EmployeeId = @EmployeeId
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where m.Status not in ('Cancelled')
	                        AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Contact c on c.MainId = m.MainId
	                        inner join dat_UserOrg o on o.Org = c.WorkOrg
		                        and o.EmployeeId = @EmployeeId
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where m.Status not in ('Cancelled')
	                        AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        order by m.MainId";
            }
            else
            {
                switch (viewMode)
                {
                    case ViewModeOptionEnum.Cancelled:
                        vMode1 = "Cancelled";
                        break;
                    case ViewModeOptionEnum.Rejected:
                        vMode1 = "Rejected";
                        break;
                    case ViewModeOptionEnum.Complete:
                        vMode1 = "Completed";
                        vMode2 = "ReviewNotRequired";
                        break;
                    case ViewModeOptionEnum.UnderReview:
                        vMode1 = "InReview";
                        vMode2 = "InPeerReview";
                        break;
                    case ViewModeOptionEnum.InProcess:
                        vMode1 = "New";
                        vMode2 = "Rejected";
                        break;
                }

                sql = @"select distinct m.* 
                            From dat_Main m
	                        inner join dat_UserOrg o on o.EmployeeID = m.OwnerEmployeeId
                                and o.Org = @WorkOrg
		                        and o.EmployeeId = @EmployeeId
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where m.OwnerEmployeeId = @EmployeeId 
                            AND m.Status in (@VMode1, @VMode2, @VMode3)
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Author a on a.MainId = m.MainId
	                        inner join dat_UserOrg o on o.Org = a.WorkOrg
		                        and o.EmployeeId = @EmployeeId
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where m.Status in (@VMode1, @VMode2, @VMode3)
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Contact c on c.MainId = m.MainId
	                        inner join dat_UserOrg o on o.Org = c.WorkOrg
		                        and o.EmployeeId = @EmployeeId
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where m.Status in (@VMode1, @VMode2, @VMode3)
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        order by m.MainId";
            }

            return Config.Conn.Query<MainObject>(sql, new { EmployeeId = employeeId, WorkOrg = employee.WorkOrginization, VMode1 = vMode1, VMode2 = vMode2, VMode3 = vMode3, TargetDate = targetDate, OrgOption = orgOption }).ToList();
        }

        public int GetAllForOrgCount(ViewModeOptionEnum viewMode, int viewTime, string employeeId, string orgOption)
        {
            var employee = EmployeeCache.GetEmployee(employeeId);

            DateTime targetDate = DateTime.Now.AddMonths((viewTime * -1));
            string vMode1 = string.Empty;
            string vMode2 = string.Empty;
            string vMode3 = string.Empty;
            string sql = string.Empty;
            if (viewMode == ViewModeOptionEnum.All)
            {
                sql = @"select count(*) from (
                        select distinct m.* 
                            From dat_Main m
	                        inner join dat_UserOrg o on on o.EmployeeId = m.OwnerEmployeeId
                                and o.Org = @WorkOrg
                                and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where m.OwnerEmployeeId = @EmployeeId
                            AND m.Status not in ('Cancelled')
	                        AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Author a on a.MainId = m.MainId
	                        inner join dat_UserOrg o on o.Org = a.WorkOrg
		                        and o.EmployeeId = @EmployeeId
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where m.Status not in ('Cancelled')
	                        AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Contact c on c.MainId = m.MainId
	                        inner join dat_UserOrg o on o.Org = c.WorkOrg
		                        and o.EmployeeId = @EmployeeId
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where m.Status not in ('Cancelled')
	                        AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        ) as dt";
            }
            else
            {
                switch (viewMode)
                {
                    case ViewModeOptionEnum.Cancelled:
                        vMode1 = "Cancelled";
                        break;
                    case ViewModeOptionEnum.Rejected:
                        vMode1 = "Rejected";
                        break;
                    case ViewModeOptionEnum.Complete:
                        vMode1 = "Completed";
                        vMode2 = "ReviewNotRequired";
                        break;
                    case ViewModeOptionEnum.UnderReview:
                        vMode1 = "InReview";
                        vMode2 = "InPeerReview";
                        break;
                    case ViewModeOptionEnum.InProcess:
                        vMode1 = "New";
                        vMode2 = "Rejected";
                        break;
                }

                sql = @"select count(*) from (
                        select distinct m.* 
                            From dat_Main m
	                        inner join dat_UserOrg o on o.EmployeeId = m.OwnerEmployeeId
                                and o.Org = @WorkOrg
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where m.OwnerEmployeeId = @EmployeeId
                            AND m.Status in (@VMode1, @VMode2, @VMode3)
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Author a on a.MainId = m.MainId
	                        inner join dat_UserOrg o on o.Org = a.WorkOrg
		                        and o.EmployeeId = @EmployeeId
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where m.Status in (@VMode1, @VMode2, @VMode3)
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        union
                            select m.*
                            from dat_Main m
                            inner join dat_Contact c on c.MainId = m.MainId
	                        inner join dat_UserOrg o on o.Org = c.WorkOrg
		                        and o.EmployeeId = @EmployeeId
		                        and (o.Org = @OrgOption or @OrgOption is null or @OrgOption = '')
                            where m.Status in (@VMode1, @VMode2, @VMode3)
                            AND m.Deleted = 0
                            AND (m.CreateDate >= @TargetDate OR m.ActivityDate >= @TargetDate)
                        ) as dt";
            }

            return Config.Conn.QueryFirstOrDefault<int>(sql, new { EmployeeId = employeeId, WorkOrg = employee.WorkOrginization, VMode1 = vMode1, VMode2 = vMode2, VMode3 = vMode3, TargetDate = targetDate, OrgOption = orgOption });
        }

        public List<MainObject> GetReviewsForUser(string employeeId)
        {
            string sql = @" select distinct m.*
                            from dat_Main m
                            inner join dat_Review r on r.MainId = m.MainId
	                            and r.Status = 'Active'
	                            and r.ReviewerType in ('Manager', 'PeerTechnical')
	                            and r.ReviewerEmployeeId = @EmployeeId
                            where m.Status in ('InPeerReview', 'InReview')
                            AND m.Deleted = 0
                            union
                            select distinct m.*
                            from dat_Main m
                            inner join dat_Review r on r.MainId = m.MainId
	                            and r.Status = 'Active'
	                            and r.ReviewerType in ('Classification', 'ExportControl', 'TechDeployment')
                            inner join lu_GenericReviewUser ru on ru.ReviewerType = r.ReviewerType
	                            and ru.EmployeeId = @EmployeeId
                            where m.Status in ('InPeerReview', 'InReview')
                            AND m.Deleted = 0";

            return Config.Conn.Query<MainObject>(sql, new { EmployeeId = employeeId }).ToList();
        }

        public List<MainObject> GetAllMains(ViewModeOptionEnum viewMode, int viewTime)
        {
            string sql = string.Empty;
            DateTime targetDate = DateTime.Now.AddMonths((viewTime * -1));
            switch (viewMode)
            {
                case ViewModeOptionEnum.Cancelled:
                    sql = @"SELECT * FROM dat_Main WHERE Status = 'Cancelled' AND Deleted = 0 AND (CreateDate >= @TargetDate OR ActivityDate >= @TargetDate)";
                    break;
                case ViewModeOptionEnum.Complete:
                    sql = @"SELECT * FROM dat_Main WHERE Status in ('Completed', 'ReviewNotRequired') AND Deleted = 0 AND (CreateDate >= @TargetDate OR ActivityDate >= @TargetDate)";
                    break;
                case ViewModeOptionEnum.InProcess:
                    sql = @"SELECT * FROM dat_Main WHERE Status in ('New') AND Deleted = 0 AND (CreateDate >= @TargetDate OR ActivityDate >= @TargetDate)";
                    break;
                case ViewModeOptionEnum.UnderReview:
                    sql = @"SELECT * FROM dat_Main WHERE Status in ('InReview', 'InPeerReview') AND Deleted = 0 AND (CreateDate >= @TargetDate OR ActivityDate >= @TargetDate)";
                    break;
                case ViewModeOptionEnum.Rejected:
                    sql = @"SELECT * FROM dat_Main WHERE Status = 'Rejected' AND Deleted = 0 AND (CreateDate >= @TargetDate OR ActivityDate >= @TargetDate)";
                    break;
                default:
                    sql = @"SELECT * FROM dat_Main WHERE Status not in ('Cancelled') AND Deleted = 0 AND (CreateDate >= @TargetDate OR ActivityDate >= @TargetDate)";
                    break;
            }

            return Config.Conn.Query<MainObject>(sql, new {TargetDate = targetDate}).ToList();
        } 

        public int GetAllMainsCount(ViewModeOptionEnum viewMode, int viewTime)
        {
            string sql = string.Empty;
            DateTime targetDate = DateTime.Now.AddMonths((viewTime * -1));
            switch (viewMode)
            {
                case ViewModeOptionEnum.Cancelled:
                    sql = @"SELECT Count(*) FROM dat_Main WHERE Status = 'Cancelled' AND Deleted = 0 AND (CreateDate >= @TargetDate OR ActivityDate >= @TargetDate)";
                    break;
                case ViewModeOptionEnum.Complete:
                    sql = @"SELECT Count(*) FROM dat_Main WHERE Status in ('Completed', 'ReviewNotRequired') AND Deleted = 0 AND (CreateDate >= @TargetDate OR ActivityDate >= @TargetDate)";
                    break;
                case ViewModeOptionEnum.InProcess:
                    sql = @"SELECT Count(*) FROM dat_Main WHERE Status in ('New') AND Deleted = 0 AND (CreateDate >= @TargetDate OR ActivityDate >= @TargetDate)";
                    break;
                case ViewModeOptionEnum.UnderReview:
                    sql = @"SELECT Count(*) FROM dat_Main WHERE Status in ('InReview', 'InPeerReview') AND Deleted = 0 AND (CreateDate >= @TargetDate OR ActivityDate >= @TargetDate)";
                    break;
                case ViewModeOptionEnum.Rejected:
                    sql = @"SELECT Count(*) FROM dat_Main WHERE Status = 'Rejected' AND Deleted = 0 AND (CreateDate >= @TargetDate OR ActivityDate >= @TargetDate)";
                    break;
                default:
                    sql = @"SELECT Count(*) FROM dat_Main WHERE Status not in ('Cancelled') AND Deleted = 0 AND (CreateDate >= @TargetDate OR ActivityDate >= @TargetDate)";
                    break;
            }

            return Config.Conn.QueryFirstOrDefault<int>(sql, new { TargetDate = targetDate });
        }

        public MainObject SaveMain(MainObject main)
        {
            if (main.MainId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_Main
                    SET     DocumentType = @DocumentType,
                            StiNumber = @StiNumber,
                            Title = @Title,
                            Status = @Status,
                            Abstract = @Abstract,                            
                            Comments = @Comments,
                            ContainsSciInfo = @ContainsSciInfo,
                            ContainsTechData = @ContainsTechData,
                            TechDataPublic = @TechDataPublic,
                            NeedByDate = @NeedByDate,
                            ActivityDate = @ActivityDate,
                            ConferenceName = @ConferenceName,
                            ConferenceSponsor = @ConferenceSponsor,
                            ConferenceLocation = @ConferenceLocation,
                            ConferenceBeginDate = @ConferenceBeginDate,
                            ConferenceEndDate = @ConferenceEndDate,
                            JournalName = @JournalName,
                            RelatedSti = @RelatedSti,
                            ChildId = @ChildId,
                            ApprovalDate = @ApprovalDate,
                            ReviewStatus = @ReviewStatus, 
                            ReviewState = @ReviewState,
                            ReviewPercent = @ReviewPercent,
                            Ouo3 = @Ouo3,
                            Ouo3b = @Ouo3b,
                            Ouo4 = @Ouo4,
                            Ouo5 = @Ouo5,
                            Ouo6 = @Ouo6,
                            Ouo7 = @Ouo7,
                            Ouo7EmployeeId = @Ouo7EmployeeId,
                            OstiId = @OstiId,
                            DateOsti = @DateOsti,
                            RushFlag = @RushFlag,
                            LimitedExp = @LimitedExp,
                            OwnerFirstName = @OwnerFirstName,
                            OwnerLastName = @OwnerLastName
                    WHERE   MainId = @MainId";
                Config.Conn.Execute(sql, main);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_Main (
                        OwnerEmployeeId,
                        DocumentType,
                        Title,
                        Status,
                        StiNumber,
                        Revision,
                        Abstract,
                        CreateDate,
                        Comments,
                        ContainsSciInfo,
                        ContainsTechData,
                        TechDataPublic,
                        NeedByDate,
                        ActivityDate,
                        DocNumber,
                        ConferenceName,
                        ConferenceSponsor,
                        ConferenceLocation,
                        ConferenceBeginDate,
                        ConferenceEndDate,
                        JournalName,
                        RelatedSti,
                        ParentId,
                        ReviewStatus, 
                        ReviewState,
                        ReviewPercent,
                        Ouo3,
                        Ouo3b,
                        Ouo4,
                        Ouo5,
                        Ouo6,
                        Ouo7,
                        Ouo7EmployeeId,
                        OldLrsId,
                        OstiId,
                        DateOsti,
                        RushFlag,
                        LimitedExp,
                        OwnerFirstName,
                        OwnerLastName
                    )
                    VALUES (
                        @OwnerEmployeeId,
                        @DocumentType,
                        @Title,
                        @Status,
                        @StiNumber,
                        @Revision,
                        @Abstract,
                        @CreateDate,                        
                        @Comments,
                        @ContainsSciInfo,
                        @ContainsTechData,
                        @TechDataPublic,
                        @NeedByDate,
                        @ActivityDate,
                        @DocNumber,
                        @ConferenceName,
                        @ConferenceSponsor,
                        @ConferenceLocation,
                        @ConferenceBeginDate,
                        @ConferenceEndDate,
                        @JournalName,
                        @RelatedSti,
                        @ParentId,
                        @ReviewStatus, 
                        @ReviewState,
                        @ReviewPercent,
                        @Ouo3,
                        @Ouo3b,
                        @Ouo4,
                        @Ouo5,
                        @Ouo6,
                        @Ouo7,
                        @Ouo7EmployeeId,
                        @OldLrsId,
                        @OstiId,
                        @DateOsti,
                        @RushFlag,
                        @LimitedExp,
                        @OwnerFirstName,
                        @OwnerLastName
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                main.MainId = Config.Conn.Query<int>(sql, main).Single();
            }
            return main;
        }

        public List<CountData> GetCounts(int viewTime)
        {
            DateTime targetDate = DateTime.Now.AddMonths((viewTime * -1));
            string sql = @"select [Status], 
                                  COUNT(*) as NumItems
                            From dat_Main
                            WHERE Deleted = 0
                            AND (CreateDate >= @TargetDate OR ActivityDate >= @TargetDate)
                            group by [Status]";
            return Config.Conn.Query<CountData>(sql, new { TargetDate = targetDate }).ToList();
        }

        public bool DeleteMain(int mainId, string reason, string employeeId)
        {
            try
            {
                Config.Conn.Execute("UPDATE dat_Main SET Deleted = 1, Status = @Status, DeleteComment = @Comment, DeleteEmployeeId = @EmployeeId WHERE MainId = @MainId", new { Comment = reason, Status = StatusEnum.Deleted.ToString(), EmployeeId = employeeId, MainId = mainId });
            }
            catch { return false; }
            return true;
        }

        public void UpdateActivityDateToNow(int mainId) => Config.Conn.Execute("UPDATE dat_Main SET ActivityDate = @ModDate WHERE MainId = @MainId", new { ModDate = DateTime.Now, MainId = mainId });
    }

    public class CountData
    {
        public string Status { get; set; }
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
}