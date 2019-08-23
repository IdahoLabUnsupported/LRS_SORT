using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class OutstandingReviewsModel
    {
        public List<ReviewObject> Reviews { get; } = new List<ReviewObject>();
        public OutstandingReviewsModel()
        {
            Reviews = ReviewObject.GetAllActiveReviews();
        }
    }
}