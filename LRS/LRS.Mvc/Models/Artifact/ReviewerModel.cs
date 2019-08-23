using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Inl.MvcHelper;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class ReviewerModel : IValidatableObject
    {
        public int? ReviewId { get; set; }
        [Required]
        public int MainId { get; set; }
        [Required, Display(Name = "Reviewer")]
        public string ReviewerEmployeeId { get; set; }
        [Required, Display(Name = "Reviewer Type")]
        public string ReviewerType { get; set; }
        public List<ReviewObject> Reviews { get; set; }
        public List<ReviewCommentObject> ReviewComments { get; set; }

        public ReviewerTypeEnum ReviewerTypeEnum
        {
            get => ReviewerType.ToEnum<ReviewerTypeEnum>();
            set => ReviewerType = value.ToString();
        }

        public ReviewerModel() { }

        public ReviewerModel(int mainId, int? reviewId)
        {
            MainId = mainId;
            Reviews = ReviewObject.GetReviews(mainId);
            ReviewComments = ReviewCommentObject.GetReviewComments(mainId);

            if (reviewId.HasValue)
            {
                var review = ReviewObject.GetReview(reviewId.Value);
                if (review != null)
                {
                    reviewId = review.ReviewId;
                    ReviewerType = review.ReviewerType;
                    ReviewerEmployeeId = review.ReviewerEmployeeId;
                }
            }
        }

        public void Save()
        {
            bool isNew = false;
            var review = ReviewObject.GetReview(ReviewId ?? 0);
            if (review == null)
            {
                review = new ReviewObject();
                isNew = true;
            }
            review.MainId = MainId;
            review.ReviewerEmployeeId = ReviewerEmployeeId;
            review.ReviewerFirstName = EmployeeCache.GetEmployee(ReviewerEmployeeId)?.FirstName;
            review.ReviewerLastName = EmployeeCache.GetEmployee(ReviewerEmployeeId)?.LastName;
            review.ReviewerType = ReviewerType;
            review.Required = ReviewerTypeEnum == ReviewerTypeEnum.Manager;
            review.ReviewStatus = string.IsNullOrWhiteSpace(review.Status) ? ReviewStatusEnum.New : review.ReviewStatus;
            review.Save(isNew);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReviewerEmployeeId.Equals(Current.User.EmployeeId, StringComparison.InvariantCultureIgnoreCase))
            {
                yield return new ValidationResult("You cannot assign yourself as a reviewer.", new[] { "ReviewerEmployeeId" });
            }
            else if(ReviewObject.GetReviews(MainId).Exists(n => n.ReviewerTypeEnum != ReviewerTypeEnum.PeerTechnical && ReviewerEmployeeId.Equals(n.ReviewerEmployeeId, StringComparison.InvariantCultureIgnoreCase)))
            {
                yield return new ValidationResult("Reviewer already exists! You can only use a Reviewer once.", new[] { "ReviewerEmployeeId" });
            }
            else if (ReviewerTypeEnum != ReviewerTypeEnum.PeerTechnical && AuthorObject.GetAuthors(MainId).Exists(n => ReviewerEmployeeId.Equals(n.EmployeeId, StringComparison.InvariantCultureIgnoreCase)))
            {
                yield return new ValidationResult("Reviewer is an Author! A reviewer can not be a author.", new[] { "ReviewerEmployeeId" });
            }
        }
    }
}