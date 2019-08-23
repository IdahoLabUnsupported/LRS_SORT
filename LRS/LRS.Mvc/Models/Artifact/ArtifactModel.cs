using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class ArtifactModel 
    {
        #region Properties
        public int MainId { get; set; }
        public string Status => Main?.Status;
        public StatusEnum? StatusEnum => Main?.StatusEnum;

        public MainObject Main { get; set; } = new MainObject();

        public ReviewObject Reviewer => Main?.ActiveReviewer;
        public bool IsActiveReviewer => Main?.IsActiveReviewer ?? false;

        public int? ReviewId => Reviewer?.ReviewId;

        public ReviewerTypeEnum? ReviewerType => Reviewer?.ReviewerTypeEnum;
        //Selected Tab
        public string SelectedTab { get; set; }
        //Selected Tab End

        #endregion

        #region Extended Properties
        [Display(Name = "Intellectual Property")]
        public List<IntellectualPropertyObject> Intellectuals => Main?.Intellectuals;
        [Display(Name = "Subject Category Codes")]
        public List<MetaDataObject> SubjectCategories => Main?.SubjectCategories;
        [Display(Name = "Core Capabilities")]
        public List<MetaDataObject> CoreCapabilities => Main?.CoreCapabilities;
        [Display(Name = "Keyword(s)")]
        public List<MetaDataObject> KeywordList => Main?.KeyWordList;
        [Display(Name = "Attachment(s)")]
        public List<AttachmentObject> Attachments => Main?.Attachments;

        public string OwnerName => Main?.OwnerName;

        public int? ChildId => (Main != null && Main.ChildId.HasValue) ? Main.ChildId : null;
        public int? ParentId => (Main != null && Main.ParentId.HasValue) ? Main.ParentId : null;

        public string SortUrl
        {
            get
            {
                if (Main != null && Main.SortId.HasValue)
                {
                    return $"{Config.SortUrl.TrailingSlash()}Artifact/Index/{Main.SortId}";
                }

                return string.Empty;
            }
        }

        public bool Ouo3 { get; set; }
        public bool Ouo7 { get; set; }
        [Display(Name = "Derivative Classifier")]
        public string Ouo7EmployeeId { get; set; }

        public bool ShowAdminControls => IsReleaseOfficer || (Main?.IsSpecialReviewer ?? false);
        public bool IsReleaseOfficer => Current.IsReleaseOfficer;
        #endregion

        #region Constructor
        public ArtifactModel() { }

        public ArtifactModel(int id, string selectedTab)
        {
            MainId = id;
            SelectedTab = selectedTab;
            HydrateData();
        }
        #endregion

        #region Functions
        public void HydrateData()
        {
            Main = MainObject.GetMain(MainId);
            Ouo3 = Main?.Ouo3 ?? false;
            Ouo7 = Main?.Ouo7 ?? false;
            Ouo7EmployeeId = Main?.Ouo7EmployeeId ?? UserObject.CurrentUser.EmployeeId;

            Main?.CheckForMissingData();
        }

        #endregion

    }
}