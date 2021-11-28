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
using Core.Models;
using Examine;
using static Umbraco.Cms.Core.Constants;
using Examine.Search;
using Umbraco.Cms.Infrastructure.Examine;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;



namespace Core.Controllers
{
   public class SearchController :SurfaceController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IExamineManager _examineManager;
        private readonly ILogger<SearchController> _logger;

        public SearchController(IUmbracoContextFactory umbracoContextFactory, 
            IExamineManager examineManager,
            ILogger<SearchController> logger,
            IHttpContextAccessor httpContextAccessor,
            IUmbracoContextAccessor umbracoContextAccessor, 
            IUmbracoDatabaseFactory databaseFactory, 
            ServiceContext services, 
            AppCaches appCaches, 
            IProfilingLogger profilingLogger, 
            IPublishedUrlProvider publishedUrlProvider) 
            : base(umbracoContextAccessor, 
                  databaseFactory, 
                  services, 
                  appCaches, 
                  profilingLogger, 
                  publishedUrlProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _umbracoContextFactory = umbracoContextFactory;
            _examineManager = examineManager;
            _logger = logger;
        }
        [HttpGet]
        public virtual IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public virtual IActionResult Index(SearchModels searchModels)
        //public IActionResult Index(SearchModels searchModels)
        //public SearchModels Index(SearchModels searchModels)
        {

            var Data = _httpContextAccessor.HttpContext.Request.Query;
            string b = Data["q"];
            //string a = _httpContextAccessor.HttpContext.Request.QueryString["q"];
            string searchData = searchModels.query;
            try
            {
                //if (!_examineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex index))
                if (!_examineManager.TryGetIndex("blogPost", out IIndex index))

                 throw new InvalidOperationException($"No index found by name {"blogPost"}");
            
                var searcher = index.Searcher;

                var criteria = searcher.CreateQuery(IndexTypes.Content, BooleanOperation.And);

                //  var filter = criteria.Field("__IndexType", "content");

                var examineQuery = criteria.NodeTypeAlias("blogPost");
                //var examineQuery = criteria.NodeTypeAlias("content");

                //  IBooleanOperation query; 
                //query = criteria.GroupedNot(new string[] { "umbracoNaviHide" }, "1");
               
                if (!string.IsNullOrEmpty(searchData))
                {
                   
                    if (searchData.Contains(" "))
                    {
                        string[] terms = searchData.Split(' ');
                        //criteria.GroupedOr(new[] { "pageTitle" }, terms);
                        //criteria.OrderByDescending(new SortableField[] { new SortableField("sortableDate") });
                        examineQuery.And().GroupedOr(new[] { "pageTitle" }, terms);
                        examineQuery.And().Field("pageTitle", searchData);
                        //examineQuery.OrderByDescending(new SortableField[] { new SortableField("sortableDate") });
                       // filter.And().GroupedOr(new[] { "pageTitle" }, terms);
                       //filter.OrderByDescending(new SortableField[] { new SortableField("sortableDate") });
                    }
                    else
                    {
                        //filter.And().Field("pageTitle", searchData.MultipleCharacterWildcard());
                        //filter.OrderByDescending(new SortableField[] { new SortableField("sortableDate") });
                        examineQuery.And().Field("pageTitle", searchData.MultipleCharacterWildcard());
                        examineQuery.OrderByDescending(new SortableField[] { new SortableField("sortableDate") });
                        //criteria.GroupedOr(new[] { "pageTitle" }, searchData.MultipleCharacterWildcard());
                    }
                }
              
                int pageIndex = searchModels.CurrentPage - 1;
                int pageSize = searchModels.ItemsPerPage;

                //ISearchResults searchResult = searcher.CreateQuery("content").Field("pageTitle", searchData.MultipleCharacterWildcard()).Execute();
                var results = examineQuery.Execute();
                ISearchResults searchResult = examineQuery.Execute();
                // ISearchResults searchResult = filter.Execute();
                IEnumerable<ISearchResult> pagedResults = searchResult.Skip(pageIndex * pageSize).Take(searchModels.ItemsPerPage);
               
                int totalResults = Convert.ToInt32(searchResult.TotalItemCount);
                searchModels.TotalItems = totalResults;
                if(pageSize!=0)
                {
                    searchModels.TotalPages = (totalResults + searchModels.ItemsPerPage - 1) /searchModels.ItemsPerPage;
                }
                if(searchResult.Count()>0)
                {
                    searchModels.searchDatalist = GetBlogList(searchResult);
                }
               
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Search Error: Exception: {0} | Message: {1} | Stack Trace: {2}", e.InnerException != null ? e.InnerException.ToString() : "", e.Message != null ? e.Message.ToString() : "", e.StackTrace);
            }
            //var childPage = CurrentPage.FirstChild();
            //return RedirectToUmbracoPage(childPage);
            // return CurrentUmbracoPage();
            //var ss = searchModels.searchDatalist;

            //return searchModels;
            //return View("Search", searchModels);
            //return PartialView("Search/_SearchBlog", searchModels);
            return PartialView("~/Views/Partials/Search/_SearchBlog.cshtml", searchModels);
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
                        if (nodeId >0)
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
