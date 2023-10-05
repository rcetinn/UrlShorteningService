using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShorteningService.Entities;

namespace UrlShorteningService.Entities
{
    public record Url : IEntity
    {
        public string OriginalUrl { get; init; }
        public string ShortenedUrl { get; init; }
    }
}
