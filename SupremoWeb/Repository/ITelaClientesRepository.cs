using Newtonsoft.Json;
using SupremoWeb.Models;
using System.Text;

namespace SupremoWeb.Repository
{
    public interface ITelaClientesRepository
    {
        Task<IEnumerable<NodeModel>> ListAllClientes();
        Task<MensagemModel> AddCliente(ClienteModel cliente);
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
                string token = RetornoAutenticacaoModel.token;

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://supremo-api.jelastic.saveincloud.net/api");
                request.Headers.Add("Authorization", "Bearer " + token);
                var content = new StringContent("{\"query\":\"query Node($limit: Int) {\\r\\n  Customers(limit: $limit) {\\r\\n    edges {\\r\\n      node {\\r\\n        uid\\r\\n        companyId\\r\\n        id\\r\\n        companyName\\r\\n        tradingName\\r\\n        taxPayerNumber\\r\\n        identificationCard\\r\\n        phone\\r\\n        cellphone\\r\\n        email\\r\\n        street\\r\\n        houseNumber\\r\\n        complement\\r\\n        neighborhood\\r\\n        city\\r\\n        state\\r\\n        postalCode\\r\\n        customerSellers {\\r\\n          sellerId\\r\\n          seller {\\r\\n            name\\r\\n          }\\r\\n        }\\r\\n      }\\r\\n    }\\r\\n  }\\r\\n}\",\"variables\":{\"limit\":50}}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

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

        public async Task<MensagemModel> AddCliente(ClienteModel cliente)
        {
            try
            {
                cliente.lobId = 0;
                cliente.companyId = 0;
                cliente.postalCode = cliente.postalCode.Replace("-", "");
                cliente.taxPayerNumber = cliente.postalCode.Replace(".", "");
                cliente.taxPayerNumber = cliente.postalCode.Replace("/", "");

                string token = RetornoAutenticacaoModel.token;

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://supremo-api.jelastic.saveincloud.net/api");
                request.Headers.Add("Authorization", "Bearer " + token);

                var mutation = @"
                        mutation AddCustomer($customer: AddCustomerInput!) {
                        addCustomer(customer: $customer) {
                        companyName
                        tradingName
                        taxPayerNumber
                        identificationCard
                        phone
                        cellphone
                        email
                        street
                        houseNumber
                        complement
                        neighborhood
                        city
                        state
                        postalCode
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

    }

}
