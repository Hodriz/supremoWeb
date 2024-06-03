using System.Text;

namespace SupremoWeb.Repository
{
    public interface ILoggerRepository
    {
        public Task<List<string>> GetFilesLog();
        public Task<string> GetLog(string fileLog);
        public Task WriteLog(string classe, string funcao, string messageLog);
    }

    public class LoggerRepository : ILoggerRepository
    {
        private readonly IConfiguration _config;
        public LoggerRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<string>> GetFilesLog()
        {
            List<string> listFiles = new List<string>();

            var files = Directory.GetFiles(_config["URL:uriAPI"]);

            foreach (var file in files)
            {
                listFiles.Add(Path.GetFileName(file));
            }

            return await Task.FromResult(listFiles);
        }

        public async Task<string> GetLog(string fileLog)
        {
            try
            {
                return File.ReadAllText($"{_config["Parametros:LogErrorDirectory"]}{@"\"}{fileLog}");
            }
            catch (Exception e)
            {
                await WriteLog("LoggerRepository", "GetLog", e.Message);
                return string.Empty;
            }
        }

        public Task WriteLog(string classe, string funcao, string messageLog)
        {
            string logFile = $"{_config["Parametros:LogErrorDirectory"]}{@"\"}{_config["Parametros:LogErrorFile"]}{"-"}{DateTime.Now.ToString("dd-MM-yyyy")}{".log"}";
            StringBuilder sRowLog = new StringBuilder();

            if (!Directory.Exists(_config["Parametros:LogErrorDirectory"]))
            {
                Directory.CreateDirectory(_config["Parametros:LogErrorDirectory"]);
            }

            Stream stream = new FileStream(logFile, FileMode.Append);

            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                sRowLog.Append(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                sRowLog.Append("|");
                sRowLog.Append($"{"CLASSE:"}{classe}");
                sRowLog.Append("|");
                sRowLog.Append($"{"FUNÇÃO:"}{funcao}");
                sRowLog.Append("|");
                sRowLog.Append(messageLog);

                writer.WriteLine(sRowLog.ToString());
            }

            return Task.CompletedTask;
        }
    }
}
