using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Product : BaseEntity
    {
        [StringLength(255),Required]
        public string Name { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public double ExTax { get; set; }
        public double DiscountPrice { get; set; }
        public double Count { get; set; }
        [Required]
        public string Description { get; set; }
        public bool Availability { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public IEnumerable<ProductColorSize> ProductColorSizes { get; set; }
        public IEnumerable<ProductTag> ProductTags { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        [NotMapped]
        public IFormFile[] ProductImageFiles { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        [NotMapped]
        public List<int> TagIds { get; set; } = new List<int>();
        [NotMapped]
        public List<int> ColorIds { get; set; } = new List<int>();
        [NotMapped]
        public List<int> SizeIds { get; set; } = new List<int>();
        [NotMapped]
        public List<int> Counts { get; set; } = new List<int>();
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
