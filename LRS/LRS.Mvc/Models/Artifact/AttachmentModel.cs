using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class AttachmentModel
    {
        #region Properties
        public int MainId { get; set; }
        public string Filename { get; set; }
        public int? Size { get; set; }
        [Display(Name = "Comments/Description")]
        public string AttachmentNote { get; set; }
        public AttachmentTypeEnum AttachmentType { get; set; }
        public int? AttachmentId { get; set; }
        public HttpPostedFileBase File { get; set; }

        public int? NumberPages { get; set; }

        public bool AdminOnly { get; set; }

        public List<AttachmentObject> Attachments { get; set; } = new List<AttachmentObject>();
        #endregion

        #region Constructor
        public AttachmentModel(){}

        public AttachmentModel(int mainId)
        {
            MainId = mainId;
            Attachments = AttachmentObject.GetAttachments(MainId);
        }
        #endregion

        public void Save()
        {
            AttachmentId = AttachmentObject.AddAttachment(MainId, Filename, AttachmentNote, Size, NumberPages, AdminOnly).AttachmentId;
        }
    }
}