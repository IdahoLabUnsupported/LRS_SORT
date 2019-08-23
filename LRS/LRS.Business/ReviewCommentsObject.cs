using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LRS.Business
{
    public class ReviewCommentObject
    {
        #region Properties

        public int ReviewCommentId { get; set; }
        public int MainId { get; set; }
        public int ReviewId { get; set; }
        public DateTime EntryDate { get; set; }
        public string EmployeeId { get; set; }
        public string Comment { get; set; }

        #endregion

        #region Extended Properties

        public string ReviewerName => EmployeeCache.GetEmployee(EmployeeId)?.FullName;

        public string MainTitle => MainObject.GetMain(MainId)?.DisplayTitle;

        public string EntryDateStr => EntryDate.ToString("MM/dd/yyyy hh:mm tt");

        #endregion

        #region Constructor

        public ReviewCommentObject() { }

        #endregion

        #region Repository

        private static IReviewCommentRepository repo => new ReviewCommentRepository();

        #endregion

        #region Static Methods

        public static List<ReviewCommentObject> GetReviewComments(int mainId) => repo.GetReviewComments(mainId);

        public static List<ReviewCommentObject> GetReviewCommentsForReview(int reviewId) => repo.GetReviewCommentsForReview(reviewId);
        
        public static ReviewCommentObject GetReviewComment(int reviewCommentId) => repo.GetReviewComment(reviewCommentId);

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveReviewComment(this);
            Email.SendEmail(MainObject.GetMain(MainId), ReviewObject.GetReview(ReviewId), EmailTypeEnum.ReviewerCommented, Comment);
            MainObject.UpdateActivityDateToNow(MainId);
        }

        public void Delete()
        {
            repo.DeleteReviewComment(ReviewCommentId);
            MainObject.UpdateActivityDateToNow(MainId);
        }

        #endregion
    }

    public interface IReviewCommentRepository
    {
        List<ReviewCommentObject> GetReviewComments(int mainId);
        List<ReviewCommentObject> GetReviewCommentsForReview(int reviewId);
        ReviewCommentObject GetReviewComment(int reviewCommentId);
        ReviewCommentObject SaveReviewComment(ReviewCommentObject reviewComment);
        bool DeleteReviewComment(int reviewCommentId);
    }

    public class ReviewCommentRepository : IReviewCommentRepository
    {
        public List<ReviewCommentObject> GetReviewComments(int mainId) => Config.Conn.Query<ReviewCommentObject>("SELECT * FROM dat_ReviewComment WHERE MainId = @MainId AND Deleted = 0 ORDER BY EntryDate", new { MainId = mainId }).ToList();

        public List<ReviewCommentObject> GetReviewCommentsForReview(int reviewId) => Config.Conn.Query<ReviewCommentObject>("SELECT * FROM dat_ReviewComment WHERE ReviewId = @ReviewId AND Deleted = 0 ORDER BY EntryDate", new { ReviewId = reviewId }).ToList();

        public ReviewCommentObject GetReviewComment(int reviewCommentId) => Config.Conn.Query<ReviewCommentObject>("SELECT * FROM dat_ReviewComment WHERE ReviewCommentId = @ReviewCommentId", new { ReviewCommentId = reviewCommentId }).FirstOrDefault();

        public ReviewCommentObject SaveReviewComment(ReviewCommentObject reviewComment)
        {
            if (reviewComment.ReviewCommentId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_ReviewComment
                    SET     Comment = @Comment
                    WHERE   ReviewCommentId = @ReviewCommentId";
                Config.Conn.Execute(sql, reviewComment);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_ReviewComment (
                        MainId,
                        ReviewId,
                        EmployeeId,
                        Comment
                    )
                    VALUES (
                        @MainId,
                        @ReviewId,
                        @EmployeeId,
                        @Comment
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                reviewComment.ReviewCommentId = Config.Conn.Query<int>(sql, reviewComment).Single();
            }
            return reviewComment;
        }

        public bool DeleteReviewComment(int reviewCommentId)
        {
            try
            {
                Config.Conn.Execute("UPDATE dat_ReviewComment SET Deleted = 1, DeleteEmployeeId = @EmployeeId WHERE ReviewCommentId = @ReviewCommentId", new { ReviewCommentId = reviewCommentId, EmployeeId = UserObject.CurrentUser.EmployeeId });
            }
            catch { return false; }
            return true;
        }
    }
}