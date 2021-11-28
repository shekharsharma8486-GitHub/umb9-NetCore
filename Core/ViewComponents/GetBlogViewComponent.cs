using Core.Models;
using Core.ViewModels;
using Examine;
using Examine.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Extensions;

namespace Core.ViewComponents
{
    public class GetBlogViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IExamineManager _examineManager;
        private readonly ILogger<SearchBlogViewComponent> _logger;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IUmbracoDatabaseFactory _databaseFactory;
        private readonly ServiceContext _ServiceContext;
        private readonly AppCaches _AppCaches;
        private readonly IProfilingLogger _IProfilingLogger;
        private readonly IPublishedUrlProvider _publishedUrlProvider;

        public GetBlogViewComponent(IHttpContextAccessor httpContextAccessor,
            IUmbracoContextFactory umbracoContextFactory,
            IExamineManager examineManager,
            ILogger<SearchBlogViewComponent> logger,
            IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _umbracoContextFactory = umbracoContextFactory;
            _examineManager = examineManager;
            _logger = logger;
            _umbracoContextAccessor = umbracoContextAccessor;
            _databaseFactory = databaseFactory;
            _ServiceContext = services;
            _AppCaches = appCaches;
            _IProfilingLogger = profilingLogger;
            _publishedUrlProvider = publishedUrlProvider;
        }
        public IViewComponentResult Invoke(SearchModels searchModels)
        {
            //if (string.IsNullOrEmpty(Convert.ToString(searchModels)))
            //{
            //    return null;
            //}
            if(searchModels.query==null)
            {
                return View(searchModels);
            }
                
            string searchData = searchModels.query;
            try
            {
              //if (!_examineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex index))
                if (!_examineManager.TryGetIndex("blogPost", out IIndex index))

                    throw new InvalidOperationException($"No index found by name {"blogPost"}");
                var searcher = index.Searcher;

                var criteria = searcher.CreateQuery(IndexTypes.Content, BooleanOperation.And);
                var examineQuery = criteria.NodeTypeAlias("blogPost");
                if (!string.IsNullOrEmpty(searchData))
                {
                    if (searchData.Contains(" "))
                    {
                        string[] terms = searchData.Split(' ');
                    }
                    else
                    {
                        examineQuery.And().Field("pageTitle", searchData.MultipleCharacterWildcard());
                    }
                }
                int pageIndex = searchModels.CurrentPage - 1;
                int pageSize = searchModels.ItemsPerPage;
                ISearchResults searchResult = examineQuery.Execute();
                IEnumerable<ISearchResult> pagedResults = searchResult.Skip(pageIndex * pageSize).Take(searchModels.ItemsPerPage);
                int totalResults = Convert.ToInt32(searchResult.TotalItemCount);
                searchModels.TotalItems = totalResults;
                if (pageSize != 0)
                {
                    searchModels.TotalPages = (totalResults + searchModels.ItemsPerPage - 1) / searchModels.ItemsPerPage;
                }
                if (searchResult.Count() > 0)
                {
                    searchModels.searchDatalist = GetBlogList(searchResult);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Search Error: Exception: {0} | Message: {1} | Stack Trace: {2}", e.InnerException != null ? e.InnerException.ToString() : "", e.Message != null ? e.Message.ToString() : "", e.StackTrace);
            }
            return View(searchModels);
          
        }
        private List<SearchModels> GetBlogList(IEnumerable<ISearchResult> pagedResults)
        {
            List<SearchModels> blogDetailsPages = new List<SearchModels>();
            using (UmbracoContextReference umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (ISearchResult result in pagedResults)
                {
                    if (int.TryParse(result.Id, out int nodeId))
                    {
                        IPublishedCache contentHelper = umbracoContextReference.UmbracoContext.Content;

                        SearchModels blogDetailsinfo = new SearchModels();
                        if (nodeId > 0)
                        {
                            var deatils = contentHelper.GetById(nodeId);
                            blogDetailsinfo.BlogTitle = result.Values["pageTitle"];
                            blogDetailsinfo.url = deatils.Url();
                            blogDetailsPages.Add(blogDetailsinfo);
                        }

                    }
                }
            }
            return blogDetailsPages;
        }
    }
}
