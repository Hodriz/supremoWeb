using System.ComponentModel.DataAnnotations;

namespace SupremoWeb.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? login { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? password { get; set; }
    }
}