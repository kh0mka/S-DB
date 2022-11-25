namespace Beltelecom
{
    public class Tariff
    {
        public int TariffId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public float Speed { get; set; }
        public int TechId { get; set; }
        public int ProvId { get; set; }

    }
}
