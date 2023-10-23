namespace Lab5.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public int EstateId { get; set; }
        public RealEstate Estate { get; set; } = null!;
        public DateTime DateOfRelease { get; set; }
        public int RealtorId { get; set; }
        public Realtor Realtor { get; set; } = null!;
        public int Cost { get; set; }
    }
}
