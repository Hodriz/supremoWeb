using System.ComponentModel.DataAnnotations;

namespace SupremoWeb.Models
{
    public class ClienteModel
    {
        public int uid { get; set; }

        public int lobId { get; set; }
        public float companyId { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(50, ErrorMessage = "Campo não pode exceder 50 caracteres")]
        public string? companyName { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(40, ErrorMessage = "Campo não pode exceder 40 caracteres")]
        public string? tradingName { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(60, ErrorMessage = "Campo não pode exceder 60 caracteres")]
        public string? street { get; set; }

        [StringLength(60, ErrorMessage = "Campo não pode exceder 60 caracteres")]
        public string? complement { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? state { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(40, ErrorMessage = "Campo não pode exceder 40 caracteres")]
        public string? city { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string? postalCode { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(40, ErrorMessage = "Campo não pode exceder 40 caracteres")]
        public string? neighborhood { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public float houseNumber { get; set; } 

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(14, ErrorMessage = "Campo não pode exceder 14 caracteres")]
        public string? taxPayerId { get; set; }

        [StringLength(15, ErrorMessage = "Campo não pode exceder 15 caracteres")]
        public string? identificationCard { get; set; }

        public string? phone { get; set; }

        public string? cellphone { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100, ErrorMessage = "Campo não pode exceder 100 caracteres")]
        public string? email { get; set; }

        //[Url(ErrorMessage = "Formato de URL inválido")]
        [StringLength(50, ErrorMessage = "Campo não pode exceder 50 caracteres")]
        public string? website { get; set; }
    }
}
