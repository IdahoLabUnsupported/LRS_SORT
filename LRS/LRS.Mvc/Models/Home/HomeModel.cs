using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inl.MvcHelper;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class HomeModel
    {
        #region Properties
        public bool IsSmall { get; set; }
        public int InProcess { get; set; }
        public int UnderReview { get; set; }
        public int Completed { get; set; }
        public int Rejected { get; set; }
        public int Cancelled { get; set; }

        public List<MainObject> Mains { get; set; }

        public List<MainObject> Reviews { get; set; }

        public bool HasData => (InProcess + UnderReview + Completed + Rejected + Cancelled) > 0;

        public ViewModeOptionEnum ViewMode { get; set; }

        public int? ViewTime { get; set; } = 12;
        public SelectList ViewModes() => BsHelper.GetEnumSelectList<ViewModeOptionEnum>();

        public int OrgMode { get; set; }
        public string OrgOption { get; set; }
        public SelectList UserOrgs => new SelectList(UserOrgObject.GetUserOrgs(Current.User.EmployeeId), "Org", "OrgDisplayName");

        public bool UserIsGenericReviewer => MemoryCache.UserIsGenericUser(UserObject.CurrentUser.EmployeeId);
        #endregion

        #region Constructor

        public HomeModel()
        {
            HydrateData();
        }

        public HomeModel(ViewModeOptionEnum viewMode, int? viewTime, int? orgMode, string orgOption)
        {
            ViewMode = viewMode;
            ViewTime = viewTime ?? 12;
            OrgMode = orgMode ?? 0;
            OrgOption = orgOption;
            HydrateData();
        }
        #endregion

        public void HydrateData()
        {
            if (Current.User.IsInAnyRole("Admin,ReleaseOfficial,GenericReleaseUser"))
            {
                foreach (var data in MainObject.GetCounts(ViewTime.Value))
                {
                    switch (data.StatusEnum)
                    {
                        case StatusEnum.Cancelled:
                            Cancelled += data.NumItems;
                            break;
                        case StatusEnum.Completed:
                        case StatusEnum.ReviewNotRequired:
                            Completed += data.NumItems;
                            break;
                        case StatusEnum.Rejected:
                            Rejected += data.NumItems;
                            break;
                        case StatusEnum.InPeerReview:
                        case StatusEnum.InReview:
                            UnderReview += data.NumItems;
                            break;
                        default:
                            InProcess += data.NumItems;
                            break;
                    }
                }
            }

            Reviews = MainObject.GetReviewsForUser(Current.User.EmployeeId);

            if (GetMainCount() <= 200)
            {
                IsSmall = true;
                Mains = GetMainData(ViewMode, ViewTime, OrgMode, OrgOption);
            }
        }

        public int GetMainCount()
        {
            int count = 0;
            if (Current.User.IsInAnyRole("Admin,ReleaseOfficial,GenericReleaseUser"))
            {
                count = MainObject.GetAllMainsCount(ViewMode, ViewTime.Value);
            }
            else if (Current.User.IsInAnyRole("OrgManager"))
            {
                if (((OrgOptionEnum)OrgMode) == OrgOptionEnum.OrgArtifacts)
                {
                    count = MainObject.GetAllForOrgCount(ViewMode, ViewTime.Value, Current.User.EmployeeId, OrgOption);
                }
                else
                {
                    count = MainObject.GetAllForUserCount(ViewMode, ViewTime.Value, Current.User.EmployeeId);
                }
            }
            else
            {
                count = MainObject.GetAllForUserCount(ViewMode, ViewTime.Value, Current.User.EmployeeId);
            }

            return count;
        }

        public static List<MainObject> GetMainData(ViewModeOptionEnum viewMode, int? viewTime, int? orgMode, string orgOption)
        {
            var data = new List<MainObject>();
            viewTime = viewTime ?? 12;
            if (viewTime <= 0) viewTime = 12;
            if (viewTime > 360) viewTime = 360;
            orgMode = orgMode ?? 0;

            if (Current.User.IsInAnyRole("Admin,ReleaseOfficial,GenericReleaseUser"))
            {
                data = MainObject.GetAllMains(viewMode, viewTime.Value);
            }
            else if (Current.User.IsInAnyRole("OrgManager"))
            {
                if (((OrgOptionEnum) orgMode) == OrgOptionEnum.OrgArtifacts)
                {
                    data = MainObject.GetAllForOrg(viewMode, viewTime.Value, Current.User.EmployeeId, orgOption);
                }
                else
                {
                    data = MainObject.GetAllForUser(viewMode, viewTime.Value, Current.User.EmployeeId);
                }
            }
            else
            {
                data = MainObject.GetAllForUser(viewMode, viewTime.Value, Current.User.EmployeeId);
            }

            return data;
        }
    }
}