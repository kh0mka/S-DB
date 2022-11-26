namespace Beltelecom.ClassEntities
{
    public class Affiliate
    {
        public int AffId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Site { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int ProvId { get; set; }

    }
}
