namespace Lab5.Models
{
    public class Evaluation
    {
        public int Id { get; set; }
        public int EstateId { get; set; }
        public RealEstate Estate { get; set; } = null!;
        public DateTime DateOfRelease { get; set; }
        public int CriteriaId { get; set; }
        public Criteria Criteria { get; set; } = null!;
        public int Value { get; set; }
    }
}
