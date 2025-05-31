using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace WebBanHang.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hosting;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment hosting)
        {
            _db = db;
            _hosting = hosting;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 4;
            var dsSanPham = _db.Products.Include(x => x.Category).ToList();
            int totalItems = dsSanPham.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var items = dsSanPham
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(items);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.TL = _db.Categories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
            return View();
        }

        [HttpPost]
        public IActionResult Add(Product p, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrl != null)
                {
                    p.ImageUrl = SaveImage(ImageUrl);
                }
                _db.Products.Add(p);
                _db.SaveChanges();
                TempData["success"] = "Thêm sản phẩm thành công!!";
                return RedirectToAction("Index");
            }
            ViewBag.TL = _db.Categories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
            return View();
        }

        private string SaveImage(IFormFile image)
        {
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var path = Path.Combine(_hosting.WebRootPath, @"images/products");
            var saveFile = Path.Combine(path, filename);
            using (var filestream = new FileStream(saveFile, FileMode.Create))
            {
                image.CopyTo(filestream);
            }
            return @"images/products/" + filename;
        }

        public IActionResult Update(int id)
        {
            var sp = _db.Products.Find(id);
            ViewBag.TL = _db.Categories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
            return View(sp);
        }

        [HttpPost]
        public IActionResult Update(Product p, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                var oldProduct = _db.Products.Find(p.Id);
                if (ImageUrl != null)
                {
                    p.ImageUrl = SaveImage(ImageUrl);
                    if (!string.IsNullOrEmpty(oldProduct.ImageUrl))
                    {
                        var oldFilePath = Path.Combine(_hosting.WebRootPath, oldProduct.ImageUrl);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                }
                else
                {
                    p.ImageUrl = oldProduct.ImageUrl;
                }

                oldProduct.Name = p.Name;
                oldProduct.Price = p.Price;
                oldProduct.Description = p.Description;
                oldProduct.Category = p.Category;
                oldProduct.ImageUrl = p.ImageUrl;
                _db.SaveChanges();
                TempData["success"] = "Fix sản phẩm thành công!!";
                return RedirectToAction("Index");
            }
            ViewBag.TL = _db.Categories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
            return View();
        }

        public IActionResult Delete(int id)
        {
            var sp = _db.Products.Find(id);
            if (sp == null) return NotFound();
            return View(sp);
        }

        public IActionResult DeleteConfirmed(int id)
        {
            var sp = _db.Products.Find(id);
            if (sp == null) return NotFound();

            _db.Products.Remove(sp);
            _db.SaveChanges();

            if (!string.IsNullOrEmpty(sp.ImageUrl))
            {
                var oldFilePath = Path.Combine(_hosting.WebRootPath, sp.ImageUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            TempData["error"] = "Deleted";
            return RedirectToAction("Index");
        }

      
        [AllowAnonymous]
        public IActionResult GetCategory()
        {
            var categories = _db.Categories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    Count = _db.Products.Count(p => p.CategoryId == c.Id)
                })
                .ToList();

            return PartialView("_CategoryPartial", categories);
        }

      
        [AllowAnonymous]
        public IActionResult GetProductsByCategory(int categoryId)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == categoryId);
            ViewBag.CategoryName = category?.Name ?? "Không rõ";

            var products = _db.Products
                .Where(p => p.CategoryId == categoryId)
                .ToList();

            return PartialView("_ProductPartial", products);
        }
    }
}
