using Core.Models;
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
    public class SearchBlogViewComponent : ViewComponent
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IUmbracoDatabaseFactory _databaseFactory;
        private readonly ServiceContext _ServiceContext;
        private readonly AppCaches _AppCaches;
        private readonly IProfilingLogger _IProfilingLogger;
        private readonly IPublishedUrlProvider _publishedUrlProvider;

        public SearchBlogViewComponent(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
            _databaseFactory = databaseFactory;
            _ServiceContext = services;
            _AppCaches = appCaches;
            _IProfilingLogger = profilingLogger;
            _publishedUrlProvider = publishedUrlProvider;
        }
        public IViewComponentResult Invoke(SearchModels searchModels)
        {
          
            return View(searchModels);
          
        }
    }
}
