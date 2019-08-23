using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LRS.Business
{
    public class ReviewCommentHistoryObject
    {
        #region Properties

        public int ReviewCommentId { get; set; }
        public int MainId { get; set; }
        public int ReviewId { get; set; }
        public DateTime EntryDate { get; set; }
        public string EmployeeId { get; set; }
        public string Comment { get; set; }
        public DateTime HistoryDate { get; set; }

        #endregion

        #region Extended Properties

        public string ReviewerName => EmployeeCache.GetEmployee(EmployeeId)?.FullName;

        public string MainTitle => MainObject.GetMain(MainId)?.DisplayTitle;

        public string EntryDateStr => EntryDate.ToString("MM/dd/yyyy hh:mm tt");

        #endregion

        #region Constructor

        public ReviewCommentHistoryObject() { }

        #endregion

        #region Repository

        private static IReviewCommentHistoryRepository repo => new ReviewCommentHistoryRepository();

        #endregion

        #region Static Methods

        public static List<ReviewCommentHistoryObject> GetReviewComments(int mainId) => repo.GetReviewComments(mainId);

        public static List<ReviewCommentHistoryObject> GetReviewCommentsForReview(int reviewId) => repo.GetReviewCommentsForReview(reviewId);

        #endregion

        #region Object Methods
        
        #endregion
    }

    public interface IReviewCommentHistoryRepository
    {
        List<ReviewCommentHistoryObject> GetReviewComments(int mainId);
        List<ReviewCommentHistoryObject> GetReviewCommentsForReview(int reviewId);
    }

    public class ReviewCommentHistoryRepository : IReviewCommentHistoryRepository
    {
        public List<ReviewCommentHistoryObject> GetReviewComments(int mainId) => Config.Conn.Query<ReviewCommentHistoryObject>("SELECT * FROM dat_ReviewCommentHistory WHERE MainId = @MainId ORDER BY EntryDate", new { MainId = mainId }).ToList();

        public List<ReviewCommentHistoryObject> GetReviewCommentsForReview(int reviewId) => Config.Conn.Query<ReviewCommentHistoryObject>("SELECT * FROM dat_ReviewCommentHistory WHERE ReviewId = @ReviewId ORDER BY EntryDate", new { ReviewId = reviewId }).ToList();

    }
}
