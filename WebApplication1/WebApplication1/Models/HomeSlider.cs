using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class HomeSlider : BaseEntity
    {
        [StringLength(255)]
        public string Image { get; set; }
        [StringLength(255)]
        public string Subtitle { get; set; }
        [StringLength(255)]
        public string Title { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
