namespace Beltelecom.ClassEntities
{
    public class Clients
    {
        public int ClientId { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int TariffId { get; set; }

    }
}
