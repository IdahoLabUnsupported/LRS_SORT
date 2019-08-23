using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Sort.Mvc.Controllers
{
    public class ErrorController : Controller
    {
        /// <summary>
        /// Returns an HTTP 400 Bad Request error view. Returns a partial view if the request is an AJAX call.
        /// </summary>
        /// <returns>The partial or full bad request view.</returns>
        [OutputCache(CacheProfile = "BadRequest")]
        public ActionResult BadRequest()
        {
            return this.getErrorView(HttpStatusCode.BadRequest, "BadRequest");
        }

        /// <summary>
        /// Returns a HTTP 403 Forbidden error view. Returns a partial view if the request is an AJAX call.
        /// Unlike a 401 Unauthorized response, authenticating will make no difference
        /// </summary>
        /// <returns>The partial or full forbidden view.</returns>
        [OutputCache(CacheProfile = "Forbidden")]
        public ActionResult Forbidden()
        {
            return this.getErrorView(HttpStatusCode.Forbidden, "Forbidden");
        }

        /// <summary>
        /// Returns a HTTP 500 Internal Server Error error view. Returns a partial view if the request is an AJAX call.
        /// </summary>
        /// <returns>The partial or full internal server error view.</returns>
        [OutputCache(CacheProfile = "InternalServerError")]
        public ActionResult InternalServerError()
        {
            return this.getErrorView(HttpStatusCode.InternalServerError, "InternalServerError");
        }

        /// <summary>
        /// Returns a HTTP 405 Method Not Allowed error view. Returns a partial view if the request is an AJAX call.
        /// </summary>
        /// <returns>The partial or full method not allowed view.</returns>
        [OutputCache(CacheProfile = "MethodNotAllowed")]
        public ActionResult MethodNotAllowed()
        {
            return this.getErrorView(HttpStatusCode.MethodNotAllowed, "MethodNotAllowed");
        }

        /// <summary>
        /// Returns a HTTP 404 Not Found error view. Returns a partial view if the request is an AJAX call.
        /// </summary>
        /// <returns>The partial or full not found view.</returns>
        [OutputCache(CacheProfile = "NotFound")]
        public ActionResult NotFound()
        {
            return this.getErrorView(HttpStatusCode.NotFound, "NotFound");
        }

        /// <summary>
        /// Returns a HTTP 401 Unauthorized error view. Returns a partial view if the request is an AJAX call.
        /// </summary>
        /// <returns>The partial or full unauthorized view.</returns>
        [OutputCache(CacheProfile = "Unauthorized")]
        public ActionResult Unauthorized()
        {
            return this.getErrorView(HttpStatusCode.Unauthorized, "Unauthorized");
        }

        #region Private Methods

        private ActionResult getErrorView(HttpStatusCode statusCode, string viewName)
        {
            this.Response.StatusCode = (int)statusCode;

            // Don't show IIS custom errors
            this.Response.TrySkipIisCustomErrors = true;

            ActionResult result;
            if (this.Request.IsAjaxRequest())
            {
                // This allows us to show errors even in partial views
                result = this.PartialView(viewName);
            }
            else
            {
                result = this.View(viewName);
            }

            return result;
        }

        #endregion
    }
}