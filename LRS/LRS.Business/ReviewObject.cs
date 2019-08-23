using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using Inl.MvcHelper;

namespace LRS.Business
{
    public class ReviewObject
    {
        #region Properties

        public int ReviewId { get; set; }
        public int MainId { get; set; }
        public DateTime EntryDate { get; set; }
        public string ReviewerEmployeeId { get; set; }
        public string ReviewerType { get; set; }
        public DateTime? ReviewDate { get; set; }
        public bool Required { get; set; }
        public bool SystemReviewer => (ReviewerTypeEnum == ReviewerTypeEnum.Classification || ReviewerTypeEnum == ReviewerTypeEnum.ExportControl || ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment);
        public string Status { get; set; }
        public string Comments { get; set; }
        public DateTime? StatusDate { get; set; }
        public DateTime? LastEmailDate { get; set; }
        public string ReviewerFirstName { get; set; }
        public string ReviewerLastName { get; set; }

        #endregion

        #region Extended Properties

        public string ReviewerName => string.IsNullOrWhiteSpace(ReviewerEmployeeId) ? string.Empty : EmployeeCache.GetEmployee(ReviewerEmployeeId)?.FullName;

        public string Email => EmployeeCache.GetEmployee(ReviewerEmployeeId)?.Email;

        public ReviewerTypeEnum ReviewerTypeEnum
        {
            get => ReviewerType.ToEnum<ReviewerTypeEnum>();
            set => ReviewerType = value.ToString();
        }

        public ReviewStatusEnum ReviewStatus
        {
            get => Status. ToEnum< ReviewStatusEnum>();
            set => Status = value.ToString();
        }

        public string ReviewStatusTxt => ReviewStatus.GetEnumDisplayName();

        public string ReviewerTypeDisplayName => ReviewerTypeEnum.GetEnumDisplayName();

        public List<GenericReviewUserObject> SystemReviewerUsers => MemoryCache.GetGenericReviewUsers(ReviewerTypeEnum) ?? new List<GenericReviewUserObject>();

        public string EditButtons
        {
            get
            {
                string returnStr = string.Empty;
                returnStr += "<div class=\"btn-group btn-group-xs\">";
                if (MainObject.CheckUserHasWriteAccess(MainId))
                {
                    if (ReviewStatus == ReviewStatusEnum.New || ReviewStatus == ReviewStatusEnum.Active || ReviewStatus == ReviewStatusEnum.NotReviewing)
                    {
                        returnStr += "      <button style=\"margin-right:5px;\" type=\"button\" onclick=\"FireButton(this, RemoveReviewer)\" class=\"btn btn-danger\">";
                        returnStr += "          <i class=\"fa fa-times\" aria-hidden=\"true\" title=\"Remove Reviewer\"> Remove</i></button>";
                    }
                    if (ReviewStatus == ReviewStatusEnum.Active && LastEmailDate.HasValue && LastEmailDate.Value.AddDays(7) < DateTime.Now)
                    {
                        returnStr += "      <button style=\"margin-right:5px;\" type=\"button\" onclick=\"FireButton(this, SendReminder)\" class=\"btn btn-primary\">";
                        returnStr += "          <i class=\"fa fa-envelope\" aria-hidden=\"true\" title=\"Send Reviewer Reminder Email\"> Reminder</i></button>";
                    }
                    if (ReviewStatus == ReviewStatusEnum.Complete && ReviewerTypeEnum == ReviewerTypeEnum.PeerTechnical)
                    {
                        returnStr += "      <button style=\"margin:0;\" type=\"button\" onclick=\"FireButton(this, ReactivateReviewer)\" class=\"btn btn-default\">";
                        returnStr += "          <i class=\"fa fa-refresh\" aria-hidden=\"true\" title=\"Reactivate Reviewer\"> Reactivate</i></button>";
                    }
                }

                returnStr += "</div>";
                return returnStr;
            }
        }

        public string IndexButtons
        {
            get
            {
                string returnStr = string.Empty;
                if (MainObject.CheckUserHasWriteAccess(MainId))
                {
                    if (ReviewStatus == ReviewStatusEnum.Active && (LastEmailDate.HasValue && LastEmailDate.Value.AddDays(7) < DateTime.Now || UserObject.CurrentUser.IsInAnyRole("Admin,ReleaseOfficial")))
                    {
                        returnStr += "<div class=\"btn-group btn-group-xs\">";
                        returnStr += "      <button style=\"margin:0;\" type=\"button\" onclick=\"FireButton(this, SendReminder)\" class=\"btn btn-primary\">";
                        returnStr += "          <i class=\"fa fa-envelope\" aria-hidden=\"true\" title=\"Send Reviewer Reminder Email\"> Reminder</i></button>";
                        returnStr += "</div>";
                    }
                }

                return returnStr;
            }
        }

