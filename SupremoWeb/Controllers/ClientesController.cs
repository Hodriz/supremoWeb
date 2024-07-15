using Microsoft.AspNetCore.Mvc;
using SupremoWeb.Models;
using SupremoWeb.Repository;
using SupremoWeb.Shared;
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
        public async Task<IActionResult> Index()                    //Carrega página Cliente Index
        {
            CamposGerais camposGerais = new CamposGerais();
            IEnumerable<NodeModel> clienteModels = await _telaClientesRepository.ListAllClientes();

            ViewBag.Clientes = clienteModels;
            ViewBag.EstadosBrasileiros = camposGerais.RetornaEstadosBrasileiro();
            ViewBag.Atuacao = camposGerais.RetornaAtuacao();

            return View();
        }

        [HttpPost]
        [Route("FiltroCliente")]
        public async Task<IActionResult> ListFiltroClientes(ClienteFiltroModel clienteFiltroModel)      //Filtro de Pesquisa
        {
            CamposGerais camposGerais = new CamposGerais();

            //if (clienteFiltroModel.companyName != null || clienteFiltroModel.tradingName != null)
            //{
                IEnumerable<NodeModel> clienteModels = await _telaClientesRepository.ListFiltroClientes(clienteFiltroModel);

                ViewBag.Clientes = clienteModels;
                ViewBag.EstadosBrasileiros = camposGerais.RetornaEstadosBrasileiro();
                ViewBag.Atuacao = camposGerais.RetornaAtuacao();
            //}
            //else
            //{
            //    TempData["Message"] = "Favor preencher um dos campos de pesquisa !!!";
            //    TempData["MessageHeading"] = "AVISO";

            //    IEnumerable<NodeModel> clienteModels = await _telaClientesRepository.ListAllClientes();
            //    ViewBag.Clientes = clienteModels;
            //    ViewBag.EstadosBrasileiros = camposGerais.RetornaEstadosBrasileiro();
            //    ViewBag.Atuacao = camposGerais.RetornaAtuacao();
            //}
            return View("Index");
        }

        [HttpGet]
        [Route("Clientes/IncluirCliente")]
        public async Task<IActionResult> IncluirCliente()                   //Carrega Tela IncluirCliente Sem Cliente
        {
            CamposGerais camposGerais = new CamposGerais();
            ViewBag.EstadosBrasileiros = camposGerais.RetornaEstadosBrasileiro();
            ViewBag.TipoPessoa = camposGerais.RetornaTipoPessoa();
            ViewBag.PersonStatus = camposGerais.RetornaPersonStatus();

            return View();
        }

        [HttpGet]
        [Route("Clientes/AlterarCliente/{uid:int}")]                        //Carrega Tela IncluirCliente Com Cliente
        public async Task<IActionResult> IncluirCliente(int uid)
        {
            CamposGerais camposGerais = new CamposGerais();

            ViewBag.EstadosBrasileiros = camposGerais.RetornaEstadosBrasileiro();
            ViewBag.TipoPessoa = camposGerais.RetornaTipoPessoa();
            ViewBag.PersonStatus = camposGerais.RetornaPersonStatus();

            ClienteTotalModel clienteTotalModel = await _telaClientesRepository.ListCliente(uid);

            return View("IncluirCliente", clienteTotalModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Clientes/IncluirCliente")]
        public async Task<IActionResult> IncluirCliente(ClienteTotalModel clienteTotalModel)        //Inclui Cliente Novo
        {
            if (ModelState.IsValid)
            {
                clienteTotalModel.companyId = 1;    //Implementado posteriormente

                MensagemModel mensagemModel = await _telaClientesRepository.AddCliente(clienteTotalModel);
                TempData["Message"] = mensagemModel.Message;
                TempData["MessageHeading"] = mensagemModel.MessageHeading;

                if (mensagemModel.IsSuccess)
                {
                    return RedirectToAction("IncluirCliente");
                }
            }

            CamposGerais camposGerais = new CamposGerais();
            ViewBag.EstadosBrasileiros = camposGerais.RetornaEstadosBrasileiro();
            ViewBag.TipoPessoa = camposGerais.RetornaTipoPessoa();
            ViewBag.PersonStatus = camposGerais.RetornaPersonStatus();

            return View(clienteTotalModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Clientes/AlterarCliente/{uid:int}")]
        public async Task<IActionResult> AlterarCliente(int uid, ClienteTotalModel clienteTotalModel)       // Altera Cliente
        {
            CamposGerais camposGerais = new CamposGerais();
            ViewBag.EstadosBrasileiros = camposGerais.RetornaEstadosBrasileiro();
            ViewBag.TipoPessoa = camposGerais.RetornaTipoPessoa();
            ViewBag.PersonStatus = camposGerais.RetornaPersonStatus();

            if (ModelState.IsValid)
            {
                MensagemModel mensagemModel = await _telaClientesRepository.UpdateCliente(clienteTotalModel);
                TempData["Message"] = mensagemModel.Message;
                TempData["MessageHeading"] = mensagemModel.MessageHeading;

                if (mensagemModel.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
            }

            return View("IncluirCliente", clienteTotalModel);
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





