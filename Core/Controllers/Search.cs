using Microsoft.AspNetCore.Mvc;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Core.Controllers
{
   public class SearchController :SurfaceController
    {
        public SearchController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {

        }
        [HttpPost]
        public IActionResult Index(string q)
        {
            //if (!context.RouteContext.HttpContext.Request.Query.ContainsKey(_parameter))
            //{
            //    return false;
            //}
          
            string searchData = q;
            //string searchData = HttpContext.Request.Query["q"];
            //var searchData = Request.QueryString["q"];
            //var pageid = Context.Request.Query["q"];

            // Gets the first child page of the current page
            var childPage = CurrentPage.FirstChild();
            //return RedirectToUmbracoPage(childPage);
            return CurrentUmbracoPage();
        }
    }
}
