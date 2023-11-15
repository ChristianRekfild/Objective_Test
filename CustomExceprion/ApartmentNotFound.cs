namespace ApartmentData.CustomExceprion
{
    public class ApartmentNotFound : Exception
    {
        public ApartmentNotFound(string? message) : base(message)
        {
        }
    }
}
