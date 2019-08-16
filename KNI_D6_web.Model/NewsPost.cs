using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.Model
{
    public class NewsPost
    {
        public int Id { get; set; }

        public DateTime PublicationDate { get; set; }

        public string Article { get; set; }

        public string Text { get; set; }

        public string AuthorId { get; set; }

        public User Author { get; set; }
    }
}
