using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShorteningService.Entities
{
    public record IEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
