
using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Examine.Lucene;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;

namespace Core.CustomSearchBlog
{
  public  class BlogIndex : UmbracoExamineIndex, IUmbracoContentIndex, IDisposable
    {

        public BlogIndex(ILoggerFactory loggerFactory, string name, IOptionsSnapshot<LuceneDirectoryIndexOptions> indexOptions, IHostingEnvironment hostingEnvironment, IRuntimeState runtimeState)  : base(loggerFactory, name, indexOptions, hostingEnvironment, runtimeState)
        
        {
            loggerFactory.CreateLogger<BlogIndex>();
            LuceneDirectoryIndexOptions namedOptions = indexOptions.Get(name);
            if (namedOptions == null)
            {
                throw new InvalidOperationException($"No named {typeof(LuceneDirectoryIndexOptions)} options with name {name}");
            }
            if (namedOptions.Validator is IContentValueSetValidator contentValueSetValidator)
            {
                PublishedValuesOnly = contentValueSetValidator.PublishedValuesOnly;
            }
        }
       void IIndex.IndexItems(IEnumerable<ValueSet> values) => PerformIndexItems(values.Where(v => v.ItemType == "blogPost"), OnIndexOperationComplete);
    }
   
}
