using LRS.Business;

namespace LRS.Mvc.Models
{
    public class CommentModel
    {
        public int MainId { get; set; }
        public int? CommentReviewId { get; set; }
        public int? CommentId { get; set; }
        public string Comment { get; set; }
        public string RejectComment { get; set; }

        public string StiNumber => MainObject.GetMain(MainId)?.DisplayTitle;

        public CommentModel() { }

        public CommentModel(int mainId, int? reviewId)
        {
            MainId = mainId;
            CommentReviewId = reviewId;
        }

        public void Save()
        {
            if (CommentId.HasValue)
            {
                var c = ReviewCommentObject.GetReviewComment(CommentId.Value);
                if (c != null)
                {
                    c.Comment = Comment;
                    c.Save();
                }
            }
            else
            {
                var c = new ReviewCommentObject();
                c.MainId = MainId;
                c.ReviewId = CommentReviewId.Value;
                c.Comment = Comment;
                c.EmployeeId = Current.User.EmployeeId;
                c.Save();
            }
        }

        public void SaveRejection()
        {
            MainObject.GetMain(MainId)?.MarkReviewRejected(CommentReviewId.Value, RejectComment);
        }
    }
}