using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Inl.MvcHelper;

namespace LRS.Business
{
    public class OrgReviewReport
    {
        #region Properties
        public string ReviewerType { get; set; }
        public int NumReviews { get; set; }
        #endregion

        #region Extended Properties
        public List<OrgReviewIndividual> IndividualReviews { get; set; } = new List<OrgReviewIndividual>();
        public ReviewerTypeEnum ReviewType => ReviewerType.ToEnum<ReviewerTypeEnum>();
        public string ReviewTypeStr => ReviewType.GetEnumDisplayName();
        #endregion

        #region Constructor
        
        public OrgReviewReport() { }
        #endregion

        #region Static Functions
        public static List<OrgReviewReport> GetReviews(DateTime startDate, DateTime endDate)
        {
            string sql = @" select ReviewerType, count(*) as NumReviews
                            From dat_Review
                            where ReviewerType in ('Classification','TechDeployment','ExportControl')
                            and [Status] NOT IN ('New', 'Active')
                            and ReviewDate between @StartDate and @EndDate
                            Group by ReviewerType
                            order by ReviewerType";

            var data = Config.Conn.Query<OrgReviewReport>(sql, new { StartDate = startDate, EndDate = endDate }).ToList();
            data.ForEach(n => n.IndividualReviews = OrgReviewIndividual.GetReviews(startDate, endDate, n.ReviewerType));

            return data;
        }
        #endregion

    }

    public class OrgReviewIndividual
    {
        #region Properties
        public string ReviewerType { get; set; }
        public string ReviewerEmployeeId { get; set; }
        public string Name => EmployeeCache.GetName(ReviewerEmployeeId);
        public int NumReviews { get; set; }
        #endregion

        #region Extended Properties
        public ReviewerTypeEnum ReviewType => ReviewerType.ToEnum<ReviewerTypeEnum>();

        public string ReviewTypeStr => ReviewType.GetEnumDisplayName();
        #endregion

        #region Constructor
        public OrgReviewIndividual() { }
        #endregion

        #region Static Functions
        internal static List<OrgReviewIndividual> GetReviews(DateTime startDate, DateTime endDate, string reviewType)
        {
            string sql = @" select r.ReviewerType, r.ReviewerEmployeeId, count(*) as NumReviews
                            From dat_Review r
                            where r.ReviewerType = @ReviewerType
                            and r.[Status] NOT IN ('New', 'Active')
                            and r.ReviewDate between @StartDate and @EndDate
                            Group by r.ReviewerType, r.ReviewerEmployeeId
                            order by r.ReviewerType, r.ReviewerEmployeeId";

            return Config.Conn.Query<OrgReviewIndividual>(sql, new { StartDate = startDate, EndDate = endDate, ReviewerType = reviewType }).ToList();
        }
        #endregion
    }
}
