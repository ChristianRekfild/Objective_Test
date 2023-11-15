namespace ApartmentData.Models
{
    public class ApartmentDTO_withPriceList
    {
        public int Id { get; init; }
        public int NumberOfRooms { get; set; }
        public string Url { get; set; }

        // список цен
        public List<ApartmentCostDTO> ApartmentPrices { get; set; }
        
    }
}
