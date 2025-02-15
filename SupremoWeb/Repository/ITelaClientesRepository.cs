﻿using Newtonsoft.Json;
using SupremoWeb.Models;
using SupremoWeb.Views.Shared;
using System.Text;

namespace SupremoWeb.Repository
{
    public interface ITelaClientesRepository
    {
        Task<IEnumerable<NodeModel>> ListAllClientes();
        Task<IEnumerable<NodeModel>> ListFiltroClientes(ClienteFiltroModel clienteFiltroModel);
        Task<ClienteTotalModel> ListCliente(int uid);
        Task<MensagemModel> AddCliente(ClienteTotalModel clienteTotalModel);
        Task<MensagemModel> UpdateCliente(ClienteTotalModel clienteTotalModel);
        Task<MensagemModel> DeleteCliente(int uid);
    }

    public class TelaClientesRepository : ITelaClientesRepository
    {
        private readonly ILoggerRepository _loggerRepository;
        private readonly IConfiguration _configuration;
        public TelaClientesRepository(IConfiguration configuration, ILoggerRepository loggerRepository)
        {
            _configuration = configuration;
            _loggerRepository = loggerRepository;
        }

        public async Task<IEnumerable<NodeModel>> ListAllClientes()
        {
            try
            {
                StringContent stringContent = new StringContent("{\"query\":\"query Node($pageSize: Int, $filter: CustomerFilter) {\\r\\n  customers(pageSize: $pageSize, filter: $filter) {\\r\\n    edges {\\r\\n      node {\\r\\n        uid\\r\\n        companyId\\r\\n        id\\r\\n        companyName\\r\\n        tradingName\\r\\n        taxPayerId\\r\\n        identificationCard\\r\\n      }\\r\\n    }\\r\\n  }\\r\\n}\",\"variables\":{\"pageSize\":50,\"filter\":{\"companyName\":{\"_ne\":\"\"}}}}", null, "application/json");
                var responseBody = await ShareFunctions.RecebeResponseBody(stringContent);
                RootObjectModel rootObject = JsonConvert.DeserializeObject<RootObjectModel>(responseBody);
                IEnumerable<NodeModel> nodes = rootObject.data.Customers.edges.Select(e => e.node).ToArray();

                return nodes;
            }
            catch (Exception ex)
            {
                await _loggerRepository.WriteLog("TelaClientesRepository", "ListaAllClientes", ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<NodeModel>> ListFiltroClientes(ClienteFiltroModel clienteFiltroModel)
        {
            try
            {
                var filters = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(clienteFiltroModel.cpfCnpj))
                {
                    clienteFiltroModel.cpfCnpj = clienteFiltroModel.cpfCnpj?.Replace(".", "") ?? clienteFiltroModel.cpfCnpj;
                    clienteFiltroModel.cpfCnpj = clienteFiltroModel.cpfCnpj?.Replace("/", "") ?? clienteFiltroModel.cpfCnpj;
                    clienteFiltroModel.cpfCnpj = clienteFiltroModel.cpfCnpj?.Replace("-", "") ?? clienteFiltroModel.cpfCnpj;

                    filters.Add("taxPayerId", new { _iLike = $"%{clienteFiltroModel.cpfCnpj}%" });
                }

                if (!string.IsNullOrEmpty(clienteFiltroModel.rgInscricao))
                {
                    clienteFiltroModel.rgInscricao = clienteFiltroModel.rgInscricao?.Replace(".", "") ?? clienteFiltroModel.rgInscricao;
                    clienteFiltroModel.rgInscricao = clienteFiltroModel.rgInscricao?.Replace("/", "") ?? clienteFiltroModel.rgInscricao;
                    clienteFiltroModel.rgInscricao = clienteFiltroModel.rgInscricao?.Replace("-", "") ?? clienteFiltroModel.rgInscricao;

                    filters.Add("identificationCard", new { _iLike = $"%{clienteFiltroModel.rgInscricao}%" });
                }

                if (clienteFiltroModel.codigo != null)
                {
                    filters.Add("id", new { _eq = clienteFiltroModel.codigo });
                }

                if (clienteFiltroModel.empresa != null)
                {
                    filters.Add("companyId", new { _eq = clienteFiltroModel.empresa });
                }
                else
                {
                    filters.Add("companyId", new { _gt = 0 });
                }

                if (!string.IsNullOrEmpty(clienteFiltroModel.companyName))
                {
                    filters.Add("companyName", new { _iLike = $"%{clienteFiltroModel.companyName}%" });
                }

                if (!string.IsNullOrEmpty(clienteFiltroModel.tradingName))
                {
                    filters.Add("tradingName", new { _iLike = $"%{clienteFiltroModel.tradingName}%" });
                }

                if (!string.IsNullOrEmpty(clienteFiltroModel.endereco))
                {
                    filters.Add("street", new { _iLike = $"%{clienteFiltroModel.endereco}%" });
                }

                if (!string.IsNullOrEmpty(clienteFiltroModel.bairro))
                {
                    filters.Add("neighborhood", new { _iLike = $"%{clienteFiltroModel.bairro}%" });
                }

                if (!string.IsNullOrEmpty(clienteFiltroModel.cidade))
                {
                    filters.Add("city", new { _iLike = $"%{clienteFiltroModel.cidade}%" });
                }

                if (!string.IsNullOrEmpty(clienteFiltroModel.uf))
                {
                    filters.Add("state", new { _iLike = $"%{clienteFiltroModel.uf}%" });
                }

                if (!string.IsNullOrEmpty(clienteFiltroModel.telefone))
                {
                    filters.Add("phone", new { _iLike = $"%{clienteFiltroModel.telefone}%" });
                }

                var variables = new { filter = filters };
                var query = new
                {
                    query = "query Customers($filter: CustomerFilter) { customers(filter: $filter) { edges { node { uid companyId id companyName tradingName taxPayerId identificationCard phone cellphone email neighborhood street houseNumber complement city state postalCode website lobId personType } } } }",
                    variables = variables
                };

                var jsonContent = JsonConvert.SerializeObject(query);
                StringContent stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                await _loggerRepository.WriteLog("TelaClientesRepository", "ListFiltroClientes", $"Request JSON: {jsonContent}");

                var responseBody = await ShareFunctions.RecebeResponseBody(stringContent);
                RootObjectModel rootObject = JsonConvert.DeserializeObject<RootObjectModel>(responseBody);
                IEnumerable<NodeModel> nodes = rootObject.data.Customers.edges.Select(e => e.node).ToArray();

                return nodes;
            }
            catch (Exception ex)
            {
                await _loggerRepository.WriteLog("TelaClientesRepository", "ListaAllClientes", ex.Message);
                return null;
            }
        }

        public async Task<ClienteTotalModel> ListCliente(int uid)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _configuration["Parametros:url"]);
                request.Headers.Add("Authorization", "Bearer " + RetornoAutenticacaoModel.token);

                var content = new StringContent(
                    "{\"query\":\"query Node($filter: CustomerFilter) {\\r\\n  customers(filter: $filter) {\\r\\n    edges {\\r\\n      node {\\r\\n        uid\\r\\n        companyId\\r\\n        id\\r\\n        personType\\r\\n        personStatus\\r\\n        companyName\\r\\n        tradingName\\r\\n        taxPayerId\\r\\n        identificationCard\\r\\n        phone\\r\\n        cellphone\\r\\n        email\\r\\n        neighborhood\\r\\n        street\\r\\n        houseNumber\\r\\n        complement\\r\\n        city\\r\\n        state\\r\\n        postalCode\\r\\n        website\\r\\n        lobId\\r\\n        addresses {\\r\\n          type\\r\\n          street\\r\\n          houseNumber\\r\\n          complement\\r\\n          neighborhood\\r\\n          city\\r\\n          state\\r\\n          postalCode\\r\\n        }\\r\\n      }\\r\\n    }\\r\\n  }\\r\\n}\",\"variables\":{\"filter\":{\"uid\":{\"_eq\":" + uid + "},\"companyId\":{\"_gt\":0},\"id\":{\"_gt\":0}}}}",
                    Encoding.UTF8,
                    "application/json"
                );

                request.Content = content;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                RootObjectModel rootObject = JsonConvert.DeserializeObject<RootObjectModel>(responseBody);
                IEnumerable<NodeModel> nodes = rootObject.data.Customers.edges.Select(e => e.node).ToArray();

                NodeModel nodeModel = nodes.FirstOrDefault();
                ClienteTotalModel clienteTotalModel = new ClienteTotalModel();

                if (nodeModel != null)
                {
                    clienteTotalModel.uid = Convert.ToInt32(nodeModel.uid);
                    clienteTotalModel.lobId = Convert.ToInt32(nodeModel.lobId);
                    clienteTotalModel.personType = nodeModel.personType;
                    clienteTotalModel.companyId = Convert.ToInt32(nodeModel.companyId);
                    clienteTotalModel.postalCode = nodeModel.postalCode?.Trim() ?? nodeModel.postalCode;
                    clienteTotalModel.phone = nodeModel.phone?.Trim() ?? nodeModel.phone;
                    clienteTotalModel.cellphone = nodeModel.cellphone?.Trim() ?? nodeModel.cellphone;
                    clienteTotalModel.companyName = nodeModel.companyName?.Trim() ?? nodeModel.companyName;
                    clienteTotalModel.tradingName = nodeModel.tradingName?.Trim() ?? nodeModel.tradingName;
                    clienteTotalModel.street = nodeModel.street?.Trim() ?? nodeModel.street;
                    clienteTotalModel.complement = nodeModel.complement?.Trim() ?? nodeModel.complement;
                    clienteTotalModel.state = nodeModel.state?.Trim() ?? nodeModel.state;
                    clienteTotalModel.city = nodeModel.city?.Trim() ?? nodeModel.city;
                    clienteTotalModel.postalCode = nodeModel.postalCode?.Trim() ?? nodeModel.postalCode;
                    clienteTotalModel.neighborhood = nodeModel.neighborhood?.Trim() ?? nodeModel.neighborhood;
                    clienteTotalModel.houseNumber = Convert.ToInt32(nodeModel.houseNumber);
                    clienteTotalModel.email = nodeModel.email?.Trim() ?? nodeModel.email;
                    clienteTotalModel.website = nodeModel.website?.Trim() ?? nodeModel.website;
                    clienteTotalModel.personStatus = nodeModel.personStatus?.Trim() ?? nodeModel.personStatus;

                    if (clienteTotalModel.personType == "LEGAL_ENTITY")
                    {
                        clienteTotalModel.cnpj = nodeModel.taxPayerId?.Trim() ?? nodeModel.taxPayerId;
                        clienteTotalModel.inscr = nodeModel.identificationCard?.Trim() ?? nodeModel.identificationCard;
                    }
                    else
                    {
                        clienteTotalModel.cpf = nodeModel.taxPayerId?.Trim() ?? nodeModel.taxPayerId;
                        clienteTotalModel.rg = nodeModel.identificationCard?.Trim() ?? nodeModel.identificationCard;
                    }

                    //var billingAddress = nodeModel.addresses.FirstOrDefault(a => a.type == "BILLING");
                    //if (billingAddress != null)
                    //{
                    //    clienteTotalModel.BillingAddressModel = new AddressModel
                    //    {
                    //        type = billingAddress.type,
                    //        street = billingAddress.street?.Trim(),
                    //        houseNumber = billingAddress.houseNumber,
                    //        complement = billingAddress.complement?.Trim(),
                    //        neighborhood = billingAddress.neighborhood?.Trim(),
                    //        city = billingAddress.city?.Trim(),
                    //        state = billingAddress.state?.Trim(),
                    //        postalCode = billingAddress.postalCode?.Trim()
                    //    };
                    //}

                }

                return clienteTotalModel;
            }
            catch (Exception ex)
            {
                await _loggerRepository.WriteLog("TelaClientesRepository", "ListCliente", ex.Message);
                return null;
            }
        }

