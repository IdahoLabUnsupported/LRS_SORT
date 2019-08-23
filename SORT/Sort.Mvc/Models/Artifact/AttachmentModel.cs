using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class AttachmentModel
    {
        #region Properties
        public int SortMainId { get; set; }
        public string Filename { get; set; }
        public int? Size { get; set; }
        public string AttachmentNote { get; set; }
        public AttachmentTypeEnum AttachmentType { get; set; }
        public int? SortAttachmentId { get; set; }
        public HttpPostedFileBase File { get; set; }

        public List<SortAttachmentObject> Attachments { get; set; } = new List<SortAttachmentObject>();
        #endregion

        #region Constructor
        public AttachmentModel()
        {
        }

        public AttachmentModel(int sortMainId)
        {
            SortMainId = sortMainId;
            Attachments = SortAttachmentObject.GetSortAttachments(sortMainId);
        }
        #endregion

        public bool DuplicateFinal()
        {
            return (AttachmentType == AttachmentTypeEnum.FinalDoc &&
                    SortAttachmentObject.GetSortAttachments(SortMainId).Exists(n => !n.IsDeleted && n.AttachmentTypeEnum == AttachmentTypeEnum.FinalDoc));
        }

        public void Save()
        {
            SortAttachmentId = SortAttachmentObject.AddAttachment(SortMainId, Filename, AttachmentNote, AttachmentType, Size).SortAttachmentId;
        }
    }
}