using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LRS.Mvc.Controllers
{
    [AllowAnonymous]
    public class EdmsController : Controller
    {
        public ActionResult SortHome()
        {
            return Redirect(Config.SortUrl);
        }
    }
}