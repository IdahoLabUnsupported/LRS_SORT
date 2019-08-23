using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Inl.MvcHelper;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class HomeModel
    {
        #region Properties

        public bool IsSmall { get; set; }
        public int InProcess { get; set; }
        public int PastDue { get; set; }
        public int CompletedNeedReview { get; set; }
        public int Completed { get; set; }
        public int Published { get; set; }

        public List<SortMainObject> Sorts { get; set; }

        public bool HasData => (InProcess + Completed + PastDue + Published) > 0;

        public ViewModeOptionEnum ViewMode { get; set; }
        public bool ShowPublished { get; set; }
        public SelectList ViewModes() => BsHelper.GetEnumSelectList<ViewModeOptionEnum>();
        public DateTime? LastUpdate { get; set; }

        public int OrgMode { get; set; }
        public string OrgOption { get; set; }
        public SelectList UserOrgs => new SelectList(UserOrgObject.GetUserOrgs(Config.User.EmployeeId), "Org", "OrgDisplayName");
        #endregion

        #region Constructor

        public HomeModel()
        {
            HydrateData();
        }

        public HomeModel(ViewModeOptionEnum viewMode, bool? showPublished, int? orgMode, string orgOption) 
        {
            ViewMode = viewMode;
            ShowPublished = showPublished ?? false;
            OrgMode = orgMode ?? 0;
            OrgOption = orgOption;
            HydrateData();
        }
        #endregion

        public void HydrateData()
        {
            LastUpdate = Config.LastUpdateTime;
            if (Config.User.IsInAnyRole("Admin,ReleaseOfficial,ReadAll"))
            {
                foreach (var data in SortMainObject.GetCounts())
                {
                    switch (data.StatusEnum)
                    {
                        case StatusEnum.Imported:
                            if (data.PastDueDate)
                            {
                                PastDue += data.NumItems;
                            }
                            else
                            {
                                InProcess += data.NumItems;
                            }
                            break;
                        case StatusEnum.Complete:
                            if (data.ReviewStatus == "Approved")
                            {
                                Completed += data.NumItems;
                            }
                            else
                            {
                                CompletedNeedReview += data.NumItems;
                            }
                            break;
                        case StatusEnum.Published:
                            Published += ShowPublished ? data.NumItems : 0;
                            break;
                    }
                }
            }

            if (GetMainCount() <= 1000)
            {
                IsSmall = true;
                Sorts = GetMainData(ViewMode, OrgMode, OrgOption);
            }

        }
        public int GetMainCount()
        {
            int count = 0;
            if (Config.User.IsInAnyRole("Admin,ReleaseOfficial,ReadAll"))
            {
                count = SortMainObject.GetAllSortsCount(ViewMode);
            }
            else if (Config.User.IsInAnyRole("OrgManager"))
            {
                if (((OrgOptionEnum)OrgMode) == OrgOptionEnum.OrgArtifacts)
                {
                    count = SortMainObject.GetSortForOrgCount(Config.User.EmployeeId, OrgOption);
                }
                else
                {
                    count = SortMainObject.GetSortForUserCount(Config.User.EmployeeId);
                }
            }
            else
            {
                count = SortMainObject.GetSortForUserCount(Config.User.EmployeeId);
            }

            return count;
        }

        public static List<SortMainObject> GetMainData(ViewModeOptionEnum viewMode, int? orgMode, string orgOption)
        {
            var data = new List<SortMainObject>();
            orgMode = orgMode ?? 0;

            if (Config.User.IsInAnyRole("Admin,ReleaseOfficial,ReadAll"))
            {
                data = SortMainObject.GetAllSorts(viewMode);
            }
            else if (Config.User.IsInAnyRole("OrgManager"))
            {
                if (((OrgOptionEnum)orgMode) == OrgOptionEnum.OrgArtifacts)
                {
                    data = SortMainObject.GetSortForUserOrg(Config.User.EmployeeId, orgOption);
                }
                else
                {
                    data = SortMainObject.GetSortForUser(Config.User.EmployeeId);
                }
            }
            else
            {
                data = SortMainObject.GetSortForUser(Config.User.EmployeeId);
            }

            return data;
        }
    }
}