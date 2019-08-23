using Sort.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sort.Mvc.Classes
{
    public static class Menu
    {
		private static List<MenuItem> baseMenu = new List<MenuItem>() {
			new MenuItem() { Text = "Home", Icon = "home" },
		    new MenuItem() { Text = "Report", Icon = "search", Controller = "Report" },
            new MenuItem() { Text = "Admin", Controller = "Admin", Roles = "Admin", SubMenuItems = new List<MenuItem>() {
				new MenuItem() { Text = "Users", Action = "Users", Icon = "users" },
			    new MenuItem() {Text =  "Code Table", Controller = "Lookup", Icon = "table"},
                new MenuItem() {Text =  "Email Templates", Action = "EditEmail", Icon = "envelope"},
                new MenuItem() {Text =  "OSTI Info", Action = "OstiInfo", Icon = "lock"}
            }},
		    new MenuItem() { Text = "LRS Website", Icon = "globe", Controller = "Edms", Action = "LrsHome"}
        };

		private static List<MenuItem> items;
		public static List<MenuItem> Items {
			get
			{
				if (items == null)
				{
					items = baseMenu;
				}
				return items;
			}
		}

        public static MenuItem GetMainMenu(string controller)
        {
            foreach (MenuItem item in Items)
            {
                if (item.Controller == controller) return item;
            }
            return new MenuItem();
        }

        public static string CurrentController => HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower();
        public static string CurrentAction => HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();

        public static List<KeyValuePair<string, string>> AutoBreadcrumb
        {
            get
            {
                var url = new UrlHelper(HttpContext.Current.Request.RequestContext);
                List<KeyValuePair<string, string>> toReturn = new List<KeyValuePair<string, string>>();

                toReturn.Add(new KeyValuePair<string, string>("Home", url.Action("Index", "Home")));

                if (CurrentController == "home" && CurrentAction == "index")
                    return toReturn;

                MenuItem controllerMatch = Items.FirstOrDefault(m => m.Controller.ToLower() == CurrentController);
                if (controllerMatch != null)
                {
                    toReturn.Add(new KeyValuePair<string, string>(controllerMatch.Text, url.Action(controllerMatch.Action, controllerMatch.Controller)));
                    MenuItem actionMatch = controllerMatch.SubMenuItems.FirstOrDefault(m => m.Action.ToLower() == CurrentAction);
                    if (actionMatch != null)
                    {
                        toReturn.Add(new KeyValuePair<string, string>(actionMatch.Text, url.Action(actionMatch.Action, actionMatch.Controller)));
                    }
                }
                return toReturn;
            }
        }

        public static List<KeyValuePair<string, string>> AutoShortBreadcrumb
        {
            get
            {
                var url = new UrlHelper(HttpContext.Current.Request.RequestContext);
                List<KeyValuePair<string, string>> toReturn = new List<KeyValuePair<string, string>>();

                toReturn.Add(new KeyValuePair<string, string>("Home", url.Action("Index", "Home")));

                if (CurrentController == "home")
                    return toReturn;

                MenuItem controllerMatch = Items.FirstOrDefault(m => m.Controller.ToLower() == CurrentController);
                if (controllerMatch != null)
                    toReturn.Add(new KeyValuePair<string, string>(controllerMatch.Text, url.Action(controllerMatch.Action, controllerMatch.Controller)));
                return toReturn;
            }
        }
    }

    public class MenuItem
    {
		#region Properties

		public MenuItem parent { get; set; }
        public string Icon { get; set; }
        public string Text { get; set; }

		private string controller;
        public string Controller {
			get
			{
				if (!string.IsNullOrEmpty(controller))
					return controller;
				else if (parent != null)
					return parent.Controller;
				else
					return "Home";
			}
			set { controller = value; }
		}

        private string action;
        public string Action
        {
            get
            {
                if (!String.IsNullOrEmpty(action))
                    return action;
                return "Index";
            }
            set
            {
                action = value;
            }
        }

        private List<string> roles;
        public string Roles {
            get
            {
                return string.Join(",", roles);
            }
            set
            {
                roles = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

		private bool hasIcons;
		private List<MenuItem> subMenuItems;
        public List<MenuItem> SubMenuItems
		{
			get { return subMenuItems; }
			set
			{
				subMenuItems = value;
				subMenuItems.ForEach(i => i.parent = this);
				hasIcons = subMenuItems.Where(i => !string.IsNullOrEmpty(i.Icon)).Count() > 0;
			}
		}

		#endregion

		#region Derived Properties

		public string LiClass
		{
			get
			{
				string currentController = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
				return currentController == Controller ? "active" : "";
			}
		}

		public MvcHtmlString IconMarkup
		{
			get
			{
				if (parent != null && !parent.hasIcons)
					return new MvcHtmlString("");

				string toReturnClass = "";
				if (!String.IsNullOrEmpty(Icon))
					toReturnClass = "fa-" + Icon;

				if (parent != null)
					toReturnClass += " fa-fw";

				if (!String.IsNullOrEmpty(toReturnClass))
				{
					return new MvcHtmlString(string.Format("<i class='fa {0}'></i> ", toReturnClass));
				}
				return new MvcHtmlString("");
			}
		}

		#endregion

		#region Constructor

		public MenuItem()
        {
            this.roles = new List<string>();
            this.subMenuItems = new List<MenuItem>();
        }

		public MenuItem(MenuItem _parent) : this()
		{
			this.parent = _parent;
		}

		#endregion

		#region Object Methods

		public bool UserHasAccess(System.Security.Principal.IPrincipal user)
        {
            if (roles.Count == 0)
                return true;
            else
            {
                foreach (string role in roles)
                {
                    if (user.IsInRole(role))
                        return true;
                }
            }
            return false;
        }

		#endregion
	}
	
}