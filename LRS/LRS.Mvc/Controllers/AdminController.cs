using LRS.Business;
using LRS.Mvc.Models;
using LRS.Mvc.Models.Admin;
using System.Web.Mvc;

namespace LRS.Mvc.Controllers
{
    [Authorize(Roles = "Admin,Impersonate")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

		#region User Stuff

		public ActionResult Users()
		{
			var model = UserListObject.GetUsers();
			return View(model);
		}

		public ActionResult EditUser(string id)
		{
			EditUserModel model = new EditUserModel(id);
			return View(model);
		}

		[HttpPost]
		public ActionResult EditUser(EditUserModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			model.Save();
		    if (model.Roles.Contains(UserRole.OrgManager.ToString()))
		    {
		        return RedirectToAction("EditUser", new { id = model.EmployeeId });
		    }
            return RedirectToAction("Users");
		}

        public ActionResult DeleteUser(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                new EditUserModel(id)?.Delete();
            }

            return RedirectToAction("Users");
        }

        public JsonResult GetCurrentRole(string id)
		{
			EditUserModel model = new EditUserModel(id);
			return Json(model, JsonRequestBehavior.AllowGet);
		}

        public ActionResult UserOrgsPartial(string id)
        {
            var model = new UserOrgsModel(id);

            return PartialView("Partials/_userOrgs", model);
        }

        public ActionResult AddUserOrgs(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Users");
            }

            var model = new UserOrgsModel(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult AddUserOrgs(UserOrgsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Save();

            return RedirectToAction("EditUser", new {id = model.UserEmployeeId});
        }

        [HttpPost]
        public ActionResult RemoveUserOrg(int? id, string user)
        {
            if (id.HasValue)
            {
                UserOrgObject.GetUserOrg(id.Value)?.Delete();
            }

            return UserOrgsPartial(user);
        }

        #endregion

        #region Impersonate

        public ActionResult Impersonate()
        {
            return View(new ImpersonateModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Impersonate(ImpersonateModel model)
        {
            try
            {
                UserObject toImpersonate = UserObject.GetUser(model.EmployeeId);
                HttpContext.Session["ImpersonateAs"] = toImpersonate.UserName;
            }
            catch
            {
                ModelState.AddModelError("EmployeeId", "Invalid Employee, be sure you select from the suggestion list");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult EndImpersonate()
        {
            HttpContext.Session["ImpersonateAs"] = null;
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Email Template

        public ActionResult EditEmail()
        {
            return View(new EditEmailModel());
        }

        [HttpPost]
        public ActionResult EditEmail(EditEmailModel model)
        {
            ModelState.Clear();
            model.HydrateData();
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveEmail(EditEmailModel model)
        {
            if (ModelState.IsValid)
            {
                model.Save();
            }

            return Json(new { id = model.EmailTemplateId, saved = model.IsSaved });
        }

        public ActionResult DeleteEmail(int? id)
        {
            if (id.HasValue)
            {
                EmailTemplateObject.GetEmailTemplate(id.Value)?.Delete();
            }

            return RedirectToAction("EditEmail");
        }

        #endregion

        #region Outstanding Reviews

        public ActionResult OutstandingReviews(bool? sent)
        {
            if (sent.HasValue)
            {
                TempData["Sent"] = sent.Value;
            }

            return View(new OutstandingReviewsModel());
        }

        public ActionResult SendOutstandingEmails()
        {
            var reviews = new OutstandingReviewsModel().Reviews;
            reviews.ForEach(n => n.SendReminderEmail());

            return RedirectToAction("OutstandingReviews", new {sent = true});
        }

        public ActionResult SendIndividualReminder(int? id)
        {
            if (id.HasValue)
            {
                var review = ReviewObject.GetReview(id.Value);
                if (review != null)
                {
                    review.SendReminderEmail();
                    return RedirectToAction("OutstandingReviews", new { sent = true });
                }
            }

            return RedirectToAction("OutstandingReviews", new { sent = false });
        }
        #endregion

        #region Ping Sort

        public ActionResult PingSort()
        {
            var response = ApiRequest.PingSort();

            return View(response);
        }

        #endregion
    }
}