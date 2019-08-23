using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sort.Business
{
    public class ReviewObject
    {
        #region Properties

        public int ReviewId { get; set; }
        public int SortMainId { get; set; }
        public string Reviewer { get; set; }
        public string ReviewerType { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string Approval { get; set; }
        public string Access { get; set; }
        public string Reason { get; set; }
        public string AccessReason { get; set; }
        public string OfficialUse { get; set; }
        public string Comments { get; set; }

        #endregion

        #region Extended Properties

        public ReviewerTypeEnum ReviewerTypeEnum
        {
            get { return ReviewerType.ToEnum<ReviewerTypeEnum>(); }
            set { ReviewerType = value.ToString(); }
        }

        public string ReviewerTypeDisplayName => ReviewerTypeEnum.GetEnumDisplayName();

        public string SortTitle { get; set; }
        #endregion

        #region Constructor

        public ReviewObject() { }

        #endregion

        #region Repository

        private static IReviewRepository repo => new ReviewRepository();

        #endregion

        #region Static Methods

        public static List<ReviewObject> GetReviews(int sortMainId) => repo.GetReviews(sortMainId);

        public static ReviewObject GetReview(int reviewId) => repo.GetReview(reviewId);
        public static ReviewObject GetReview(int sortMainId, string reviewerType) => repo.GetReview(sortMainId, reviewerType);

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveReview(this);
        }

        public bool Delete()
        {
            return repo.DeleteReview(this);
        }

        #endregion
    }

    public interface IReviewRepository
    {
        List<ReviewObject> GetReviews(int sortMainId);
        ReviewObject GetReview(int reviewId);
        ReviewObject GetReview(int sortMainId, string reviewerType);
        ReviewObject SaveReview(ReviewObject review);
        bool DeleteReview(ReviewObject review);
    }

    public class ReviewRepository : IReviewRepository
    {
        public List<ReviewObject> GetReviews(int sortMainId)
        {
            string sql = @" SELECT r.*, s.Title as 'SortTitle' 
                            FROM dat_Review r 
                            inner join dat_SortMain s on s.SortMainId = r.SortMainId
                            WHERE r.SortMainId = @SortMainId";

            return Config.Conn.Query<ReviewObject>(sql, new { SortMainId = sortMainId }).ToList();
        }

        public ReviewObject GetReview(int reviewId)
        {
            string sql = @" SELECT r.*, s.Title as 'SortTitle' 
                            FROM dat_Review r 
                            inner join dat_SortMain s on s.SortMainId = r.SortMainId
                            WHERE r.ReviewId = @ReviewId";

            return Config.Conn.Query<ReviewObject>(sql, new { ReviewId = reviewId }).FirstOrDefault();
        }

        public ReviewObject GetReview(int sortMainId, string reviewerType)
        {
            string sql = @" SELECT r.*, s.Title as 'SortTitle' 
                            FROM dat_Review r 
                            inner join dat_SortMain s on s.SortMainId = r.SortMainId
                            WHERE r.SortMainId = @SortMainId
                            AND r.ReviewerType = @ReviewerType";

            return Config.Conn.Query<ReviewObject>(sql, new { SortMainId = sortMainId, ReviewerType = reviewerType }).FirstOrDefault();
        }

        public ReviewObject SaveReview(ReviewObject review)
        {
            if (review.ReviewId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_Review
                    SET     SortMainId = @SortMainId,
                            Reviewer = @Reviewer,
                            ReviewerType = @ReviewerType,
                            ReviewDate = @ReviewDate,
                            Approval = @Approval,
                            Reason = @Reason,
                            Access = @Access,
                            AccessReason = @AccessReason,
                            OfficialUse = @OfficialUse,
                            Comments = @Comments
                    WHERE   ReviewId = @ReviewId";
                Config.Conn.Execute(sql, review);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_Review (
                        SortMainId,
                        Reviewer,
                        ReviewerType,
                        ReviewDate,
                        Approval,
                        Reason,
                        Access,
                        AccessReason,
                        OfficialUse,
                        Comments
                    )
                    VALUES (
                        @SortMainId,
                        @Reviewer,
                        @ReviewerType,
                        @ReviewDate,
                        @Approval,
                        @Reason,
                        @Access,
                        @AccessReason,
                        @OfficialUse,
                        @Comments
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                review.ReviewId = Config.Conn.Query<int>(sql, review).Single();
            }
            return review;
        }

        public bool DeleteReview(ReviewObject review)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_Review WHERE ReviewId = @ReviewId", review);
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("ReviewObject::DeleteReview", ex);
                return false;
            }
            return true;
        }
    }
}
