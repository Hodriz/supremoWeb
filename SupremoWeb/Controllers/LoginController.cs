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
            RetornoAutenticacaoModel ret = await _autenticacao.AutenticacaoLogin(loginModel);
            return RedirectToAction("Index", "Home");
        }
    }
}
