using Microsoft.AspNetCore.Mvc;

namespace SupremoWeb.Controllers
{
    public class ClientesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
