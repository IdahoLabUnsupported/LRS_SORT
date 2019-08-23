using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sort.Business;

namespace Sort.Mvc.Models
{
    public class SearchModel
    {
        public List<SearchResult> Results { get; set; } = new List<SearchResult>();

        public string SearchData { get; set; }
        public string ReturnUrl { get; set; }
        public bool IncludeTitle { get; set; }
        public bool IncludeAbstract { get; set; }
        public bool IncludeReviewer { get; set; }

        public bool Search()
        {
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            string uri = string.Empty;

            if (!string.IsNullOrWhiteSpace(SearchData))
            {
                SortMainObject.SearchForUser(SearchData, IncludeTitle, IncludeAbstract, IncludeReviewer).ForEach(n => Results.Add(new SearchResult(n.SortMainId.Value, url.Action("Index", "Artifact", new { id = n.SortMainId }), n.DisplayTitle, n.Title, n.StatusDisplayName)));
            }

            return Results.Count > 0;
        }
    }

    public class SearchResult
    {
        public string Uri { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }

        public SearchResult()
        {

        }

        public SearchResult(int id, string uri, string name, string title, string status)
        {
            Id = id;
            Uri = uri;
            Name = name;
            Title = title;
            Status = status;
        }
    }
}