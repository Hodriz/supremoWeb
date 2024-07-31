namespace SupremoWeb.Models
{
    public class AddressModel
    {
        public string? type { get; set; }
        public string? street { get; set; }
        public float? houseNumber { get; set; }
        public string? complement { get; set; }
        public string? neighborhood { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? postalCode { get; set; }
    }
}
