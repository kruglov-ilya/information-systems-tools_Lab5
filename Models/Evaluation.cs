namespace Lab5.Models
{
    public class Evaluation
    {
        public int Id { get; set; }
        public int EstateId { get; set; }
        public DateTime DateOfRelease { get; set; }
        public int CriteriaId { get; set; }
        public int Value { get; set; }
    }
}
