namespace ApartmentData.Models
{
    public class ApartmentDTO_withPriceOnly
    {
        public int Id { get; init; }
        public int NumberOfRooms { get; set; }
        public string Url { get; set; }

        // В данном DTO будет только одна цена, так как нет смысла выводить список цен
        public decimal Price { get; set; }
        
    }
}
