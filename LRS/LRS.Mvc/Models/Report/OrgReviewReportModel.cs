using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LRS.Business;

namespace LRS.Mvc.Models
{
    public class OrgReviewReportModel
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<OrgReviewReport> Data { get; set; } = new List<OrgReviewReport>();

        public OrgReviewReportModel()
        {
            DateTime curr = DateTime.Now;
            DateTime furt = curr.AddMonths(1);

            StartDate = new DateTime(curr.Year, curr.Month, 1, 0, 0, 0);
            EndDate = new DateTime(furt.Year, furt.Month, 1, 0, 0, 0);
        }

        public OrgReviewReportModel(DateTime? startDate, DateTime? endDate)
        {
            DateTime curr = DateTime.Now;
            DateTime furt = curr.AddMonths(1);

            StartDate = startDate?.Date ?? new DateTime(curr.Year, curr.Month, 1, 0, 0, 0); ;
            EndDate = endDate?.Date.AddDays(1) ?? new DateTime(furt.Year, furt.Month, 1, 0, 0, 0); 

            if (StartDate.HasValue && EndDate.HasValue)
            {
                Data = OrgReviewReport.GetReviews(StartDate.Value, EndDate.Value);
            }
        }
    }
}