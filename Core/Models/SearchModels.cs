using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class SearchModels
    {
        public string url { get; set; }
        public string BlogTitle { get; set; }
        public string query { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public List<SearchModels> searchDatalist { get; set; }
    }
}
