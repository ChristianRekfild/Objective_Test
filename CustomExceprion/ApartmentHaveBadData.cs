namespace ApartmentData.CustomExceprion
{
    public class ApartmentHaveBadData : Exception
    {
        public ApartmentHaveBadData(string? message) : base(message)
        {
        }
    }
}
