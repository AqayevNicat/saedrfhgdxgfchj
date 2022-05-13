using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.ViewModels.Basket;

namespace WebApplication1.ViewModels.Home
{
    public class HomeVM
    {
        public IEnumerable<HomeSlider> HomeSliders { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<WebApplication1.Models.Blog> Blogs { get; set; }
    }
}
