using Inl.MvcHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Sort.EmployeeHelper
{
    public static class HtmlHelperExtensions
    {
        #region EmployeeFor

        public static MvcHtmlString EmployeeFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes) => EmployeeFor(htmlHelper, expression, new UrlHelper(htmlHelper.ViewContext.RequestContext).Action("SearchEmployee", "Home"), htmlAttributes);
        public static MvcHtmlString EmployeeFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string searchUrl, object htmlAttributes) => EmployeeFor(htmlHelper, expression, searchUrl, new RouteValueDictionary(htmlAttributes));
        public static MvcHtmlString EmployeeFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) => EmployeeFor(htmlHelper, expression, new UrlHelper(htmlHelper.ViewContext.RequestContext).Action("SearchEmployee", "Home"));
        public static MvcHtmlString EmployeeFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string searchUrl) => EmployeeFor(htmlHelper, expression, searchUrl, new Dictionary<string, object>());
        public static MvcHtmlString EmployeeFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes) => EmployeeFor(htmlHelper, expression, new UrlHelper(htmlHelper.ViewContext.RequestContext).Action("SearchEmployee", "Home"), htmlAttributes);
        public static MvcHtmlString EmployeeFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string searchUrl, IDictionary<string, object> htmlAttributes)
        {
            string buildstamp = Inl.MvcHelper.BsHelper.GetBuildStamp().ToString();
            // Register the dependency
            BsHelper.AddDependency(Dependency.TypeAhead);
            BsHelper.AddCssDependency($"    <link href='~/EmployeeHelper/Css/Employee.helper.css?s={buildstamp}' rel='stylesheet' />");
            BsHelper.AddJsDependency($"    <script src='~/EmployeeHelper/Script/employee.helper.js?s={buildstamp}'></script>");

            // make sure that htmlAttributes is not null, or we won't be able to add it
            if (htmlAttributes == null)
                htmlAttributes = new Dictionary<string, object>();

            // get the field name for use with bootstrap
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName;
            if (labelText == null)
            {
                labelText = (metadata.PropertyName ?? htmlFieldName.Split('.').Last()).SplitCamelCase();
            }

            // add a placeholder if it isn't already there
            if (!String.IsNullOrEmpty(labelText))
            {
                if (!htmlAttributes.ContainsKey("placeholder"))
                {
                    htmlAttributes["placeholder"] = labelText;
                }
            }

            // add form-control
            if (htmlAttributes.ContainsKey("class"))
            {
                htmlAttributes["class"] += " form-control";
            }
            else
            {
                htmlAttributes["class"] = "form-control";
            }

            string htmlId = GenerateIdFromName(htmlFieldName);
            string searchBox = htmlHelper.TextBoxFor(expression, htmlAttributes).ToString();
            searchBox = searchBox.Replace(htmlFieldName, htmlFieldName + "_search");
            searchBox = searchBox.Replace($"placeholder=\"{htmlFieldName}_search\"", $"placeholder=\"{htmlFieldName}\"");
            if (htmlFieldName != htmlId)
            {
                searchBox = searchBox.Replace(htmlId, htmlId + "_search");
            }
            string hiddenField = htmlHelper.HiddenFor(expression).ToString();

            string employees = "<span class=\"employee-badge\">Employee</span>";

            string extras = $@"<script>$(function() {{ employeeInit('{searchUrl}', '{htmlId}'); }})</script>";

            return new MvcHtmlString("<div class=\"employee-wrap\">" + searchBox + hiddenField + employees + extras + "</div>");
        }

        #endregion

        #region Employee


        public static MvcHtmlString Employee<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string value, object htmlAttributes) => Employee(htmlHelper, id, value, new UrlHelper(htmlHelper.ViewContext.RequestContext).Action("SearchEmployee", "Home"), htmlAttributes);
        public static MvcHtmlString Employee<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string value, string searchUrl, object htmlAttributes) => Employee(htmlHelper, id, value, searchUrl, new RouteValueDictionary(htmlAttributes));
        public static MvcHtmlString Employee<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string value) => Employee(htmlHelper, id, value, new UrlHelper(htmlHelper.ViewContext.RequestContext).Action("SearchEmployee", "Home"));
        public static MvcHtmlString Employee<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string value, string searchUrl) => Employee(htmlHelper, id, value, searchUrl, new Dictionary<string, object>());
        public static MvcHtmlString Employee<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string value, IDictionary<string, object> htmlAttributes) => Employee(htmlHelper, id, value, new UrlHelper(htmlHelper.ViewContext.RequestContext).Action("SearchEmployee", "Home"), htmlAttributes);
        public static MvcHtmlString Employee<TModel>(this HtmlHelper<TModel> htmlHelper, string id, string value, string searchUrl, IDictionary<string, object> htmlAttributes)
        {
            // Register the dependency
            BsHelper.AddDependency(Dependency.TypeAhead);
            BsHelper.AddCssDependency("    <link href='~/EmployeeHelper/Css/Employee.helper.css' rel='stylesheet' />");
            BsHelper.AddJsDependency("    <script src='~/EmployeeHelper/Script/employee.helper.js'></script>");

            // make sure that htmlAttributes is not null, or we won't be able to add it
            if (htmlAttributes == null)
                htmlAttributes = new Dictionary<string, object>();

            // get the field name for use with bootstrap
            string htmlFieldName = id;
            string labelText = id;
            if (labelText == null)
            {
                labelText = (id ?? htmlFieldName.Split('.').Last()).SplitCamelCase();
            }

            // add a placeholder if it isn't already there
            if (!String.IsNullOrEmpty(labelText))
            {
                if (!htmlAttributes.ContainsKey("placeholder"))
                {
                    htmlAttributes["placeholder"] = labelText;
                }
            }

            // add form-control
            if (htmlAttributes.ContainsKey("class"))
            {
                htmlAttributes["class"] += " form-control";
            }
            else
            {
                htmlAttributes["class"] = "form-control";
            }

            htmlAttributes["name"] = htmlAttributes["id"] = id;

            string htmlId = GenerateIdFromName(htmlFieldName);
            string searchBox = htmlHelper.TextBox(id, null, htmlAttributes).ToString();
            searchBox = searchBox.Replace(htmlFieldName, htmlFieldName + "_search");
            if (htmlFieldName != htmlId)
            {
                searchBox = searchBox.Replace(htmlId, htmlId + "_search");
            }

            string hiddenField = htmlHelper.Hidden(id, value).ToString();

            string employees = "<span class=\"employee-badge\">Employee</span>";

            string extras = $@"<script>$(function() {{ employeeInit('{searchUrl}', '{htmlId}'); }})</script>";

            return new MvcHtmlString("<div class=\"employee-wrap\">" + searchBox + hiddenField + employees + extras + "</div>");
        }

        #endregion

        #region EmployeesFor

        public static MvcHtmlString EmployeesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            return EmployeesFor(htmlHelper, expression, urlHelper.Action("SearchEmployee", "Home"), urlHelper.Action("GetEmployee", "Home"));
        }

        public static MvcHtmlString EmployeesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string searchUrl, string getUrl) => EmployeesFor(htmlHelper, expression, searchUrl, getUrl, new Dictionary<string, object>());

        public static MvcHtmlString EmployeesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            return EmployeesFor(htmlHelper, expression, urlHelper.Action("SearchEmployee", "Home"), urlHelper.Action("GetEmployee", "Home"), htmlAttributes);
        }

        public static MvcHtmlString EmployeesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string searchUrl, string getUrl, object htmlAttributes) => EmployeesFor(htmlHelper, expression, searchUrl, getUrl, new RouteValueDictionary(htmlAttributes));

        public static MvcHtmlString SNumEmployeesForbersFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            return EmployeesFor(htmlHelper, expression, urlHelper.Action("SearchEmployee", "Home"), urlHelper.Action("GetEmployee", "Home"), htmlAttributes);
        }

        public static MvcHtmlString EmployeesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string searchUrl, string getUrl, IDictionary<string, object> htmlAttributes)
        {
            string buildstamp = Inl.MvcHelper.BsHelper.GetBuildStamp().ToString();
            // Register the dependency
            BsHelper.AddDependency(Dependency.TypeAhead);
            BsHelper.AddCssDependency($"    <link href='~/EmployeeHelper/Css/Employees.helper.css?s={buildstamp}' rel='stylesheet' />");
            BsHelper.AddJsDependency($"    <script src='~/EmployeeHelper/Script/employees.helper.js?s={buildstamp}'></script>");

            // make sure that htmlAttributes is not null, or we won't be able to add it
            if (htmlAttributes == null)
                htmlAttributes = new Dictionary<string, object>();

            // get the field name for use with bootstrap
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName;
            if (labelText == null)
            {
                labelText = (metadata.PropertyName ?? htmlFieldName.Split('.').Last()).SplitCamelCase();
            }

            // add a placeholder if it isn't already there
            if (!String.IsNullOrEmpty(labelText))
            {
                if (!htmlAttributes.ContainsKey("placeholder"))
                {
                    htmlAttributes["placeholder"] = "Search Employees to Add...";
                }
            }

            // add form-control
            if (htmlAttributes.ContainsKey("class"))
            {
                htmlAttributes["class"] += " form-control";
            }
            else
            {
                htmlAttributes["class"] = "form-control";
            }

            // DS: Since we always need a List<string> to check contains, we need to convert whatever we have
            List<string> values = new List<string>();
            if (metadata.Model == null)
            {
                // do nothing
            }
            else if (metadata.ModelType == typeof(IEnumerable<SelectListItem>))
            {
                values = ((IEnumerable<SelectListItem>)metadata.Model).Select(x => x.Value).ToList();
            }
            else if (typeof(IEnumerable).IsAssignableFrom(metadata.ModelType))
            {
                IEnumerable enumerable = (IEnumerable)metadata.Model;
                foreach (object item in enumerable)
                {
                    values.Add(item.ToString());
                }
            }

            string searchBox = htmlHelper.TextBox(htmlFieldName + "_search", string.Join(",", values), htmlAttributes).ToString();
            string employeeBadge = "<span class=\"employee-badge\">Employees</span>";
            string extras = $@"<script>$(function() {{ employeesInit('{searchUrl}', '{getUrl}', '{GenerateIdFromName(htmlFieldName)}', '{htmlFieldName}'); }})</script>";

            return new MvcHtmlString("<div class='employee-wrap'>" + searchBox + employeeBadge + "</div>" + extras);
        }

        #endregion

        #region Helper Methods

        private static string GenerateIdFromName(string name)
        {
            string id = name;
            id = id.Replace('[', '_');
            id = id.Replace(']', '_');
            id = id.Replace('.', '_');
            return id;
        }

        #endregion

    }
}
