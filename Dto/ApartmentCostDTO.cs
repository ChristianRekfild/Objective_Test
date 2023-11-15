namespace ApartmentData.Models
{
    public class ApartmentCostDTO
    {
        public int Id { get; init; }
        public decimal Price { get; set; }
        public DateTime DateAdded { get; init; }
        public int ApartmentId { get; init; }
    }
}