        [Display(Name = "Reviewer Comments")]
        public string CommentsTable
        {
            get
            {
                var comments = ReviewCommentObject.GetReviewCommentsForReview(ReviewId);
                if (comments != null && comments.Count > 0)
                {
                    StringBuilder table = new StringBuilder();
                    table.Append("<table class=\"table table-condensed datatable\" role=\"grid\"> ");
                    table.Append("<thead>");
                    table.Append("<tr role=\"row\" class=\"active\">");
                    table.Append("<th class=\"hidden\" rowspan=\"1\" colspan=\"1\">CommentId</th>");
                    if (ReviewerTypeEnum == ReviewerTypeEnum.Classification ||
                        ReviewerTypeEnum == ReviewerTypeEnum.ExportControl ||
                        ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment)
                    {
                        table.Append("<th rowspan=\"1\" colspan=\"1\">Comment By</th>");
                    }
                    table.Append("<th rowspan=\"1\" colspan=\"1\">Date</th>");
                    table.Append("<th rowspan=\"1\" colspan=\"1\">Comment</th>");
                    if (comments.Exists(n => n.EmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        table.Append("<th rowspan=\"1\" colspan=\"1\"></th>");
                    }
                    table.Append("</tr>");
                    table.Append("</thead>");
                    table.Append("<tbody>");
                    foreach (var c in comments)
                    {
                        table.Append("<tr role=\"row\" class=\"active\">");
                        table.Append("<td class=\"hidden\">" + c.ReviewCommentId + "</td>");
                        if (ReviewerTypeEnum == ReviewerTypeEnum.Classification ||
                            ReviewerTypeEnum == ReviewerTypeEnum.ExportControl ||
                            ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment)
                        {
                            table.Append("<td>" + c.ReviewerName + "</td>");
                        }
                        table.Append("<td>" + c.EntryDate.ToString("MM/dd/yyyy") + "</td>");
                        table.Append("<td>" + c.Comment + "</td>");
                        if (c.EmployeeId == UserObject.CurrentUser.EmployeeId || UserObject.CurrentUser.IsInAnyRole("Admin"))
                        {
                            table.Append("<td>");
                            table.Append("<div class=\"btn-group btn-group-xs\">");
                            table.Append("      <button style=\"margin:0;\" type=\"button\" onclick=\"EditComment(" + c.ReviewCommentId + ");\" class=\"btn btn-primary\">");
                            table.Append("          <i class=\"fa fa-pencil\" aria-hidden=\"true\" title=\"Edit Comment\"></i></button>");
                            table.Append("      <button style=\"margin:0;\" type=\"button\" onclick=\"DeleteComment(" + c.ReviewCommentId + ");\" class=\"btn btn-danger\">");
                            table.Append("          <i class=\"fa fa-times\" aria-hidden=\"true\" title=\"Delete Comment\"></i></button>");
                            table.Append("</div>");
                            table.Append("</td>");
                        }
                        table.Append("</tr>");
                    }

                    table.Append("</tbody>");
                    table.Append("</table>");
                    return table.ToString();
                }

                return null;
            }
        }

        public string ReviewDateStr => ReviewDate?.ToString("MM/dd/yyyy hh:mm tt");
        public string StatusDateStr => StatusDate?.ToString("MM/dd/yyyy hh:mm tt");
        public string LastEmailDateStr => LastEmailDate?.ToString("MM/dd/yyyy hh:mm tt");

        public string MainDisplayTitle => MainObject.GetMain(MainId)?.DisplayTitle;
        public string MainTitle => MainObject.GetMain(MainId)?.Title;
        public string NumberPagesStr => MainObject.GetMain(MainId)?.NumberPagesStr;
        #endregion

        #region Constructor

        public ReviewObject() { }

        #endregion

        #region Repository

        private static IReviewRepository repo => new ReviewRepository();

        #endregion

        #region Static Methods

        public static List<ReviewObject> GetReviews(int mainId, IDbConnection conn = null) => repo.GetReviews(mainId, conn);

        public static List<ReviewObject> GetAllActiveReviews(IDbConnection conn = null) => repo.GetAllActiveReviews(conn);

        public static ReviewObject GetReview(int reviewId) => repo.GetReview(reviewId);
        
        public static bool MoveReviewsTohistory(int mainId) => repo.MoveReviewsTohistory(mainId);

        public static void CreateNew(MainObject main, ReviewerTypeEnum reviewType, ReviewStatusEnum startStatus, bool required)
        {
            var review = new ReviewObject
            {
                MainId = main.MainId.Value,
                ReviewStatus = startStatus,
                StatusDate = DateTime.Now,
                //ReviewDate = DateTime.Now,
                ReviewerTypeEnum = reviewType,
                Required = required
            };
            review.Save();

            if (review.ReviewStatus == ReviewStatusEnum.Active)
            {
                review.SendActivateEmail(main);
            }
        }
        #endregion

        #region Object Methods

        public void Save(bool checkForEmail = false)
        {
            bool needsEmail = false;
            MainObject main = null;
            if (!Config.IsImport)
            {
                if (ReviewId == 0 || checkForEmail)
                {
                    main = MainObject.GetMain(MainId);
                    if (main != null)
                    {
                        if ((main.StatusEnum == StatusEnum.InPeerReview && ReviewerTypeEnum == ReviewerTypeEnum.PeerTechnical) ||
                            (main.StatusEnum == StatusEnum.InReview && ReviewerTypeEnum == ReviewerTypeEnum.Manager))
                        {
                            needsEmail = true;
                            ReviewStatus = ReviewStatusEnum.Active;
                        }
                    }
                }
            }

            ReviewerFirstName = ReviewerFirstName ?? EmployeeCache.GetEmployee(ReviewerEmployeeId)?.FirstName;
            ReviewerLastName = ReviewerLastName ?? EmployeeCache.GetEmployee(ReviewerEmployeeId)?.LastName;

            repo.SaveReview(this);

            if (needsEmail && main != null)
            {
                switch (ReviewerTypeEnum)
                {
                    case ReviewerTypeEnum.PeerTechnical:
                        LRS.Business.Email.SendEmail(main, this, EmailTypeEnum.PeerReviewer, null);
                        break;
                    case ReviewerTypeEnum.Manager:
                        LRS.Business.Email.SendEmail(main, this, EmailTypeEnum.ManagerReviewer, null);
                        break;
                }
            }
            MainObject.UpdateActivityDateToNow(MainId);
        }

        public void Activate(MainObject main = null)
        {
            main = main ?? MainObject.GetMain(MainId);
            ReviewStatus = ReviewStatusEnum.Active;
            StatusDate = DateTime.Now;
            Save();
            SendActivateEmail(main);
        }

        public void SendActivateEmail(MainObject main)
        {
            EmailTypeEnum? emailType = null;
            switch (ReviewerTypeEnum)
            {
                case ReviewerTypeEnum.Classification:
                    emailType = EmailTypeEnum.ClassReviewer;
                    break;
                case ReviewerTypeEnum.ExportControl:
                    emailType = EmailTypeEnum.ExportReviewer;
                    break;
                case ReviewerTypeEnum.Manager:
                    emailType = EmailTypeEnum.ManagerReviewer;
                    break;
                case ReviewerTypeEnum.PeerTechnical:
                    emailType = EmailTypeEnum.PeerReviewer;
                    break;
                case ReviewerTypeEnum.TechDeployment:
                    emailType = EmailTypeEnum.TechReviewer;
                    break;
            }

            if (emailType.HasValue)
            {
                LRS.Business.Email.SendEmail(main, this, emailType.Value, null);
            }
        }

        public void Complete(MainObject main = null)
        {
            main = main ?? MainObject.GetMain(MainId);

            ReviewStatus = ReviewStatusEnum.Complete;
            StatusDate = DateTime.Now;
            ReviewDate = DateTime.Now;
            if (string.IsNullOrWhiteSpace(ReviewerEmployeeId) || !ReviewerEmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase))
            {
                ReviewerEmployeeId = UserObject.CurrentUser.EmployeeId;
                ReviewerFirstName = ReviewerFirstName ?? EmployeeCache.GetEmployee(ReviewerEmployeeId)?.FirstName;
                ReviewerLastName = ReviewerLastName ?? EmployeeCache.GetEmployee(ReviewerEmployeeId)?.LastName;
            }
            Save();
            LRS.Business.Email.SendEmail(main, this, EmailTypeEnum.ReviewComplete, null); //Send email that review is complete
        }

