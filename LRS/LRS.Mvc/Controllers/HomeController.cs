using System;
using System.Web.Mvc;
using System.Collections;
using System.Web;
using Inl.MvcHelper;
using LRS.Business;
using LRS.Mvc.Models;
using Inl.MvcHelper.Grid;

namespace LRS.Mvc.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HomeModel model = null;
            // check to see if there is a cookie
            var cookie = Request.Cookies[$"{Current.User.EmployeeId}_ViewMode"];
            if (cookie != null)
            {
                ViewModeOptionEnum viewMode = cookie.Value.ToEnum<ViewModeOptionEnum>();
                int viewTime = 12;
                int orgMode = 1;
                string orgOption = string.Empty;
                if (cookie.HasKeys)
                {
                    viewTime = cookie.Values["viewTime"]?.ToInt() ?? 12;
                    viewMode = cookie.Values["viewMode"].ToEnum<ViewModeOptionEnum>();
                    orgMode = cookie.Values["orgMode"]?.ToInt() ?? 1;
                    orgOption = cookie.Values["orgOption"];
                }

                model = new HomeModel(viewMode, viewTime, orgMode, orgOption);
            }
            else
            {
                model = new HomeModel();
            }

            if (model.IsSmall)
            {
                return View(Current.User.IsInAnyRole("Admin,ReleaseOfficial,GenericReleaseUser") ? "Manager" : (Current.User.IsInAnyRole("OrgManager") ? "OrgManager" : "Index"), model);
            }

            return View(Current.User.IsInAnyRole("Admin,ReleaseOfficial,GenericReleaseUser") ? "ManagerLarge" : (Current.User.IsInAnyRole("OrgManager") ? "OrgManagerLarge" : "IndexLarge"), model);
        }

        [HttpPost]
        public ActionResult Index(ViewModeOptionEnum viewMode, int? viewTime, int? orgMode, string orgOption)
        {
            viewTime = viewTime ?? 0;
            orgMode = orgMode ?? 1;
            var cookie = new HttpCookie($"{Current.User.EmployeeId}_ViewMode", viewMode.ToString());
            cookie.Values["viewMode"] = viewMode.ToString();
            cookie.Values["viewTime"] = viewTime.ToString();
            cookie.Values["OrgMode"] = orgMode.ToString();
            cookie.Values["orgOption"] = orgOption;
            cookie.Expires = DateTime.Now.AddDays(1);
            Response.SetCookie(cookie);

            var model = new HomeModel(viewMode, viewTime, orgMode, orgOption);
            if (model.IsSmall)
            {
                return View(Current.User.IsInAnyRole("Admin,ReleaseOfficial,GenericReleaseUser") ? "Manager" : (Current.User.IsInAnyRole("OrgManager") ? "OrgManager" : "Index"), model);
            }

            return View(Current.User.IsInAnyRole("Admin,ReleaseOfficial,GenericReleaseUser") ? "ManagerLarge" : (Current.User.IsInAnyRole("OrgManager") ? "OrgManagerLarge" : "IndexLarge"), model);
        }

        public JsonResult HomGridData(GridDataRequestObject<MainObject> req, ViewModeOptionEnum viewMode, int? viewTime, int? orgMode, string orgOption)
        {
            return Json(req.GetResult(HomeModel.GetMainData(viewMode, viewTime, orgMode, orgOption)));
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

        
    }
}