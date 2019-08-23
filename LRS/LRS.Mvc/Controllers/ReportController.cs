using System;
using System.Linq;
using System.Web.Mvc;
using Inl.MvcHelper;
using LRS.Business;
using LRS.Mvc.Models;

namespace LRS.Mvc.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            return View(new ReportModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(ReportModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ViewBag.Title = "Report Results";
            var result = model.ExecuteReport();
            return View("ReportResults", result);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ExportReport(ReportExportModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            return File(model.GenerateExcelFile(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LRSReport.xlsx");
        }

        public ActionResult PendingReviews()
        {
            return View(PendingReviewReport.GetPendingReviews());
        }

        public ActionResult OrgReviewReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            var model = new OrgReviewReportModel(startDate, endDate);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PendingReviewExcel()
        {
            byte[] excelFile = Talon.Excel(PendingReviewReport.GetPendingReviews().OrderByDescending(n => n.Age)).Columns(c =>
            {
                c.Bound(m => m.ArtifactDisplayTitle).Title("STI Number and Rev").Width(100);
                c.Bound(m => m.ReviewerName).Title("Reviewer").Width(150);
                c.Bound(m => m.ReviewerOrg).Title("Reviewer Org").Width(150);
                c.Bound(m => m.ReviewerTypeDisplayName).Title("Review Type").Width(150);
                c.Bound(m => m.ReviewStatusDisplayName).Title("Review Status").Width(100);
                c.Bound(m => m.ReviewStartDateStr).Title("Review Start Date").Width(100);
                c.Bound(m => m.Age).Title("Age").Width(100);
            }).Export();

            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"LRS_PendingReviews_{DateTime.Now:MM-dd-yyyy}.xlsx");
        }
    }
}