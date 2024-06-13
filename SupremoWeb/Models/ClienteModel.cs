using System.ComponentModel.DataAnnotations;

namespace SupremoWeb.Models
{
    public class ClienteModel
    {
        //[Required(ErrorMessage = "Campo Obrigatório")]
        public int lobId { get; set; }  

        [Required(ErrorMessage = "Campo Obrigatório")]
        public float CompanyId { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? TradingName { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? Street { get; set; }

        public string? Complement { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? State { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? PostalCode { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? Neighborhood { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public float HouseNumber { get; set; } 

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? TaxPayerNumber { get; set; }

        public string? IdentificationCard { get; set; }

        public string? Phone { get; set; }

        public string? Cellphone { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string? Email { get; set; }

        //[Url(ErrorMessage = "Formato de URL inválido")]
        public string? Website { get; set; }
    }
}
