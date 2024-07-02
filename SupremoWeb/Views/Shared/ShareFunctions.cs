using SupremoWeb.Models;
using SupremoWeb.Repository;

namespace SupremoWeb.Views.Shared
{
    public static class ShareFunctions
    {
        private static IConfiguration _configuration;
        private static ILoggerRepository _loggerRepository;

        public static void Initialize(IConfiguration configuration, ILoggerRepository loggerRepository)
        {
            _configuration = configuration;
            _loggerRepository = loggerRepository;
        }

        public static async Task<string> RecebeResponseBody(StringContent stringContent)
        {
            try
            {
                var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _configuration["Parametros:url"]);
                request.Headers.Add("Authorization", "Bearer " + RetornoAutenticacaoModel.token);
                request.Content = stringContent;
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
            }
            catch (Exception ex)
            {
                await _loggerRepository.WriteLog("ShareFunctions", "RecebeResponseBody", ex.Message);
                return null;
            }
        }
    }
}
