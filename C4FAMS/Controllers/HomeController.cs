using Microsoft.AspNetCore.Mvc;

namespace C4FAMS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StatusCode(int code)
        {
            switch (code)
            {
                case 404:
                    return View("~/Views/Shared/NotFound.cshtml", "Trang không tồn tại.");
                case 403:
                    return View("~/Views/Shared/AccessDenied.cshtml", "Bạn không có quyền truy cập.");
                default:
                    return View("~/Views/Shared/Error.cshtml", $"Lỗi {code}: Đã xảy ra lỗi không xác định.");
            }
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml", "Đã xảy ra lỗi trong hệ thống.");
        }
    }
}