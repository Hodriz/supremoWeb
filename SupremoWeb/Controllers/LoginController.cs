using Microsoft.AspNetCore.Mvc;
using SupremoWeb.Models;
using SupremoWeb.Repository;

namespace SupremoWeb.Controllers
{
    public class LoginController : Controller
    {
        private IAutenticacaoRepository _autenticacao;
        public LoginController(IAutenticacaoRepository autenticacao) 
        {
            _autenticacao = autenticacao;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Autenticacao(LoginModel loginModel)
        {
            await _autenticacao.AutenticacaoLogin(loginModel);

            TempData["message"] = RetornoAutenticacaoModel.Message;
            TempData["MessageHeading"] = RetornoAutenticacaoModel.MessageHeading;

            if (RetornoAutenticacaoModel.IsSuccess )
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Index");
            }
        }
    }
}
