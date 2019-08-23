using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class GenericReviewUsersModel
    {
        public ReviewerTypeEnum? ReviewerType { get; set; }
        public List<GenericReviewUserObject> Users { get; set; }

        public GenericReviewUsersModel() { }

        public GenericReviewUsersModel(ReviewerTypeEnum? reviewerType)
        {
            ReviewerType = reviewerType;

            if (ReviewerType.HasValue)
            {
                Users = MemoryCache.GetGenericReviewUsers(ReviewerType.Value);
            }
        }
    }
}