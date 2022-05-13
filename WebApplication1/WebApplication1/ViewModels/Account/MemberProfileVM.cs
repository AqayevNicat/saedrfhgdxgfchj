using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.ViewModels.Account
{
    public class MemberProfileVM
    {
        public MemberUpdateVM Member { get; set; }
        public List<WebApplication1.Models.Order> Orders { get; set; }
    }
}
