namespace SupremoWeb.Models
{
    public static class RetornoAutenticacaoModel
    {
        public static bool IsSuccess { get; set; }
        public static string? Message { get; set; }
        public static string? MessageHeading { get; set; }

        public static string token { get; set; }
    }
}
