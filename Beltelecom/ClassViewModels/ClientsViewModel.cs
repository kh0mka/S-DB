namespace Beltelecom.ClassViewModels
{
    public class ClientsViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string TariffName { get; set; } = string.Empty;
    }
}