        public async Task<MensagemModel> AddCliente(ClienteTotalModel clienteTotalModel)
        {
            try
            {
                //Prepara classe para envio
                ClienteAddModel clienteAddModel = new ClienteAddModel();

                clienteAddModel.companyId = 1;                      //Alterar Futuramente
                clienteAddModel.lobId = clienteTotalModel.lobId;
                clienteAddModel.personType = clienteTotalModel.personType;
                clienteAddModel.companyName = clienteTotalModel.companyName;
                clienteAddModel.tradingName = clienteTotalModel.tradingName;
                clienteAddModel.street = clienteTotalModel.street;
                clienteAddModel.complement = clienteTotalModel.complement;
                clienteAddModel.state = clienteTotalModel.state;
                clienteAddModel.city = clienteTotalModel.city;
                clienteAddModel.email = clienteTotalModel.email;
                clienteAddModel.website = clienteTotalModel.website;

                clienteAddModel.postalCode = clienteTotalModel.postalCode?.Replace("-", "") ?? clienteTotalModel.postalCode;

                clienteAddModel.neighborhood = clienteTotalModel.neighborhood;
                clienteAddModel.houseNumber = clienteTotalModel.houseNumber;

                clienteAddModel.phone = clienteTotalModel.phone?.Replace("(", "") ?? clienteTotalModel.phone;
                clienteAddModel.phone = clienteAddModel.phone?.Replace(")", "") ?? clienteAddModel.phone;
                clienteAddModel.phone = clienteAddModel.phone?.Replace("-", "") ?? clienteAddModel.phone;

                clienteAddModel.cellphone = clienteTotalModel.cellphone?.Replace("(", "") ?? clienteTotalModel.cellphone;
                clienteAddModel.cellphone = clienteAddModel.cellphone?.Replace(")", "") ?? clienteAddModel.cellphone;
                clienteAddModel.cellphone = clienteAddModel.cellphone?.Replace("-", "") ?? clienteAddModel.cellphone;

                if (clienteTotalModel.personType == "LEGAL_ENTITY")
                {
                    clienteAddModel.taxPayerId = clienteTotalModel.cnpj?.Replace(".", "") ?? clienteTotalModel.cnpj;
                    clienteAddModel.taxPayerId = clienteAddModel.taxPayerId?.Replace("/", "") ?? clienteAddModel.taxPayerId;
                    clienteAddModel.taxPayerId = clienteAddModel.taxPayerId?.Replace("-", "") ?? clienteAddModel.taxPayerId;

                    clienteAddModel.identificationCard = clienteTotalModel.inscr;
                }
                else
                {
                    clienteAddModel.taxPayerId = clienteTotalModel.cpf?.Replace(".", "") ?? clienteTotalModel.cpf;
                    clienteAddModel.taxPayerId = clienteAddModel.taxPayerId?.Replace("-", "") ?? clienteAddModel.taxPayerId;

                    clienteAddModel.identificationCard = clienteTotalModel.rg;
                }

                clienteAddModel.addresses = new AddressModel
                {
                    type = "BILLING",
                    street = clienteTotalModel.street,
                    houseNumber = clienteTotalModel.houseNumber,
                    complement = clienteTotalModel.complement,
                    neighborhood = clienteTotalModel.neighborhood,
                    city = clienteTotalModel.city,
                    state = clienteTotalModel.state,
                    postalCode = clienteTotalModel.postalCode?.Replace("-", "")
                };

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _configuration["Parametros:url"]);
                request.Headers.Add("Authorization", "Bearer " + RetornoAutenticacaoModel.token);

                var mutation = @"mutation AddCustomer($customer: AddCustomerInput!) {
                    addCustomer(customer: $customer) {
                        companyName
                        tradingName
                        taxPayerId
                        identificationCard
                        postalCode
                        email
                        street
                        houseNumber
                        complement
                        neighborhood
                        city
                        state
                        phone
                        cellphone
                        website
                        lobId
                        companyId
                        personType
                        personStatus
                    }
                }";

