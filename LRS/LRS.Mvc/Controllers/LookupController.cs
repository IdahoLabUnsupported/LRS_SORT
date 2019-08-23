using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Inl.MvcHelper.Extensions;
using LRS.EmployeeHelper;
using LRS.Business;
using LRS.Mvc.Models;
using EmployeeCache = LRS.Business.EmployeeCache;

namespace LRS.Mvc.Controllers
{
    [Authorize]
    public class LookupController : Controller
    {
        [Authorize(Roles = "Admin,Impersonate")]
        public ActionResult Index()
        {
            return View();
        }

        #region User Info

        [HttpPost]
        public JsonResult GetUserName(string id)
        {
            return Json(EmployeeCache.GetName(id));
        }

        public JsonResult SearchEmployees(string text)
        {
            var model = EmployeeSearchObject.Search(text);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetEmployee(string employeeIds)
        {
            List<string> ids = employeeIds.SplitClean();
            var model = EmployeeSearchObject.Get(ids);
            return Json(model);
        }
        
        [HttpPost]
        public JsonResult GetUserInfo(string id)
        {
            var model = new UserInfoModel(id);
            return Json(model);
        }

        [HttpPost]
        public JsonResult GetIpdsInfo(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                ApiResponse response = ApiRequest.GetIpdsInfo(id);
                if (response.Success)
                {
                    return Json(response.IdpsResponse);
                }
            }
            
            return null;
        }

        [HttpPost]
        public JsonResult ValidateLoiessTrackingNumber(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                ApiResponse response = ApiRequest.ValidateLoiessTrackingNumber(id);
                if (response.Success)
                {
                    return Json(response.LoiessResponse);
                }
            }

            return null;
        }
        #endregion

        #region Searches

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

        public ActionResult SubjectCategoryDefinitions()
        {
            return File(System.IO.File.ReadAllBytes(Server.MapPath("~/Documents/SubjectCategoryDefinitions.doc")), System.Net.Mime.MediaTypeNames.Application.Octet, "SubjectCategoryDefinitions.doc");
        }

        #endregion

        #region Country

        [Authorize(Roles = "Admin")]
        public ActionResult Countries()
        {
            List<CountryObject> model = MemoryCache.GetCountries(false);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditCountry(int? id)
        {
            CountryObject model = new CountryObject();
            if (id.HasValue)
                model = MemoryCache.GetCountry(id.Value);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public ActionResult Languages()
        {
            List<LanguageObject> model = MemoryCache.GetLanguages(false);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditLanguage(int? id)
        {
            LanguageObject model = new LanguageObject();
            if (id.HasValue)
                model = MemoryCache.GetLanguage(id.Value);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditLanguage(LanguageObject model)
        {
            if (!ModelState.IsValid)
                return View(model);
            model.Save();
            return RedirectToAction("Languages");
        }
        #endregion

        #region DOE Funding Categories

        [Authorize(Roles = "Admin")]
        public ActionResult DoeFundingCategories()
        {
            List<DoeFundingCategoryObject> model = MemoryCache.GetDoeFunding(false);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditDoeFundingCategory(int? id)
        {
            DoeFundingCategoryObject model = new DoeFundingCategoryObject();
            if (id.HasValue)
                model = MemoryCache.GetDoeFunding(id.Value);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public ActionResult FundingTypes()
        {
            List<FundingTypeObject> model = MemoryCache.GetFundingTypes(false);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditFundingType(int? id)
        {
            FundingTypeObject model = new FundingTypeObject();
            if (id.HasValue)
                model = MemoryCache.GetFundingType(id.Value);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public ActionResult SppFederalAgencies()
        {
            List<SppCategoryFederalAgencyObject> model = MemoryCache.GetSppFundingFederalAgencies(false);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditSppFederalAgency(int? id)
        {
            SppCategoryFederalAgencyObject model = new SppCategoryFederalAgencyObject();
            if (id.HasValue)
                model = MemoryCache.GetSppFundingFederalAgency(id.Value);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditSppFederalAgency(SppCategoryFederalAgencyObject model)
        {
            if (!ModelState.IsValid)
                return View(model);
            model.Save();
            return RedirectToAction("SppFederalAgencies");
        }
        #endregion

        #region Generic Review Data

        [Authorize(Roles = "Admin")]
        public ActionResult GenericReviewData()
        {
            return View(new GenericReviewModel(MemoryCache.GetGenericReviewData()));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GenericReviewData(GenericReviewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Save();

            return RedirectToAction("Index");
        }

        #endregion

        #region Generic Review Users

        [Authorize(Roles = "Admin")]
        public ActionResult GenericReviewUsers(ReviewerTypeEnum? type = null)
        {
            return View(new GenericReviewUsersModel(type));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditGenericReviewUser(int? id, ReviewerTypeEnum type)
        {
            GenericReviewUserObject model = new GenericReviewUserObject();
            model.ReviewerTypeEnum = type;
            if (id.HasValue)
                model = MemoryCache.GetGenericUser(id.Value);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditGenericReviewUser(GenericReviewUserObject model)
        {
            if (!ModelState.IsValid)
                return View(model);
            model.Save();
            return RedirectToAction("GenericReviewUsers", new { type = model.ReviewerType });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RemoveGenericReviewUser(int? id, ReviewerTypeEnum? type)
        {
            if (id.HasValue)
            {
                MemoryCache.GetGenericUser(id.Value)?.Delete();
            }

            return RedirectToAction("GenericReviewUsers", new {type});
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