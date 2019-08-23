using System;
using System.ComponentModel.DataAnnotations;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class DelayModel
    {
        [Required]
        public int SortMainId { get; set; }
        [Required, Display(Name = "Delay To Date")]
        public DateTime DelayToDate { get; set; }
        [Display(Name = "STI Number and Rev")]
        public string Title { get; set; }
        [Display(Name = "Title")]
        public string PublishTitle { get; set; }
        [Display(Name = "Delay Reason")]
        public string DelayReason { get; set; }
        public bool UserHasWriteAccess { get; private set; }

        public DelayModel() { }

        public DelayModel(int sortId)
        {
            SortMainId = sortId;
            var sort = SortMainObject.GetSortMain(sortId);
            if (sort != null)
            {
                UserHasWriteAccess = sort.UserHasWriteAccess();
                DelayToDate = sort.DelayToDate ?? DateTime.Now.AddMonths(6);
                Title = sort.TitleStr;
                PublishTitle = sort.PublishTitle;
            }
        }

        public void Save()
        {
            if (SortMainObject.CheckUserHasWriteAccess(SortMainId))
            {
                SortMainObject.UpdateDelayToDate(SortMainId, DelayToDate, DelayReason);
            }
        }
    }
}