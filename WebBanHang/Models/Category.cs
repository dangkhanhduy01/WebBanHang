using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WebBanHang.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
