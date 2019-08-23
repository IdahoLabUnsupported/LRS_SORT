using System.Collections.Generic;
using System.Web.Mvc;
using Inl.MvcHelper;
using Sort.Business;
using Sort.Mvc.Models;
using Sort.Mvc.Models.Report;

namespace Sort.Mvc.Controllers
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
            
            return File(model.GenerateExcelFile(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SortReport.xlsx");
        }
    }
}