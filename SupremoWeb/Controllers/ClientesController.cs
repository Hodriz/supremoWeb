using Microsoft.AspNetCore.Mvc;
using SupremoWeb.Models;
using SupremoWeb.Repository;

namespace SupremoWeb.Controllers
{
    public class ClientesController : Controller
    {
        private ITelaClientesRepository _telaClientesRepository;
        public ClientesController(ITelaClientesRepository telaClientesRepository)
        {
            _telaClientesRepository = telaClientesRepository;
        }

        public async Task <IActionResult> Index()
        {
            //Carrega list views
            Shared shared = new Shared();
            ViewBag.EstadosBrasileiros = shared.RetornaEstadosBrasileiro();
            ViewBag.Atuacao = shared.RetornaAtuacao();

            //Carrega tabela inicial de clientes
            IEnumerable<NodeModel> clienteModels = await _telaClientesRepository.ListaAllClientes();

            ViewBag.Clientes = clienteModels;

            return View();
        }

        public IActionResult Incluir()
        {
            return View();
        }

        // POST: Clientes/Incluir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Incluir(ClienteModel cliente)
        {
            if (ModelState.IsValid)
            {
                bool sucesso = await _telaClientesRepository.AddCliente(cliente);
                if (sucesso)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(cliente);
        }
    }

}

