using System;
using System.ComponentModel.DataAnnotations;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class DueDateModel
    {
        [Required]
        public int SortMainId { get; set; }
        [Required, Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }
        [Display(Name = "Send Reminder")]
        public bool SendReminder { get; set; }
        public string Title { get; set; }
        public bool? AltLocation { get; set; }

        public DueDateModel() { }

        public DueDateModel(int sortMainId, bool? altLocation)
        {
            SortMainId = sortMainId;
            AltLocation = altLocation;
            var sort = SortMainObject.GetSortMain(SortMainId);
            DueDate = DateTime.Now.AddDays(30);
            Title = sort?.TitleStr;
        }

        public bool UserHasAccess() => UserObject.CurrentUser.IsInAnyRole("Admin,ReleaseOfficial");

        public void Save()
        {
            var sort = SortMainObject.GetSortMain(SortMainId);
            if (sort != null && sort.UserHasWriteAccess())
            {
                sort.DueDate = DueDate;
                sort.Save();
                if (SendReminder)
                {
                    EmailReminder();
                }
            }
        }

        public string EmailReminder()
        {
            var sort = SortMainObject.GetSortMain(SortMainId);
            if (sort != null)
            {
                string errorMsg = string.Empty;
                if (!Email.SendEmail(sort, EmailTypeEnum.Reminder, true, ref errorMsg))
                {
                    return errorMsg;
                }

                return string.Empty;
            }

            return "Error: Unable to find the correct SORT Artifact to send the email for.";
        }
    }
}