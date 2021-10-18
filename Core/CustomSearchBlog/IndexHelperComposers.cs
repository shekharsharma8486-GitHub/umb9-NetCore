
using Examine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.Examine;

namespace Core.CustomSearchBlog
{
  public  class IndexHelperComposers : IUserComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            IServiceCollection services = builder.Services;
            services.AddExamineLuceneIndex<BlogIndex, ConfigurationEnabledDirectoryFactory>("blogPost");
        }
    }
}
