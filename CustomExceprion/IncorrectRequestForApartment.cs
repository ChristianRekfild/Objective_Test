namespace ApartmentData.CustomExceprion
{
    public class IncorrectRequestForApartment : Exception
    {
        public IncorrectRequestForApartment(string? message) : base(message)
        {
        }
    }
}
