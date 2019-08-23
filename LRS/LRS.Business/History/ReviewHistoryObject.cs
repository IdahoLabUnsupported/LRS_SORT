using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Inl.MvcHelper;

namespace LRS.Business
{
    public class ReviewHistoryObject
    {
        #region Properties

        public int ReviewId { get; set; }
        public int MainId { get; set; }
        public DateTime EntryDate { get; set; }
        public string ReviewerEmployeeId { get; set; }
        public string ReviewerType { get; set; }
        public DateTime? ReviewDate { get; set; }
        public bool Required { get; set; }
        public bool SystemReviewer { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public DateTime? StatusDate { get; set; }
        public DateTime HistoryDate { get; set; }

        #endregion

        #region Extended Properties

        public string ReviewerName => string.IsNullOrWhiteSpace(ReviewerEmployeeId) ? string.Empty : EmployeeCache.GetEmployee(ReviewerEmployeeId, true)?.FullName;

        public string Email => EmployeeCache.GetEmployee(ReviewerEmployeeId)?.Email;

        public ReviewerTypeEnum ReviewerTypeEnum
        {
            get => ReviewerType.ToEnum<ReviewerTypeEnum>();
            set => ReviewerType = value.ToString();
        }

        public ReviewStatusEnum ReviewStatus
        {
            get => Status.ToEnum<ReviewStatusEnum>();
            set => Status = value.ToString();
        }

        public string ReviewStatusTxt => ReviewStatus.GetEnumDisplayName();

        public string ReviewerTypeDisplayName => ReviewerTypeEnum.GetEnumDisplayName();
        
        [Display(Name = "Reviewer Comments")]
        public string CommentsTable
        {
            get
            {
                var comments = ReviewCommentHistoryObject.GetReviewCommentsForReview(ReviewId);
                if (comments != null && comments.Count > 0)
                {
                    StringBuilder table = new StringBuilder();
                    table.Append("<table class=\"table table-condensed\"> ");
                    table.Append("<thead>");
                    table.Append("<tr class=\"active\">");
                    if (ReviewerTypeEnum == ReviewerTypeEnum.Classification ||
                        ReviewerTypeEnum == ReviewerTypeEnum.ExportControl ||
                        ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment)
                    {
                        table.Append("<th>Comment By</th>");
                    }
                    table.Append("<th>Date</th>");
                    table.Append("<th>Comment</th>");
                    table.Append("</tr>");
                    table.Append("</thead>");
                    table.Append("<tbody>");
                    foreach (var c in comments)
                    {
                        table.Append("<tr class=\"active\">");
                        if (ReviewerTypeEnum == ReviewerTypeEnum.Classification ||
                            ReviewerTypeEnum == ReviewerTypeEnum.ExportControl ||
                            ReviewerTypeEnum == ReviewerTypeEnum.TechDeployment)
                        {
                            table.Append("<td>" + c.ReviewerName + "</td>");
                        }
                        table.Append("<td>" + c.EntryDate.ToString("MM/dd/yyyy") + "</td>");
                        table.Append("<td>" + c.Comment + "</td>");
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
        public string HistoryDateStr => HistoryDate.ToString("MM/dd/yyyy hh:mm tt");
        public string MainTitle => MainObject.GetMain(MainId)?.DisplayTitle;
        public string NumberPagesStr => MainObject.GetMain(MainId)?.NumberPagesStr;
        #endregion

        #region Constructor

        public ReviewHistoryObject() { }

        #endregion

        #region Repository

        private static IReviewHistoryRepository repo => new ReviewHistoryRepository();

        #endregion

        #region Static Methods

        public static List<ReviewHistoryObject> GetReviewHistories(int mainId) => repo.GetReviewHistories(mainId);

        #endregion

        #region Object Methods
        
        #endregion
    }

    public interface IReviewHistoryRepository
    {
        List<ReviewHistoryObject> GetReviewHistories(int mainId);
    }

    public class ReviewHistoryRepository : IReviewHistoryRepository
    {
        public List<ReviewHistoryObject> GetReviewHistories(int mainId) => Config.Conn.Query<ReviewHistoryObject>("SELECT * FROM dat_ReviewHistory WHERE MainId = @MainId", new { MainId = mainId }).ToList();

    }
}