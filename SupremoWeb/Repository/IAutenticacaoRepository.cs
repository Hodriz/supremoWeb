using SupremoWeb.Models;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace SupremoWeb.Repository
{
    public interface IAutenticacaoRepository
    {
        public Task<RetornoAutenticacaoModel> AutenticacaoLogin(LoginModel loginModel);
    }

    public class AutenticacaoRepository : IAutenticacaoRepository
    {
        private readonly ILoggerRepository _loggerRepository;
        private readonly IConfiguration _configuration;
        public AutenticacaoRepository(IConfiguration configuration, ILoggerRepository loggerRepository)
        {
            _configuration = configuration;
            _loggerRepository = loggerRepository;
        }

        public async Task<RetornoAutenticacaoModel> AutenticacaoLogin(LoginModel loginModel)
        {
            RetornoAutenticacaoModel retornoAutenticacao = new RetornoAutenticacaoModel();

            try
            {
                var httpClient = new HttpClient();
                var requestBody = new { query = @"query getToken($password:String!,$login:String!){signIn(password:$password,login:$login){accessCode}}", variables = loginModel };

                string jsonSerialized = System.Text.Json.JsonSerializer.Serialize(requestBody);
                HttpContent content = new StringContent(jsonSerialized, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(_configuration["Parametros:url"], content);
                var responseBody = await response.Content.ReadAsStringAsync();

                DataObjectModel data = JsonConvert.DeserializeObject<DataObjectModel>(responseBody);

                if (data.Data is not null)
                {
                    string token = data.Data.SignIn.AccessCode.ToString();

                    retornoAutenticacao.IsSuccess = true;
                    retornoAutenticacao.token = data.Data.SignIn.AccessCode.ToString();
                    retornoAutenticacao.Message = "Bem Vindo ao Sistema SupermoWEB";
                }
                else
                {
                    retornoAutenticacao.IsSuccess = false;
                    retornoAutenticacao.Message = "Usuário ou senha inválido !!!";
                }

                return retornoAutenticacao;
            }
            catch (Exception ex)
            {
                await _loggerRepository.WriteLog("AutenticacaoRepository", "AutenticacaoLogin", ex.Message);
                retornoAutenticacao.IsSuccess = false;
                retornoAutenticacao.Message = "Houve uma falha ao receber o token. Verificar o arquivo de Log !!!";

                return retornoAutenticacao;
            }
        }
    }

    //public class SignIn
    //{
    //    public string AccessCode { get; set; }
    //}

    //public class Data
    //{
    //    public SignIn SignIn { get; set; }
    //}

    //public class DataObject
    //{
    //    public Data Data { get; set; }
    //}
}
