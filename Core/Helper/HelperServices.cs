using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Core.Helper
{
   public static class HelperServices
    {
        public static string GetPartial(IPublishedElement content)
        {
           // return string.Format(Views.Partials.NestedContent, (object)content.ContentType.Alias);
             return string.Format("~/Views/Partials/NestedContent/{0}.cshtml", (object)content.ContentType.Alias);
        }
    }
}
