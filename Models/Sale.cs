namespace Lab5.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public int EstateId { get; set; }
        public DateTime DateOfRelease { get; set; }
        public int RealtorId { get; set; }
        public int Cost { get; set; }
    }
}
