using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Blog:BaseEntity
    {
        [StringLength(255)]
        public string Image { get; set; }
        [StringLength(255), Required]
        public string Title { get; set; }
        [StringLength(255), Required]
        public string PublisherName { get; set; }
        [StringLength(10000), Required]
        public string Description { get; set; }
        [NotMapped]
        public IFormFile BlogImage { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
    }
}