                var variables = new { customer = clienteAddModel };
                var requestBody = new
                {
                    query = mutation,
                    variables = variables
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                request.Content = content;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseBody);

                if (result.errors != null)
                {
                    foreach (var error in result.errors)
                    {
                        await _loggerRepository.WriteLog("TelaClientesRepository", "AddCliente", error.message.ToString());
                    }
                    return new MensagemModel { IsSuccess = false, Message = $"Houve um erro na resposta !!!", MessageHeading = "AVISO" };
                }

                return new MensagemModel { IsSuccess = true, Message = $"Cliente {clienteAddModel.companyName} gravado com sucesso !!!", MessageHeading = "OK" };
            }
            catch (Exception ex)
            {
                await _loggerRepository.WriteLog("TelaClientesRepository", "AddCliente", ex.Message);
                return new MensagemModel { IsSuccess = false, Message = $"Houve um erro ao cadastrar/alterar o cliente !!!", MessageHeading = "FALHA" };
            }
        }

        public async Task<MensagemModel> UpdateCliente(ClienteTotalModel clienteTotalModel)
        {
            try
            {
                //Prepara classe para envio
                ClienteModel clienteModel = new ClienteModel();

                clienteModel.companyId = 1;                      //Alterar Futuramente
                clienteModel.uid = clienteTotalModel.uid;       //Obrigatório na alteração
                clienteModel.lobId = clienteTotalModel.lobId;
                clienteModel.personType = clienteTotalModel.personType;
                clienteModel.companyName = clienteTotalModel.companyName;
                clienteModel.tradingName = clienteTotalModel.tradingName;
                clienteModel.street = clienteTotalModel.street;
                clienteModel.complement = clienteTotalModel.complement;
                clienteModel.state = clienteTotalModel.state;
                clienteModel.city = clienteTotalModel.city;
                clienteModel.email = clienteTotalModel.email;
                clienteModel.website = clienteTotalModel.website;

                clienteModel.postalCode = clienteTotalModel.postalCode?.Replace("-", "") ?? clienteTotalModel.postalCode;

                clienteModel.neighborhood = clienteTotalModel.neighborhood;
                clienteModel.houseNumber = clienteTotalModel.houseNumber;

                clienteModel.phone = clienteTotalModel.phone?.Replace("(", "") ?? clienteTotalModel.phone;
                clienteModel.phone = clienteModel.phone?.Replace(")", "") ?? clienteModel.phone;
                clienteModel.phone = clienteModel.phone?.Replace("-", "") ?? clienteModel.phone;

                clienteModel.cellphone = clienteTotalModel.cellphone?.Replace("(", "") ?? clienteTotalModel.cellphone;
                clienteModel.cellphone = clienteModel.cellphone?.Replace(")", "") ?? clienteModel.cellphone;
                clienteModel.cellphone = clienteModel.cellphone?.Replace("-", "") ?? clienteModel.cellphone;

                if (clienteTotalModel.personType == "LEGAL_ENTITY")
                {
                    clienteModel.taxPayerId = clienteTotalModel.cnpj?.Replace(".", "") ?? clienteTotalModel.cnpj;
                    clienteModel.taxPayerId = clienteModel.taxPayerId?.Replace("/", "") ?? clienteModel.taxPayerId;
                    clienteModel.taxPayerId = clienteModel.taxPayerId?.Replace("-", "") ?? clienteModel.taxPayerId;

                    clienteModel.identificationCard = clienteTotalModel.inscr;
                }
                else
                {
                    clienteModel.taxPayerId = clienteTotalModel.cpf?.Replace(".", "") ?? clienteTotalModel.cpf;
                    clienteModel.taxPayerId = clienteModel.taxPayerId?.Replace("-", "") ?? clienteModel.taxPayerId;

                    clienteModel.identificationCard = clienteTotalModel.rg;
                }

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _configuration["Parametros:url"]);
                request.Headers.Add("Authorization", "Bearer " + RetornoAutenticacaoModel.token);

                var mutation = @"mutation AddCustomer($customer: AddCustomerInput!) {
                    addCustomer(customer: $customer) {
                        uid
                        companyName
                        tradingName
                        taxPayerId
                        identificationCard
                        postalCode
                        email
                        street
                        houseNumber
                        complement
                        neighborhood
                        city
                        state
                        phone
                        cellphone
                        website
                        lobId
                        companyId
                        personType
                        personStatus
                    }
                }";

                var variables = new { customer = clienteModel };
                var requestBody = new
                {
                    query = mutation,
                    variables = variables
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                request.Content = content;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseBody);

                // Verifique se há erros na resposta
                if (result.errors != null)
                {
                    foreach (var error in result.errors)
                    {
                        await _loggerRepository.WriteLog("TelaClientesRepository", "AddCliente", error.message.ToString());
                    }
                    return new MensagemModel { IsSuccess = false, Message = $"Houve um erro na resposta !!!", MessageHeading = "AVISO" };
                }

                return new MensagemModel { IsSuccess = true, Message = $"Cliente {clienteModel.companyName} atualizado com sucesso !!!", MessageHeading = "OK" };
            }
            catch (Exception ex)
            {
                await _loggerRepository.WriteLog("TelaClientesRepository", "AddCliente", ex.Message);
                return new MensagemModel { IsSuccess = false, Message = $"Houve um erro ao cadastrar/alterar o cliente !!!", MessageHeading = "FALHA" };
            }
        }

        public async Task<MensagemModel> DeleteCliente(int uid)
        {
            try
            {
                //var client = new HttpClient();
                //var request = new HttpRequestMessage(HttpMethod.Post, _configuration["Parametros:url"]);
                //request.Headers.Add("Authorization", "Bearer " + RetornoAutenticacaoModel.token);

                //var mutation = @"
                //        mutation AddCustomer($customer: AddCustomerInput!) {
                //        addCustomer(customer: $customer) {
                //        companyName
                //        tradingName
                //        taxPayerNumber
                //        identificationCard
                //        phone
                //        cellphone
                //        email
                //        street
                //        houseNumber
                //        complement
                //        neighborhood
                //        city
                //        state
                //        postalCode
                //        website
                //        lobId
                //        companyId
                //    }
                //}";

                //var variables = new { customer = client };
                //var requestBody = new
                //{
                //    query = mutation,
                //    variables = variables
                //};

                //var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                //request.Content = content;

                //var response = await client.SendAsync(request);
                //response.EnsureSuccessStatusCode();

                //var responseBody = await response.Content.ReadAsStringAsync();
                //dynamic result = JsonConvert.DeserializeObject(responseBody);

                //// Verifique se há erros na resposta
                //if (result.errors != null)
                //{
                //    foreach (var error in result.errors)
                //    {
                //        await _loggerRepository.WriteLog("TelaClientesRepository", "AddCliente", error.message.ToString());
                //    }
                //    return new MensagemModel { IsSuccess = false, Message = $"Houve um erro na resposta !!!", MessageHeading = "AVISO" };
                //}

                return new MensagemModel { IsSuccess = true, Message = $"Cliente excluído com sucesso !!!", MessageHeading = "OK" };
            }
            catch (Exception ex)
            {
                await _loggerRepository.WriteLog("TelaClientesRepository", "DeleteCliente", ex.Message);
                return new MensagemModel { IsSuccess = false, Message = $"Houve um erro ao excluir o cliente !!!", MessageHeading = "FALHA" };
            }
        }
    }

}
