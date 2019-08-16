using KNI_D6_web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.ViewModels.News
{
    public class NewsViewModel
    {
        public IEnumerable<NewsPost> NewsPosts { get; set; }

        public bool IsAdmin { get; set; }
    }
}
