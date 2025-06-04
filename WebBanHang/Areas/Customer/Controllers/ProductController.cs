using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        // 👉 Action hiển thị danh sách sản phẩm
        public IActionResult Index(int? categoryId)
        {
            ViewBag.CategoryList = _db.Categories
                .Include(c => c.Products)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name + " (" + c.Products.Count + ")"
                }).ToList();

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

        // 👉 ✅ THÊM ACTION NÀY DƯỚI ĐÂY:
        public IActionResult GetCategory()
        {
            var categories = _db.Categories
                .Include(c => c.Products)
                .ToList();

            var html = "<div class='list-group'>";
            foreach (var cat in categories)
            {
                html += $@"
                <a href='#' class='list-group-item category-link' data-id='{cat.Id}' data-name='{cat.Name}'>
                    {cat.Name} <span class='badge bg-dark rounded-pill'>{cat.Products.Count}</span>
                </a>";
            }
            html += "</div>";

            return Content(html, "text/html");
        }
        public IActionResult GetProductsByCategory(int catid)
        {
            var products = _db.Products
                .Where(p => p.CategoryId == catid)
                .ToList();

            return PartialView("_ProductPartial", products);
        }

    }
}
