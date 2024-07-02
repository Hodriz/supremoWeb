namespace SupremoWeb.Models
{
    public class NodeModel
    {
        public string uid { get; set; }
        public string companyId { get; set; }
        public string id { get; set; }
        public string personType { get; set; }
        public string personStatus { get; set; }
        public string companyName { get; set; }
        public string tradingName { get; set; }
        public string taxPayerId { get; set; }
        public string identificationCard { get; set; }
        public string phone { get; set; }
        public string cellphone { get; set; }
        public string email { get; set; }
        public string genericId { get; set; }
        public string street { get; set; }
        public string houseNumber { get; set; }
        public string complement { get; set; }
        public string neighborhood { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public string website { get; set; }
        public string lobId { get; set; }
        public CustomerSellerModel[] customerSellers { get; set; }
    }
}
