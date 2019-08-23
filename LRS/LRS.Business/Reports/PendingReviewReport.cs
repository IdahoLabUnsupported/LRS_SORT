using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Inl.MvcHelper;

namespace LRS.Business
{
    public class PendingReviewReport
    {
        #region Properties
        public int ReviewId { get; set; }
        public int MainId { get; set; }
        public string StiNumber { get; set; }
        public int Revision { get; set; }
        public DateTime MainCreateDate { get; set; }
        public DateTime EntryDate { get; set; }
        public string ReviewerEmployeeId { get; set; }
        public string ReviewerType { get; set; }
        public DateTime? ReviewDate { get; set; }
        public DateTime? ReviewStatusDate { get; set; }
        public bool Required { get; set; }
        public bool SystemReviewer { get; set; }
        public string Status { get; set; }
        #endregion

        #region Extended Properties

        public string ReviewerName => string.IsNullOrWhiteSpace(ReviewerEmployeeId) ? string.Empty : EmployeeCache.GetEmployee(ReviewerEmployeeId)?.FullName;
        public string ReviewerOrg => string.IsNullOrWhiteSpace(ReviewerEmployeeId) ? string.Empty : EmployeeCache.GetEmployee(ReviewerEmployeeId)?.HomeOrginization;
        public string Email => EmployeeCache.GetEmployee(ReviewerEmployeeId)?.Email;
        public ReviewerTypeEnum ReviewerTypeEnum => ReviewerType.ToEnum<ReviewerTypeEnum>();
        public ReviewStatusEnum ReviewStatus => Status.ToEnum<ReviewStatusEnum>();
        public string ReviewStatusDisplayName => ReviewStatus.GetEnumDisplayName();
        public string ReviewerTypeDisplayName => ReviewerTypeEnum.GetEnumDisplayName();
        public string ArtifactCreateDateStr => MainCreateDate.ToString("MM/dd/yyyy hh:mm tt");
        public string ReviewStartDateStr => EntryDate.ToString("MM/dd/yyyy hh:mm tt");
        public string ReviewLastActivityStr => ReviewStatusDate?.ToString("MM/dd/yyyy hh:mm tt");
        public List<GenericReviewUserObject> SystemReviewerUsers => MemoryCache.GetGenericReviewUsers(ReviewerTypeEnum) ?? new List<GenericReviewUserObject>();
        public string ArtifactRevisionStr => $"{Revision:D3}";
        public string ArtifactDisplayTitle => $"{StiNumber} Rev:{ArtifactRevisionStr}";
        public string Age => $"{(DateTime.Now - EntryDate).Days} Days";
        #endregion

        #region Constructor

        public PendingReviewReport() { }

        #endregion

        #region Static Methods

        public static List<PendingReviewReport> GetPendingReviews()
        {
            string sql = @" select  m.MainId,
                                    m.StiNumber, 
                                    m.Revision, 
                                    m.CreateDate as 'MainCreateDate', 
		                            r.EntryDate,
		                            r.ReviewerEmployeeId,
		                            R.ReviewerType,
		                            r.ReviewerType,
		                            r.ReviewDate,
		                            r.StatusDate as 'ReviewStatusDate',
		                            r.[Required],
		                            r.SystemReviewer,
		                            r.[Status]
                            from dat_main m
                            inner join dat_Review r on r.MainId = m.MainId
	                            and r.[Status] = 'Active'
                            where m.[Status] in ('InPeerReview', 'InReview')";

            return Config.Conn.Query<PendingReviewReport>(sql).ToList();
        }

        #endregion
    }

}
