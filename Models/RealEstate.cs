namespace Lab5.Models
{
    public class RealEstate
    {
        public int Id { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; } = null!;
        public string Address { get; set; } = string.Empty;
        public int Floor { get; set; }
        public int NumberOftRooms { get; set; }
        public int Type { get; set; }
        public RealEstateStatus Status { get; set; }
        public int Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int MaterialId { get; set; }
        public Material Material { get; set; } = null!;
        public int Square { get; set; }
        public DateTime DateOfAnnouncement { get; set; }
    }
}
