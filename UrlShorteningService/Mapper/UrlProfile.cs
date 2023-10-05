using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlShorteningService.Entities;
using UrlShorteningService.Resources;

namespace UrlShorteningService.Mapper
{
    public class UrlProfile: Profile
    {
        public UrlProfile()
        {
            CreateMap<Url, UrlResource>()
               .ForMember(t => t.Id, o => o.MapFrom(t => t.Id))
               .ForMember(t => t.OriginalUrl, o => o.MapFrom(t => t.OriginalUrl))
               .ForMember(t => t.ShortenedUrl, o => o.MapFrom(t => t.ShortenedUrl))
               .ForMember(t => t.CreationDate, o => o.MapFrom(t => t.CreationDate))
               .ReverseMap();
        }
       
    }
}
