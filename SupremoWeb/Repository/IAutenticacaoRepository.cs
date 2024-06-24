using SupremoWeb.Models;
using System.Text;
using Newtonsoft.Json;

namespace SupremoWeb.Repository
{
    public interface IAutenticacaoRepository
    {
        Task AutenticacaoLogin(LoginModel loginModel);
        Task<string> ObtemToken(LoginModel loginModel);
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

        public async Task AutenticacaoLogin(LoginModel loginModel)
        {
            string token = await ObtemToken(loginModel);

            if (!string.IsNullOrEmpty(token))
            {
                if (token != ".")
                {
                    RetornoAutenticacaoModel.IsSuccess = true;
                    RetornoAutenticacaoModel.token = token;
                    RetornoAutenticacaoModel.Message = $"Bem Vindo {loginModel.login} ao Sistema SupremoWEB";
                    RetornoAutenticacaoModel.MessageHeading = "OK";
                }
                else
                {
                    RetornoAutenticacaoModel.IsSuccess = false;
                    RetornoAutenticacaoModel.Message = "Usuário ou senha inválido !!!";
                    RetornoAutenticacaoModel.MessageHeading = "AVISO";
                }

            }
            else
            {
                RetornoAutenticacaoModel.IsSuccess = false;
                RetornoAutenticacaoModel.Message = "Houve uma falha ao receber o token. Verificar arquivo de Log !!!";
                RetornoAutenticacaoModel.MessageHeading = "FALHA";
            }

        }

        public async Task<string> ObtemToken(LoginModel loginModel)
        {
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
                    return data.Data.SignIn.AccessCode.ToString();
                }
                else
                {
                    return ".";
                }
            }
            catch (Exception ex)
            {
                await _loggerRepository.WriteLog("AutenticacaoRepository", "ObtemToken", ex.Message);
                return string.Empty;
            }

        }

    }

}
