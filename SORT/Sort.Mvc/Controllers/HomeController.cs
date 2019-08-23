using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Inl.MvcHelper.Extensions;
using System.Collections;
using Inl.MvcHelper.Grid;
using Sort.Business;
using Sort.EmployeeHelper;
using Sort.Mvc.Models;

namespace Sort.Mvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var view = Config.User.IsInAnyRole("Admin,ReleaseOfficial,ReadAll") ? "Manager" : (Config.User.IsInAnyRole("OrgManager") ? "OrgManager" : "Index");
            HomeModel model = null;

            // check to see if there is a cookie
            var cookie = Request.Cookies[$"{UserObject.CurrentUser.EmployeeId}_ViewMode"];
            if (cookie != null)
            {
                ViewModeOptionEnum viewMode = cookie.Value.ToEnum<ViewModeOptionEnum>();
                bool? showPublished = null;
                int orgMode = 1;
                string orgOption = string.Empty;
                if (cookie.HasKeys)
                {
                    showPublished = cookie.Values["ShowPublished"]?.ToBool();
                    viewMode = cookie.Values["viewMode"].ToEnum<ViewModeOptionEnum>();
                    orgMode = cookie.Values["orgMode"]?.ToInt() ?? 1;
                    orgOption = cookie.Values["orgOption"];
                }

                model = new HomeModel(viewMode, showPublished, orgMode, orgOption);
            }
            else
            {
                model = new HomeModel();
            }

            if (model.IsSmall)
            {
                return View(Config.User.IsInAnyRole("Admin,ReleaseOfficial,ReadAll") ? "Manager" : (Config.User.IsInAnyRole("OrgManager") ? "OrgManager" : "Index"), model);
            }

            return View(Config.User.IsInAnyRole("Admin,ReleaseOfficial,ReadAll") ? "ManagerLarge" : (Config.User.IsInAnyRole("OrgManager") ? "OrgManagerLarge" : "IndexLarge"), model);
        }

        [HttpPost]
        public ActionResult Index(ViewModeOptionEnum viewMode, bool showPublished, int? orgMode, string orgOption)
        {
            orgMode = orgMode ?? 1;
            var view = Config.User.IsInAnyRole("Admin,ReleaseOfficial,ReadAll") ? "Manager" : (Config.User.IsInAnyRole("OrgManager") ? "OrgManager" : "Index");
            HttpCookie cookie = new HttpCookie($"{UserObject.CurrentUser.EmployeeId}_ViewMode", viewMode.ToString());
            cookie.Values["ShowPublished"] = showPublished.ToString();
            cookie.Values["viewMode"] = viewMode.ToString();
            cookie.Values["OrgMode"] = orgMode.ToString();
            cookie.Values["orgOption"] = orgOption;
            cookie.Expires = DateTime.Now.AddDays(1);
            Response.SetCookie(cookie);

            var model = new HomeModel(viewMode, showPublished, orgMode, orgOption);

            if (model.IsSmall)
            {
                return View(Config.User.IsInAnyRole("Admin,ReleaseOfficial,ReadAll") ? "Manager" : (Config.User.IsInAnyRole("OrgManager") ? "OrgManager" : "Index"), model);
            }

            return View(Config.User.IsInAnyRole("Admin,ReleaseOfficial,ReadAll") ? "ManagerLarge" : (Config.User.IsInAnyRole("OrgManager") ? "OrgManagerLarge" : "IndexLarge"), model);
        }

        public JsonResult HomGridData(GridDataRequestObject<SortMainObject> req, ViewModeOptionEnum viewMode, int? orgMode, string orgOption)
        {
            return Json(req.GetResult(HomeModel.GetMainData(viewMode, orgMode, orgOption)));
        }

        public ActionResult About()
		{
			return View();
		}

		public ActionResult Contact()
		{
			return View();
        }
        public ActionResult ClearCache()
        {
            var cache = HttpContext.Cache;
            IDictionaryEnumerator CacheEnum = cache.GetEnumerator();
            while (CacheEnum.MoveNext())
            {
                string cacheKey = ((System.Collections.DictionaryEntry)CacheEnum.Current).Key.ToString();
                cache.Remove(cacheKey);
            }

            return RedirectToAction("Index");
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

        public ActionResult TestSharepointUpdate(int? id)
        {
            if (id.HasValue)
            {
                var sort = SortMainObject.GetSortMain(id.Value);
                if (sort != null)
                {
                    try
                    {
                        StimsData.SendToLoiess(sort);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogObject.LogError("TestSharepointUpdate", ex);
                        TempData.Add("FailMessage", ex.Message);
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }

        
    }
}