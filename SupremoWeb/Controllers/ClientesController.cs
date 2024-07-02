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
        public async Task<IActionResult> Index()
        {
            CamposGerais camposGerais = new CamposGerais();
            IEnumerable<NodeModel> clienteModels = await _telaClientesRepository.ListAllClientes();

            ViewBag.Clientes = clienteModels;
            ViewBag.EstadosBrasileiros = camposGerais.RetornaEstadosBrasileiro();
            ViewBag.Atuacao = camposGerais.RetornaAtuacao();

            return View();
        }

        [HttpGet]
        [Route("Clientes/IncluirCliente")]
        public async Task<IActionResult> IncluirCliente()                   //Carregar Tela IncluirCliente Sem Cliente
        {
            CamposGerais camposGerais = new CamposGerais();
            ViewBag.EstadosBrasileiros = camposGerais.RetornaEstadosBrasileiro();
            ViewBag.TipoPessoa = camposGerais.RetornaTipoPessoa();

            return View();
        }

        [HttpGet]
        [Route("Clientes/AlterarCliente/{uid:int}")]                        //Carregar Tela IncluirCliente Com Cliente
        public async Task<IActionResult> IncluirCliente(int uid)
        {
            CamposGerais camposGerais = new CamposGerais();

            ViewBag.EstadosBrasileiros = camposGerais.RetornaEstadosBrasileiro();
            ViewBag.TipoPessoa = camposGerais.RetornaTipoPessoa();
            ClienteTotalModel clienteTotalModel = await _telaClientesRepository.ListCliente(uid);

            return View("IncluirCliente", clienteTotalModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Clientes/IncluirCliente")]                                  
        public async Task<IActionResult> IncluirCliente(ClienteTotalModel clienteTotalModel)        //Incluir Cliente Novo
        {
            if (ModelState.IsValid)
            {
                //ClienteAddModel clienteAddModel = await RetornaClienteAdd(cliente);

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

            return View(clienteTotalModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Clientes/AlterarCliente/{uid:int}")] 
        public async Task<IActionResult> AlterarCliente(int uid, ClienteTotalModel clienteTotalModel)       // Alterar Cliente
        {
            CamposGerais camposGerais = new CamposGerais();
            ViewBag.EstadosBrasileiros = camposGerais.RetornaEstadosBrasileiro();
            ViewBag.TipoPessoa = camposGerais.RetornaTipoPessoa();

            if (ModelState.IsValid)
            {
                //ClienteModel clienteModel = await RetornaClienteUpdate(clienteTotalModel);

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


        //private async Task<ClienteAddModel> RetornaClienteAdd(ClienteTotalModel clienteTotalModel)
        //{
        //    ClienteAddModel clienteAddModel = new ClienteAddModel();

        //    clienteAddModel.lobId = clienteTotalModel.lobId;
        //    clienteAddModel.companyId = clienteTotalModel.companyId;
        //    clienteAddModel.personType = clienteTotalModel.personType;
        //    clienteAddModel.companyName = clienteTotalModel.companyName;
        //    clienteAddModel.tradingName = clienteTotalModel.tradingName;
        //    clienteAddModel.street = clienteTotalModel.street;
        //    clienteAddModel.complement = clienteTotalModel.complement;
        //    clienteAddModel.state = clienteTotalModel.state;
        //    clienteAddModel.city = clienteTotalModel.city;
        //    clienteAddModel.postalCode = clienteTotalModel.postalCode;
        //    clienteAddModel.neighborhood = clienteTotalModel.neighborhood;
        //    clienteAddModel.houseNumber = clienteTotalModel.houseNumber;
        //    clienteAddModel.identificationCard = clienteTotalModel.identificationCard;
        //    clienteAddModel.phone = clienteTotalModel.phone;
        //    clienteAddModel.cellphone = clienteTotalModel.cellphone;
        //    clienteAddModel.email = clienteTotalModel.email;
        //    clienteAddModel.website = clienteTotalModel.website;

        //    if (clienteTotalModel.personType == "LEGAL_ENTITY")
        //    {
        //        clienteAddModel.taxPayerId = clienteTotalModel.cnpj;
        //    }
        //    else
        //    {
        //        clienteAddModel.taxPayerId = clienteTotalModel.cpf;
        //    }


        //    return clienteAddModel;
        //}

        //private async Task<ClienteModel> RetornaClienteUpdate(ClienteTotalModel clienteTotalModel)
        //{
        //    ClienteModel clienteModel = new ClienteModel();

        //    clienteModel.lobId = clienteTotalModel.lobId;
        //    clienteModel.companyId = clienteTotalModel.companyId;
        //    clienteModel.companyName = clienteTotalModel.companyName;
        //    clienteModel.tradingName = clienteTotalModel.tradingName;
        //    clienteModel.street = clienteTotalModel.street;
        //    clienteModel.complement = clienteTotalModel.complement;
        //    clienteModel.state = clienteTotalModel.state;
        //    clienteModel.city = clienteTotalModel.city;
        //    clienteModel.postalCode = clienteTotalModel.postalCode;
        //    clienteModel.neighborhood = clienteTotalModel.neighborhood;
        //    clienteModel.houseNumber = clienteTotalModel.houseNumber;
        //    clienteModel.identificationCard = clienteTotalModel.identificationCard;
        //    clienteModel.phone = clienteTotalModel.phone;
        //    clienteModel.cellphone = clienteTotalModel.cellphone;
        //    clienteModel.email = clienteTotalModel.email;
        //    clienteModel.website = clienteTotalModel.website;

        //    clienteModel.uid = clienteTotalModel.uid;

        //    if (clienteTotalModel.personType == "LEGAL_ENTITY")
        //    {
        //        clienteModel.taxPayerId = clienteTotalModel.cnpj;
        //    }
        //    else
        //    {
        //        clienteModel.taxPayerId = clienteTotalModel.cpf;
        //    }

        //    return clienteModel;
        //}

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





