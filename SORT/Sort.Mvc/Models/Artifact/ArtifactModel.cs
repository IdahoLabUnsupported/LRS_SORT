using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inl.MvcHelper;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class ArtifactModel : IContact, IAuthor, IFunding
    {
        #region Properties
        public int SortMainId { get; set; }
        public SortMainObject Sort { get; set; }
        //Contact Start
        [Display(Name = "Contact Name")]
        public string EmployeeId { get; set; }
        [Display(Name = "Contact Type")]
        public string ContactType { get; set; }
        public int? ContactId { get; set; }
        //Contact End

        //Author Start
        [Display(Name = "Company Affiliation")]
        public AffiliationEnum AffiliationType { get; set; }
        [Display(Name = "Affiliation")]
        public string Affiliation { get; set; }
        [Display(Name = "Employee")]
        public string AffiliateEmployeeId { get; set; }
        [Display(Name = "Orcid ID")]
        public string AuthorOrcidId { get; set; }
        [Display(Name = "Author Name")]
        public string AuthorName { get; set; }
        public bool IsPrimary { get; set; }
        public int? AuthorId { get; set; }
        [Display(Name = "Affiliation State")]
        public int? StateId { get; set; }


        //Author End

        //Funding Start
        public int? FundingId { get; set; }
        [Display(Name = "Funding Source")]
        public int FundingTypeId { get; set; }
        [Display(Name = "DOE Program")]
        public int? DoeFundingCategoryId { get; set; }
        [Display(Name = "Grant Number")]
        [StringLength(250)]
        public string GrantNumber { get; set; }
        [Display(Name = "Fiscal Year")]
        [StringLength(4)]
        public string Year { get; set; }
        [Display(Name = "Funding Org")]
        [StringLength(10)]
        public string Org { get; set; }
        [Display(Name = "Percent")]
        [StringLength(6)]
        public string Percent { get; set; }
        [Display(Name = "Contract Number")]
        [StringLength(250)]
        public string ContractNumber { get; set; }
        [Display(Name = "Project Area")]
        [StringLength(250)]
        public string ProjectArea { get; set; }
        [Display(Name = "Tracking Number")]
        [StringLength(250)]
        public string TrackingNumber { get; set; }
        [Display(Name = "Project Number")]
        [StringLength(250)]
        public string ProjectNumber { get; set; }
        [Display(Name = "Principal Investigator")]
        [StringLength(6)]
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
        //Funding End

        //Subject
        [Display(Name = "Subject")]
        public int SubjectId { get; set; }
        //Subject End

        //Keyword
        [Display(Name = "Keyword")]
        public string Keyword { get; set; }
        //Keyword End

        //Sponsoring Orginization
        [Display(Name = "Sponsoring Orginization")]
        public string Sponsor { get; set; }
        //Sponsoring Orginization End

        //Attachments
        public List<SortAttachmentObject> Attachments { get; set; }
        [Required, Display(Name = "Type of Attachment")]
        public string AttachmentType { get; set; }
        [Required, Display(Name = "Description")]
        public string AttachmentNote { get; set; }
        [Required, Display(Name = "File")]
        public HttpPostedFileBase File { get; set; }
        //Attachments End

        //Selected Tab
        public string SelectedTab { get; set; }
        //Selected Tab End
        #endregion

        #region Extended Properties
        [Display(Name = "Author Name")]
        public List<AuthorObject> Authors => Sort?.Authors;
        public List<ContactObject> Contacts => Sort?.Contacts;
        public List<FundingObject> Funding => Sort?.Funding;
        public List<SortMetaDataObject> SubjectCategories => Sort?.SubjectCategories;
        [Display(Name = "Keyword(s)")]
        public List<SortMetaDataObject> KeywordList => Sort?.KeyWordList;
        public List<SortMetaDataObject> SponsoringOrgs => Sort?.SponsoringOrgs;
        [Display(Name = "Missing Required Data")]
        public List<MissingData> MissingData => Sort?.MissingDatas.OrderBy(n => n.Section).ToList();
        public SelectList Affiliations => BsHelper.GetEnumSelectList<AffiliationEnum>();

        public bool NeedsPublished => (Sort?.StatusEnum == StatusEnum.Complete && Sort?.ReviewStatus == "Approved" && UserObject.CurrentUser.IsInAnyRole("Admin,ReleaseOfficial") && SortAttachmentObject.GetOstiAttachment(SortMainId) != null);
        public bool CanPublish => ((Sort?.StatusEnum == StatusEnum.Complete || Sort?.StatusEnum == StatusEnum.Published) && UserObject.CurrentUser.IsInAnyRole("Admin,ReleaseOfficial"));
        public bool HasOstiDoc => SortAttachmentObject.GetOstiAttachment(SortMainId) != null;

        public bool ForceEdms => Sort?.ForceEdms ?? false;

        public bool IsPublished => (Sort?.StatusEnum == StatusEnum.Published && UserObject.CurrentUser.IsInAnyRole("Admin"));

        public bool CanUpdateDueDate => Sort?.CanUpdateDueDate ?? false;
        public bool UserIsAdmin => UserObject.CurrentUser.IsInAnyRole("Admin,ReleaseOfficial");

        public bool NeedsReminder => Sort?.CanUpdateDueDate??false;

        public string OwnerName => Sort?.OwnerName;

        public bool CoverPageRequired => Sort?.CoverPageRequired ?? (Sort?.IsExternalType ?? false ? false : true);

        [Display(Name = "Delay To Date")]
        public DateTime? DelayToDate => Sort?.DelayToDate;

        public string DelayToDateStr => $"{DelayToDate:MM/dd/yyyy}";

        [Display(Name = "Reason Delayed")]
        public string DelayReason => Sort?.DelayReason;

        public string LrsUrl
        {
            get
            {
                if (Sort != null && Sort.IsFromLrs && Sort.SharePointId.HasValue)
                {
                    return $"{Business.Config.LrsUrl.TrailingForwardSlash()}Artifact/Index/{Sort.SharePointId}";
                }

                return string.Empty;
            }
        }

        public bool UserHasWriteAccess { get; set; } = true;
        public bool UserHasReadAccess { get; set; } = true;

        public string DoeFundingNumber => Config.DoeContractNumber;
        #endregion

        #region Constructor
        public ArtifactModel() { }

        public ArtifactModel(int id, string selectedTab)
        {
            SortMainId = id;
            SelectedTab = selectedTab;
            HydrateData();
        }
        #endregion

        #region Functions
        public void HydrateData()
        {
            Sort = SortMainObject.GetSortMain(SortMainId);
            Attachments = SortAttachmentObject.GetSortAttachments(SortMainId);
            Sort?.CheckForMissingData();
            UserHasWriteAccess = Sort?.UserHasWriteAccess() ?? true;
            UserHasReadAccess = Sort?.UserHasReadAccess() ?? true;
        }
        #endregion

    }
}