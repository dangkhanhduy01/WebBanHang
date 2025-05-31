using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebBanHang.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class EmployeeHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
