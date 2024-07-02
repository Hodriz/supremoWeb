namespace SupremoWeb.Models
{
    public class ClienteBaseModel
    {
        public int? lobId { get; set; }
        public float? companyId { get; set; }
        public string? personType { get; set; }
        public string? companyName { get; set; }
        public string? tradingName { get; set; }
        public string? street { get; set; }
        public string? complement { get; set; }
        public string? state { get; set; }
        public string? city { get; set; }
        public string? postalCode { get; set; }
        public string? neighborhood { get; set; }
        public float? houseNumber { get; set; }
        public string? taxPayerId { get; set; }
        public string? identificationCard { get; set; }
        public string? phone { get; set; }
        public string? cellphone { get; set; }
        public string? email { get; set; }
        public string? website { get; set; }



    }

    public class ClienteAddModel : ClienteBaseModel
    {
        
    }

    public class ClienteModel : ClienteBaseModel            //Para Update de clientes
    {
        public int? uid { get; set; }
    }

    public class ClienteTotalModel : ClienteBaseModel       //Para clientes novos
    {
        public string? cpf { get; set; }
        public string? cnpj { get; set; }
        public int? uid { get; set; }
    }
}
