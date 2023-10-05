using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShorteningService.Resources
{
    public class UrlResource
    {
        public string OriginalUrl { get; set; }
        public string ShortenedUrl { get; set; }
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
