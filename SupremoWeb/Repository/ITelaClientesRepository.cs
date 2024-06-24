using Newtonsoft.Json;
using SupremoWeb.Models;
using SupremoWeb.Views.Shared;
using System.Text;

namespace SupremoWeb.Repository
{
    public interface ITelaClientesRepository
    {
        Task<IEnumerable<NodeModel>> ListAllClientes();
        Task<ClienteModel> ListCliente(int uid);
        Task<MensagemModel> AddCliente(ClienteModel cliente);
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

        public async Task<ClienteModel> ListCliente(int uid)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _configuration["Parametros:url"]);
                request.Headers.Add("Authorization", "Bearer " + RetornoAutenticacaoModel.token);

                // Construir a string JSON com o valor do parâmetro uid
                var queryObject = new
                {
                    query = @"query Customers($filter: CustomerFilter) {
                        customers(filter: $filter) {
                            edges {
                                node {
                                    uid
                                    companyId
                                    id
                                    companyName
                                    tradingName
                                    taxPayerId
                                    identificationCard
                                    phone
                                    cellphone
                                    email
                                    neighborhood
                                    street
                                    houseNumber
                                    complement
                                    city
                                    state
                                    postalCode
                                    website
                                    lobId
                                }
                            }
                        }
                    }",
                    variables = new
                    {
                        filter = new
                        {
                            uid = new
                            {
                                _eq = uid
                            }
                        }
                    }
                };

                var jsonQuery = JsonConvert.SerializeObject(queryObject);

                var content = new StringContent(jsonQuery, Encoding.UTF8, "application/json");
                request.Content = content;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                RootObjectModel rootObject = JsonConvert.DeserializeObject<RootObjectModel>(responseBody);
                IEnumerable<NodeModel> nodes = rootObject.data.Customers.edges.Select(e => e.node).ToArray();

                NodeModel nodeModel = nodes.FirstOrDefault();
                ClienteModel clienteModel = new ClienteModel();

                if (nodeModel != null)
                {
                    clienteModel.uid = Convert.ToInt32(nodeModel.uid);
                    clienteModel.lobId = Convert.ToInt32(nodeModel.lobId);
                    clienteModel.companyId = Convert.ToInt32(nodeModel.companyId);
                    clienteModel.companyName = nodeModel.companyName.Trim();
                    clienteModel.tradingName = nodeModel.tradingName.Trim();
                    clienteModel.street = nodeModel.street.Trim();
                    clienteModel.complement = nodeModel.complement.Trim();
                    clienteModel.state = nodeModel.state.Trim();
                    clienteModel.city = nodeModel.city.Trim();
                    clienteModel.postalCode = nodeModel.postalCode.Trim();
                    clienteModel.neighborhood = nodeModel.neighborhood.Trim();
                    clienteModel.houseNumber = Convert.ToInt32(nodeModel.houseNumber);
                    clienteModel.taxPayerId = nodeModel.taxPayerId.Trim();
                    clienteModel.identificationCard = nodeModel.identificationCard.Trim();
                    clienteModel.phone = nodeModel.phone.Trim();
                    clienteModel.cellphone = nodeModel.cellphone.Trim();
                    clienteModel.email = nodeModel.email.Trim();
                    clienteModel.website = nodeModel.website.Trim();
                }

                return clienteModel;
            }
            catch (Exception ex)
            {
                await _loggerRepository.WriteLog("TelaClientesRepository", "ListCliente", ex.Message);
                return null;
            }
        }
                
        public async Task<MensagemModel> AddCliente(ClienteModel cliente)
        {
            try
            {
                cliente.lobId = 0;
                cliente.companyId = 0;
                cliente.postalCode = cliente.postalCode.Replace("-", "");
                cliente.taxPayerId = cliente.taxPayerId.Replace(".", "");
                cliente.taxPayerId = cliente.taxPayerId.Replace("/", "");
                cliente.cellphone = cliente.cellphone.Replace("(", "");
                cliente.cellphone = cliente.cellphone.Replace(")", "");
                cliente.cellphone = cliente.cellphone.Replace("-", "");
                cliente.phone = cliente.phone.Replace("(", "");
                cliente.phone = cliente.phone.Replace(")", "");
                cliente.phone = cliente.phone.Replace("-", "");

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
                    }
                }";

                var variables = new { customer = cliente };
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

                return new MensagemModel { IsSuccess = true, Message = $"Cliente {cliente.companyName} cadastrado com sucesso !!!", MessageHeading = "OK" };
            }
            catch (Exception ex)
            {
                await _loggerRepository.WriteLog("TelaClientesRepository", "AddCliente", ex.Message);
                return new MensagemModel { IsSuccess = false, Message = $"Houve um erro ao cadastrar o cliente !!!", MessageHeading = "FALHA" };
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
