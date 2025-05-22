using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace WebBanHang.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hosting;
        private IWebHostEnvironment _host;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment host)
        {
            _db = db;
            _hosting = host;
        }
        //liet ke danh sach san pham
        //public IActionResult Index()
        //{
        //    //doc tat ca san pham tu csld
        //    var dsSanPham = _db.Products.Include(x => x.Category).ToList();
        //    //tra ve cai view Index.cshtml liet ke san pham
        //    return View(dsSanPham);
        //}

        public IActionResult Index(int page = 1) //Đây là action method được gọi khi người dùng truy cập trang sản phẩm.
        {
            int pageSize = 4; //Quy định số sản phẩm hiển thị trên mỗi trang. Ở đây là 4 sản phẩm/trang.
            var dsSanPham = _db.Products.Include(x => x.Category).ToList(); //Truy vấn toàn bộ bảng Products từ cơ sở dữ liệu (Entity Framework).
            int totalItems = dsSanPham.Count; //Đếm tổng số sản phẩm hiện có trong danh sách (totalItems).
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize); //Tính tổng số trang cần hiển thị.

            var items = dsSanPham //Tạo ra danh sách sản phẩm cho trang hiện tại:
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page; //Gán thông tin số trang hiện tại và tổng số trang vào ViewBag để truyền qua View hiển thị phân trang.
            ViewBag.TotalPages = totalPages;

            return View(items); //Trả về View, đồng thời truyền danh sách sản phẩm đã phân trang (items) xuống để hiển thị.

        }

        //tra ve giao dien them moi san pham
        [HttpGet]
        public IActionResult Add()
        {
            //doc tat ca san pham tu csld
            ViewBag.TL = _db.Categories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
            return View();
        }
        //xu ly them moi san pham
        [HttpPost]
        public IActionResult Add(Product p, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                //doc tat ca san pham tu csld
                if (ImageUrl != null)
                {
                    //code xu ly anh san pham
                    p.ImageUrl = SaveImage(ImageUrl);

                }
                _db.Products.Add(p);
                _db.SaveChanges();
                TempData["success"] = "Thêm sản phẩm thành công!!";
                //dieu huong ve cai action Index
                return RedirectToAction("Index");
                //return View();
            }
            ViewBag.TL = _db.Categories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
            return View();
        }
        //phuong thuc xu ly upload
        private string SaveImage(IFormFile image)
        {
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            //lay duong dan luu tru wwwroot tren server
            var path = Path.Combine(_hosting.WebRootPath, @"images/products");
            var saveFile = Path.Combine(path, filename);
            using (var filestream = new FileStream(saveFile, FileMode.Create))
            {
                image.CopyTo(filestream);
            }
            return @"images/products/" + filename;
        }

        //tra ve giao dien cap nhap
        //UPDATA_PRODUCT_tra ve giao dien cap nhap
        public IActionResult Update(int id)
        {
            //doc thong tin san pham can cap nhap trong csdl
            //var sp = _db.Products.Where(x => x.Id == id).FirstOrDefault();
            var sp = _db.Products.Find(id);
            //doc tat ca san pham tu csld
            ViewBag.TL = _db.Categories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
            return View(sp);
        }
        [HttpPost]
        public IActionResult Update(Product p, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                var oldProduct = _db.Products.Find(p.Id);
                //doc tat ca san pham tu csld
                if (ImageUrl != null)
                {
                    //code xu ly anh san pham
                    p.ImageUrl = SaveImage(ImageUrl);
                    //xoa anh cu
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
                //cap nhap san pham vao csdl
                oldProduct.Name = p.Name;
                oldProduct.Price = p.Price;
                oldProduct.Description = p.Description;
                oldProduct.Category = p.Category;
                oldProduct.ImageUrl = p.ImageUrl;
                _db.SaveChanges();
                TempData["success"] = "Fix sản phẩm thành công!!";
                //dieu huong ve cai action Index
                return RedirectToAction("Index");
                //return View();
            }
            ViewBag.TL = _db.Categories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
            return View();
        }
        public IActionResult Delete(int id)
        {
            var sp = _db.Products.Find(id);
            if (sp == null)
            {
                return NotFound();
            }
            return View(sp);
        }
        //delete
        public IActionResult DeleteConfirmed(int id)
        {
            //doc thong tin san pham can xoa
            var sp = _db.Products.Find(id);
            if (sp == null)
            {
                return NotFound();
            }
            //xoa khoi csdl
            _db.Products.Remove(sp);
            _db.SaveChanges();
            //xoa anh cu
            if (!string.IsNullOrEmpty(sp.ImageUrl))
            {
                var oldFilePath = Path.Combine(_hosting.WebRootPath, sp.ImageUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }            //thong bao thao tac co the thanh cong hoac that bai
            TempData["error"] = "Deleted";
            //dieu huong ve action Index
            return RedirectToAction("Index");

        }
    }
}
