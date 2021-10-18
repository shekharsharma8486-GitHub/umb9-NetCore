using Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Extensions;

namespace Core.ViewComponents
{
    public class BlogTagsViewComponent : ViewComponent
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IUmbracoDatabaseFactory _databaseFactory;
        private readonly ServiceContext _ServiceContext;
        private readonly AppCaches _AppCaches;
        private readonly IProfilingLogger _IProfilingLogger;
        private readonly IPublishedUrlProvider _publishedUrlProvider;

        public BlogTagsViewComponent(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
            _databaseFactory = databaseFactory;
            _ServiceContext = services;
            _AppCaches = appCaches;
            _IProfilingLogger = profilingLogger;
            _publishedUrlProvider = publishedUrlProvider;
        }
        public IViewComponentResult Invoke(IPublishedContent currentPage)
        {
            var pages = currentPage.Parent;
            //var pages = currentPage.Parent.AncestorOrSelf<Umbraco.Cms.Web.Common.PublishedModels.Blogs>();
            // var homePage = currentPage.AncestorOrSelf<Umbraco.Cms.Web.Common.PublishedModels.Home>();
            var blogPage = currentPage.Root();
            List<string> blogTags = new List<string>();
            //List<BlogTags> blogTags = new List<BlogTags>();
            //List<BlogTags> finalBlogTags = new List<BlogTags>();
            foreach (BlogPost item in pages.Children)
            {
                var pid = item.Id;            
                if (item.BlogTags.Any())
                {
                    foreach (var tagvalue in item.BlogTags)
                    { 
                        //btlist.BlogTag = tagvalue;
                        blogTags.Add(tagvalue.ToString());
                    }
                }
            };
           var  finalBlogTags = blogTags.Distinct().OrderBy(x=>x).ToList();
            return View(finalBlogTags);
          
        }
    }
}
