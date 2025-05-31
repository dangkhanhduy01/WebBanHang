using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHang.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string fullName { set; get; }
        public DateTime Birthday { set; get; }

    }
   
}
