using System.ComponentModel.DataAnnotations;

namespace SupremoWeb.Models
{
    public class ClienteModel
    {
        //[Required(ErrorMessage = "Campo Obrigatório")]
        public int lobId { get; set; }
        public float companyId { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? companyName { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? tradingName { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? street { get; set; }

        public string? complement { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? state { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? city { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? postalCode { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? neighborhood { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public float houseNumber { get; set; } 

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? taxPayerNumber { get; set; }

        public string? identificationCard { get; set; }

        public string? phone { get; set; }

        public string? cellphone { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string? email { get; set; }

        //[Url(ErrorMessage = "Formato de URL inválido")]
        public string? website { get; set; }
    }
}
