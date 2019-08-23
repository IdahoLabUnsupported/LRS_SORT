using System;
using System.Collections.Generic;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class AdminCommentsModel
    {
        public int? AdminCommentId { get; set; }
        public int MainId { get; set; }

        public string AdminComment { get; set; }
        public bool AdminOnly { get; set; }

        public bool IsReleaseOfficer => Current.IsReleaseOfficer;

        public List<AdminCommentObject> Comments { get; } = new List<AdminCommentObject>();

        public AdminCommentsModel() { }

        public AdminCommentsModel(int mainId)
        {
            MainId = mainId;
            Comments = AdminCommentObject.Get(MainId, IsReleaseOfficer);
        }

        public void Save()
        {
            if (AdminCommentId.HasValue)
            {
                var cmt = AdminCommentObject.GetAdminComment(AdminCommentId.Value);
                if (cmt != null)
                {
                    cmt.Comment = AdminComment;
                    cmt.AdminOnly = AdminOnly;
                    cmt.Save();
                }
            }
            else
            {
                AdminCommentId = AdminCommentObject.Add(MainId, AdminComment, AdminOnly);
            }
        }
    }
}