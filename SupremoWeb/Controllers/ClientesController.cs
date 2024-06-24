using Microsoft.AspNetCore.Mvc;
using SupremoWeb.Models;
using SupremoWeb.Repository;
using System.Text.Json;

namespace SupremoWeb.Controllers
{
    public class ClientesController : Controller
    {
        private ITelaClientesRepository _telaClientesRepository;
        private readonly IHttpClientFactory _clientFactory;
        public ClientesController(ITelaClientesRepository telaClientesRepository, IHttpClientFactory clientFactory)
        {
            _telaClientesRepository = telaClientesRepository;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        [Route("Clientes")]
        public async Task<IActionResult> Index()
        {
            Shared shared = new Shared();
            ViewBag.EstadosBrasileiros = shared.RetornaEstadosBrasileiro();
            ViewBag.Atuacao = shared.RetornaAtuacao();

            IEnumerable<NodeModel> clienteModels = await _telaClientesRepository.ListAllClientes();

            ViewBag.Clientes = clienteModels;

            return View();
        }

        [HttpGet]
        [Route("Clientes/IncluirCliente")]
        public async Task<IActionResult> IncluirCliente()           //Incluir Novo Cliente
        {
            Shared shared = new Shared();
            ViewBag.EstadosBrasileiros = shared.RetornaEstadosBrasileiro();

            return View();
        }

        [HttpGet]
        [Route("Clientes/IncluirCliente/{uid:int}")]                //Editar Cliente
        public async Task<IActionResult> IncluirCliente(int uid)
        {
            Shared shared = new Shared();
            ViewBag.EstadosBrasileiros = shared.RetornaEstadosBrasileiro();

            ClienteModel clienteModels = await _telaClientesRepository.ListCliente(uid);

            return View("IncluirCliente", clienteModels);
        }

        [HttpPost]
        [Route("Clientes/IncluirCliente/{uid:int}")]                //Editar Cliente
        public async Task<IActionResult> IncluirCliente(int uid, ClienteModel clienteModel)
        {
            Shared shared = new Shared();
            ViewBag.EstadosBrasileiros = shared.RetornaEstadosBrasileiro();

            ClienteModel clienteModels = await _telaClientesRepository.ListCliente(uid);

            return View("IncluirCliente", clienteModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Clientes/IncluirCliente")]                          //Grava Cliente na Base de Dados
        public async Task<IActionResult> IncluirCliente(ClienteModel cliente)
        {
            if (ModelState.IsValid)
            {
                if (cliente.uid == 0)
                {
                    MensagemModel mensagemModel = await _telaClientesRepository.AddCliente(cliente);
                    TempData["Message"] = mensagemModel.Message;
                    TempData["MessageHeading"] = mensagemModel.MessageHeading;

                    if (mensagemModel.IsSuccess)
                    {
                        return RedirectToAction("IncluirCliente");
                    }
                }
                else
                {
                    MensagemModel mensagemModel = await _telaClientesRepository.AddCliente(cliente);
                    TempData["Message"] = mensagemModel.Message;
                    TempData["MessageHeading"] = mensagemModel.MessageHeading;

                    if (mensagemModel.IsSuccess)
                    {
                        return RedirectToAction("IncluirCliente");
                    }
                }
            }

            Shared shared = new Shared();
            ViewBag.EstadosBrasileiros = shared.RetornaEstadosBrasileiro();

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirCliente(int id)
        {
            if (id > 0)
            {
                MensagemModel mensagemModel = await _telaClientesRepository.DeleteCliente(id);
                TempData["Message"] = mensagemModel.Message;
                TempData["MessageHeading"] = mensagemModel.MessageHeading;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> BuscarEnderecoPorCEP(string cep)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"https://viacep.com.br/ws/{cep}/json/");

            if (response.IsSuccessStatusCode)
            {
                var enderecoJson = await response.Content.ReadAsStringAsync();
                EnderecoModel? enderecoModel = JsonSerializer.Deserialize<EnderecoModel>(enderecoJson);


                EnderecoRetModel enderecoRetModel = new EnderecoRetModel
                {
                    street = enderecoModel?.logradouro,
                    neighborhood = enderecoModel?.bairro,
                    city = enderecoModel?.localidade,
                    state = enderecoModel?.uf
                };

                return Json(enderecoRetModel);
            }

            return Json(null);
        }
    }
}





