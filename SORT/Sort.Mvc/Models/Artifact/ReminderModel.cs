using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class ReminderModel
    {
        [Required]
        public int SortMainId { get; set; }
        [Required, Display(Name = "Public Release Reminder Date")]
        public DateTime ReminderDate { get; set; }
        [Display(Name = "STI Number and Rev")]
        public string Title { get; set; }
        [Display(Name = "Title")]
        public string PublishTitle { get; set; }
        public bool UserHasWriteAccess { get; private set; }

        public ReminderModel() { }

        public ReminderModel(int sortId)
        {
            SortMainId = sortId;
            var sort = SortMainObject.GetSortMain(sortId);
            if (sort != null)
            {
                UserHasWriteAccess = sort.UserHasWriteAccess();
                ReminderDate = sort.OneYearReminderDate ?? sort.PublicationDate?.AddYears(1) ?? DateTime.Now.AddYears(1);
                Title = sort.TitleStr;
                PublishTitle = sort.PublishTitle;
            }
        }

        public void Save()
        {
            if (SortMainObject.CheckUserHasWriteAccess(SortMainId))
            {
                SortMainObject.UpdateReminderDate(SortMainId, ReminderDate);
            }
        }
    }
}