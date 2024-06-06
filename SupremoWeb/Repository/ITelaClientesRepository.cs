using Newtonsoft.Json;
using SupremoWeb.Models;
using System.Text;

namespace SupremoWeb.Repository
{
    public interface ITelaClientesRepository
    {
        Task<IEnumerable<NodeModel>> ListaAllClientes();
        Task<bool> AddCliente(ClienteModel cliente);
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

        public async Task<IEnumerable<NodeModel>> ListaAllClientes()
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

        public async Task<bool> AddCliente(ClienteModel cliente)
        {
            try
            {
                string token = RetornoAutenticacaoModel.token;

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://supremo-api.jelastic.saveincloud.net/api");
                request.Headers.Add("Authorization", "Bearer " + token);
                var content = new StringContent(
                    "{\"query\":\"mutation ($cliente: ClienteInput!) { addCliente(cliente: $cliente) }\",\"variables\":{\"cliente\":" + JsonConvert.SerializeObject(cliente) + "}}",
                    Encoding.UTF8,
                    "application/json"
                );
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseBody);

                return result.data.addCliente;
            }
            catch (Exception ex)
            {
                await _loggerRepository.WriteLog("ClienteRepository", "AddCliente", ex.Message);
                return false;
            }
        }

    }

}
