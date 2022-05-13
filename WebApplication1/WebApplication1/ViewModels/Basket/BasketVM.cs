using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.ViewModels.Basket
{
    public class BasketVM
    {
        public int Count { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public int ProductId { get; set; }
        public int Color { get; set; }
        public int Size { get; set; }
        public double ExTax { get; set; }
    }
}
