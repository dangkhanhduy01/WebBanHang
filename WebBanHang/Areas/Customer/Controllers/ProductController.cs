using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace WebBanHang.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int? categoryId)
        {
            // Danh sách danh mục để render menu
            ViewBag.CategoryList = _db.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            // Lấy danh sách sản phẩm
            var products = _db.Products.Include(p => p.Category).AsQueryable();
            string categoryName = "Tất cả sản phẩm";

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
                var category = _db.Categories.Find(categoryId.Value);
                if (category != null) categoryName = category.Name;
            }

            ViewBag.Title = categoryName;
            return View(products.ToList());
        }
    }
}
