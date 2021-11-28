using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace Core.RenderMVCControllerFolder
{
   public class searchController : RenderController
    {
        public searchController(ILogger<searchController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {

        }

        public override IActionResult Index()
        {
            SearchModels searchModels = new SearchModels();
            // you are in control here!

            // return a 'model' to the selected template/view for this page.
            return CurrentTemplate(searchModels);
        }
    }
}
