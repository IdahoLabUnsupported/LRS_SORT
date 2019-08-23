using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LRS.Business
{
    public class AdminCommentObject
    {
        #region Properties

        public int AdminCommentId { get; set; }
        public int MainId { get; set; }
        public DateTime EntryDate { get; set; }
        public string EmployeeId { get; set; }
        public string Comment { get; set; }
        public bool AdminOnly { get; set; }
        public bool Deleted { get; set; }
        public string DeleteEmployeeId { get; set; }

        #endregion

        #region Extended Properties

        public string UserName => EmployeeCache.GetName(EmployeeId);

        public string GridButtonsHtml
        {
            get
            {
                string returnStr = string.Empty;
                returnStr += "<div>";
                returnStr += "  <div class=\"btn-group btn-group-xs pull-right\" style=\"margin-left: -8px;\">";

                if (EmployeeId == UserObject.CurrentUser.EmployeeId)
                {
                    returnStr += "      <button style=\"margin:0 0 0 5px;\" type=\"button\" onclick=\"FireButton(this, EditAdminComment)\" class=\"btn btn-primary\">";
                    returnStr += "          <i class=\"fa fa-pencil-square-o\" aria-hidden=\"true\" title=\"Edit Comment\"></i></button>";
                }

                if (EmployeeId == UserObject.CurrentUser.EmployeeId || UserObject.CurrentUser.IsInAnyRole("Admin"))
                {
                    returnStr += "      <button style=\"margin:0 0 0 5px;\" type=\"button\" onclick=\"FireButton(this, DeleteAdminComment)\" class=\"btn btn-danger\">";
                    returnStr += "          <i class=\"fa fa-times\" title=\"Delete Comment\"></i></button>";
                }

                returnStr += "   </div>";
                returnStr += "  <div class=\"clearfix\"></div>";
                returnStr += "</div>";
                
                return returnStr;
            }
        }

        #endregion

        #region Constructor

        public AdminCommentObject() { }

        #endregion

        #region Repository

        private static IAdminCommentRepository repo => new AdminCommentRepository();

        #endregion

        #region Static Methods

        public static List<AdminCommentObject> Get(int mainId, bool isAdmin) => repo.Get(mainId, isAdmin);

        public static AdminCommentObject GetAdminComment(int adminCommentId) => repo.GetAdminComment(adminCommentId);

        public static int Add(int mainId, string comment, bool adminOnly)
        {
            var c = new AdminCommentObject
            {
                MainId = mainId,
                EmployeeId = UserObject.CurrentUser.EmployeeId,
                Comment = comment,
                AdminOnly = adminOnly
            };

            c.Save();

            return c.AdminCommentId;
        }

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.Save(this);
        }

        public void Delete()
        {
            Deleted = true;
            DeleteEmployeeId = UserObject.CurrentUser.EmployeeId;
            Save();
        }

        #endregion
    }

    public interface IAdminCommentRepository
    {
        List<AdminCommentObject> Get(int mainId, bool isAdmin);
        AdminCommentObject GetAdminComment(int adminCommentId);
        AdminCommentObject Save(AdminCommentObject adminComment);
    }

    public class AdminCommentRepository : IAdminCommentRepository
    {
        public List<AdminCommentObject> Get(int mainId, bool isAdmin) => Config.Conn.Query<AdminCommentObject>("SELECT * FROM dat_AdminComment WHERE MainId = @MainId AND (@IsAdmin = 1 OR AdminOnly = 0) AND Deleted = 0", new { MainId = mainId, IsAdmin = isAdmin }).ToList();

        public AdminCommentObject GetAdminComment(int adminCommentId) => Config.Conn.Query<AdminCommentObject>("SELECT * FROM dat_AdminComment WHERE AdminCommentId = @AdminCommentId", new { AdminCommentId = adminCommentId }).FirstOrDefault();

        public AdminCommentObject Save(AdminCommentObject adminComment)
        {
            if (adminComment.AdminCommentId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_AdminComment
                    SET     Comment = @Comment,
                            AdminOnly = @AdminOnly,
                            Deleted = @Deleted,
                            DeleteEmployeeId = @DeleteEmployeeId
                    WHERE   AdminCommentId = @AdminCommentId";
                Config.Conn.Execute(sql, adminComment);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_AdminComment (
                        MainId,
                        EmployeeId,
                        Comment,
                        AdminOnly
                    )
                    VALUES (
                        @MainId,
                        @EmployeeId,
                        @Comment,
                        @AdminOnly
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                adminComment.AdminCommentId = Config.Conn.Query<int>(sql, adminComment).Single();
            }
            return adminComment;
        }

    }
}