        public void Approve(MainObject main = null)
        {
            main = main ?? MainObject.GetMain(MainId);
            ReviewStatus = ReviewStatusEnum.Approved;
            StatusDate = DateTime.Now;
            ReviewDate = DateTime.Now;
            if (string.IsNullOrWhiteSpace(ReviewerEmployeeId) || !ReviewerEmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase))
            {
                ReviewerEmployeeId = UserObject.CurrentUser.EmployeeId;
                ReviewerFirstName = ReviewerFirstName ?? EmployeeCache.GetEmployee(ReviewerEmployeeId)?.FirstName;
                ReviewerLastName = ReviewerLastName ?? EmployeeCache.GetEmployee(ReviewerEmployeeId)?.LastName;
            }
            Save();
            LRS.Business.Email.SendEmail(main, this, EmailTypeEnum.Approved, null); //Send email that review is approved
        }

        public void Reject(string reason, MainObject main = null)
        {
            main = main ?? MainObject.GetMain(MainId);

            // Add review comment for the reason
            new ReviewCommentObject()
            {
                Comment = reason,
                MainId = this.MainId,
                ReviewId = this.ReviewId,
                EmployeeId = UserObject.CurrentUser.EmployeeId
            }.Save();

            // Save the reject to the review
            ReviewStatus = ReviewStatusEnum.Rejected;
            StatusDate = DateTime.Now;
            ReviewDate = DateTime.Now;
            if (string.IsNullOrWhiteSpace(ReviewerEmployeeId) || !ReviewerEmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase))
            {
                ReviewerEmployeeId = UserObject.CurrentUser.EmployeeId;
                ReviewerFirstName = ReviewerFirstName ?? EmployeeCache.GetEmployee(ReviewerEmployeeId)?.FirstName;
                ReviewerLastName = ReviewerLastName ?? EmployeeCache.GetEmployee(ReviewerEmployeeId)?.LastName;
            }
            Save();
            // Send a email to the owner
            LRS.Business.Email.SendEmail(main, this, EmailTypeEnum.Rejected, null); //Send email that review is rejected
        }

        public void NotReviewing(MainObject main = null)
        {
            main = main ?? MainObject.GetMain(MainId);
            ReviewStatus = ReviewStatusEnum.NotReviewing;
            StatusDate = DateTime.Now;
            if (string.IsNullOrWhiteSpace(ReviewerEmployeeId) || !ReviewerEmployeeId.Equals(UserObject.CurrentUser.EmployeeId, StringComparison.InvariantCultureIgnoreCase))
            {
                ReviewerEmployeeId = UserObject.CurrentUser.EmployeeId;
                ReviewerFirstName = ReviewerFirstName ?? EmployeeCache.GetEmployee(ReviewerEmployeeId)?.FirstName;
                ReviewerLastName = ReviewerLastName ?? EmployeeCache.GetEmployee(ReviewerEmployeeId)?.LastName;
            }
            Save();
            LRS.Business.Email.SendEmail(main, this, EmailTypeEnum.ReviewerDeclinedReview, null); //Send email that reviewer has declined to review
        }

        public void ReActivate()
        {
            ReviewStatus = ReviewStatusEnum.New;
            ReviewDate = null;
            StatusDate = DateTime.Now;
            Save(true);
        }

        public bool Cancel()
        {
            if (repo.MoveReviewToHistory(ReviewId))
            {
                if (ReviewStatus == ReviewStatusEnum.Active)
                {
                    var main = MainObject.GetMain(MainId);
                    if (main != null)
                    {
                        //Send email to reviewer of the cancellation.
                        LRS.Business.Email.SendEmail(main, this, EmailTypeEnum.CancelledReviewer, null);
                    }
                    main.CheckStatus();
                }

                MainObject.UpdateActivityDateToNow(MainId);

                return true;
            }

            return false;
        }

        public void SendReminderEmail()
        {
            LRS.Business.Email.SendEmail(MainObject.GetMain(MainId), this, EmailTypeEnum.ReviewReminder, null);
        }
        #endregion
    }

    public interface IReviewRepository
    {
        List<ReviewObject> GetReviews(int mainId, IDbConnection conn = null);
        List<ReviewObject> GetAllActiveReviews(IDbConnection conn = null);
        ReviewObject GetReview(int reviewId);
        ReviewObject SaveReview(ReviewObject review);
        bool MoveReviewToHistory(int reviewId);
        bool MoveReviewsTohistory(int mainId);
    }

    public class ReviewRepository : IReviewRepository
    {
        public List<ReviewObject> GetReviews(int mainId, IDbConnection conn = null) => (conn ?? Config.Conn).Query<ReviewObject>("SELECT * FROM dat_Review WHERE MainId = @MainId", new {MainId = mainId}).ToList();

        public List<ReviewObject> GetAllActiveReviews(IDbConnection conn = null) => (conn ?? Config.Conn).Query<ReviewObject>("SELECT r.* FROM dat_Main m inner join dat_Review r on r.MainId = m.MainId and r.Status = 'Active' where m.Status = 'InReview'").ToList();

        public ReviewObject GetReview(int reviewId) => Config.Conn.Query<ReviewObject>("SELECT * FROM dat_Review WHERE ReviewId = @ReviewId", new { ReviewId = reviewId }).FirstOrDefault();

        public ReviewObject SaveReview(ReviewObject review)
        {
            if (review.ReviewId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_Review
                    SET     ReviewerEmployeeId = @ReviewerEmployeeId,
                            ReviewDate = @ReviewDate,
                            Status = @Status,
                            Comments = @Comments,
                            StatusDate = @StatusDate,
                            LastEmailDate = @LastEmailDate,
                            ReviewerFirstName = @ReviewerFirstName,
                            ReviewerLastName = @ReviewerLastName
                    WHERE   ReviewId = @ReviewId";
                Config.Conn.Execute(sql, review);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_Review (
                        MainId,
                        ReviewerEmployeeId,
                        ReviewerType,
                        ReviewDate,
                        Required,
                        SystemReviewer,
                        Status,
                        Comments,
                        StatusDate,
                        LastEmailDate,
                        ReviewerFirstName,
                        ReviewerLastName
                    )
                    VALUES (
                        @MainId,
                        @ReviewerEmployeeId,
                        @ReviewerType,
                        @ReviewDate,
                        @Required,
                        @SystemReviewer,
                        @Status,
                        @Comments,
                        @StatusDate,
                        @LastEmailDate,
                        @ReviewerFirstName,
                        @ReviewerLastName
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                review.ReviewId = Config.Conn.Query<int>(sql, review).Single();
            }
            return review;
        }

        public bool MoveReviewToHistory(int reviewId)
        {
            using (IDbConnection conn = Config.Conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = @" insert into dat_ReviewHistory(ReviewId, MainId, EntryDate, ReviewerEmployeeId, ReviewerType, ReviewDate, [Required],
							                                  SystemReviewer, [Status], Comments, StatusDate, LastEmailDate, HistoryDate)
                                select ReviewId, MainId, EntryDate, ReviewerEmployeeId, ReviewerType, ReviewDate, [Required],
	                                   SystemReviewer, [Status], Comments, StatusDate, LastEmailDate, GetDate()
                                from dat_Review
                                Where ReviewId = @ReviewId";
                        conn.Execute(sql, new {ReviewId = reviewId}, transaction);

                        sql = @"insert into dat_ReviewCommentHistory (ReviewCommentId, MainId, ReviewId, EntryDate, EmployeeId, Comment, HistoryDate)
                        select ReviewCommentId, MainId, ReviewId, EntryDate, EmployeeId, Comment, GetDate()
                        FROM dat_ReviewCommentHistory
                        WHERE ReviewId = @ReviewId";

                        conn.Execute(sql, new {ReviewId = reviewId}, transaction);

                        conn.Execute("DELETE FROM dat_ReviewComment WHERE ReviewId = @ReviewId", new {ReviewId = reviewId}, transaction);
                        conn.Execute("DELETE FROM dat_Review WHERE ReviewId = @ReviewId", new {ReviewId = reviewId}, transaction);
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }

                    transaction.Commit();
                    return true;
                }
            }
        }

        public bool MoveReviewsTohistory(int mainId)
        {
            using (IDbConnection conn = Config.Conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = @" insert into dat_ReviewHistory(ReviewId, MainId, EntryDate, ReviewerEmployeeId, ReviewerType, ReviewDate, [Required],
							                                  SystemReviewer, [Status], Comments, StatusDate, LastEmailDate, HistoryDate)
                                select ReviewId, MainId, EntryDate, ReviewerEmployeeId, ReviewerType, ReviewDate, [Required],
	                                   SystemReviewer, [Status], Comments, StatusDate, LastEmailDate, GetDate()
                                from dat_Review
                                Where MainId = @MainId";
                        conn.Execute(sql, new {MainId = mainId}, transaction);

                        sql = @"insert into dat_ReviewCommentHistory (ReviewCommentId, MainId, ReviewId, EntryDate, EmployeeId, Comment, HistoryDate)
                        select ReviewCommentId, MainId, ReviewId, EntryDate, EmployeeId, Comment, GetDate()
                        FROM dat_ReviewCommentHistory
                        WHERE MainId = @MainId";

                        conn.Execute(sql, new {MainId = mainId}, transaction);

                        conn.Execute("DELETE FROM dat_ReviewComment WHERE MainId = @MainId", new {MainId = mainId}, transaction);
                        conn.Execute("DELETE FROM dat_Review WHERE MainId = @MainId", new {MainId = mainId}, transaction);
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }

                    transaction.Commit();
                    return true;
                }
            }
        }
    }
}
