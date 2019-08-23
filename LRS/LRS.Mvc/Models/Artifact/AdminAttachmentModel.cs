using System.Collections.Generic;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class AdminAttachmentModel
    {
        public int? MainId { get; set; }

        public bool IsReleaseOfficer => Current.IsReleaseOfficer;

        public List<AttachmentObject> Attachments { get; } = new List<AttachmentObject>();

        public AdminAttachmentModel() { }

        public AdminAttachmentModel(int? mainId)
        {
            MainId = mainId;
            Attachments = AttachmentObject.GetAttachments(MainId ?? 0, true);
        }
    }
}