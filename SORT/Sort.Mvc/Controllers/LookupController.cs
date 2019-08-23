using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sort.Business;
using Sort.Mvc.Models;

namespace Sort.Mvc.Controllers
{
    [Authorize]
    public class LookupController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LocationSuggestionSearch(string text)
        {
            List<string> model = MemoryCache.GetJournalSuggestions().Where(n => n.JournalName.ToLower().Contains(text.ToLower())).Select(m => m.JournalName).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SponsorOrgSuggestionSearch(string text)
        {
            List<string> model = MemoryCache.GetSponsoringOrgs().Where(n => n.Name.ToLower().Contains(text.ToLower())).Select(m => m.Name).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetUserName(string id)
        {
            return Json(EmployeeCache.GetName(id));
        }

        public ActionResult SubjectCategoryDefinitions()
        {
            return File(System.IO.File.ReadAllBytes(Server.MapPath("~/Documents/SubjectCategoryDefinitions.doc")), System.Net.Mime.MediaTypeNames.Application.Octet, "SubjectCategoryDefinitions.doc");
        }

        #region Country
        public ActionResult Countries()
        {
            List<CountryObject> model = MemoryCache.GetCountries(false);
            return View(model);
        }

        public ActionResult EditCountry(int? id)
        {
            CountryObject model = new CountryObject();
            if (id.HasValue)
                model = MemoryCache.GetCountry(id.Value);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditCountry(CountryObject model)
        {
            if (!ModelState.IsValid)
                return View(model);
            model.Save();
            return RedirectToAction("Countries");
        }
        #endregion

        #region Languages
        public ActionResult Languages()
        {
            List<LanguageObject> model = MemoryCache.GetLanguages(false);
            return View(model);
        }

        public ActionResult EditLanguage(int? id)
        {
            LanguageObject model = new LanguageObject();
            if (id.HasValue)
                model = MemoryCache.GetLanguage(id.Value);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditLanguage(LanguageObject model)
        {
            if (!ModelState.IsValid)
                return View(model);
            model.Save();
            return RedirectToAction("Languages");
        }
        #endregion

        #region Journal Suggestions
        public ActionResult JournalSuggestions()
        {
            List<JournalObject> model = MemoryCache.GetJournalSuggestions(false);
            return View(model);
        }

        public ActionResult EditJournalSuggestion(int? id)
        {
            JournalObject model = new JournalObject();
            if (id.HasValue)
                model = MemoryCache.GetJournalSuggestion(id.Value);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditJournalSuggestion(JournalObject model)
        {
            if (!ModelState.IsValid)
                return View(model);
            model.Save();
            return RedirectToAction("JournalSuggestions");
        }
        #endregion

        #region DOE Funding Categories
        public ActionResult DoeFundingCategories()
        {
            List<DoeFundingCategoryObject> model = MemoryCache.GetDoeFunding(false);
            return View(model);
        }

        public ActionResult EditDoeFundingCategory(int? id)
        {
            DoeFundingCategoryObject model = new DoeFundingCategoryObject();
            if (id.HasValue)
                model = MemoryCache.GetDoeFunding(id.Value);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditDoeFundingCategory(DoeFundingCategoryObject model)
        {
            if (!ModelState.IsValid)
                return View(model);
            model.Save();
            return RedirectToAction("DoeFundingCategories");
        }
        #endregion

        #region Funding Types
        public ActionResult FundingTypes()
        {
            List<FundingTypeObject> model = MemoryCache.GetFundingTypes(false);
            return View(model);
        }

        public ActionResult EditFundingType(int? id)
        {
            FundingTypeObject model = new FundingTypeObject();
            if (id.HasValue)
                model = MemoryCache.GetFundingType(id.Value);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditFundingType(FundingTypeObject model)
        {
            if (!ModelState.IsValid)
                return View(model);
            model.Save();
            return RedirectToAction("FundingTypes");
        }
        #endregion

        #region SPP Category Federal Agencies
        public ActionResult SppFederalAgencies()
        {
            List<SppCategoryFederalAgencyObject> model = MemoryCache.GetSppFundingFederalAgencies(false);
            return View(model);
        }

        public ActionResult EditSppFederalAgency(int? id)
        {
            SppCategoryFederalAgencyObject model = new SppCategoryFederalAgencyObject();
            if (id.HasValue)
                model = MemoryCache.GetSppFundingFederalAgency(id.Value);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditSppFederalAgency(SppCategoryFederalAgencyObject model)
        {
            if (!ModelState.IsValid)
                return View(model);
            model.Save();
            return RedirectToAction("SppFederalAgencies");
        }
        #endregion

        #region Global Search
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SearchArtifact(SearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.SearchData))
            {
                if (model.Search())
                {
                    if (model.Results != null && model.Results.Count == 1)
                    {
                        return Redirect(model.Results[0].Uri);
                    }
                    else if (model.Results != null && model.Results.Count > 1)
                    {
                        model.ReturnUrl = Request.UrlReferrer.AbsoluteUri;
                        return View("SearchResult", model);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}