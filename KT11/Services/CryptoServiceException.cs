namespace KT11.Services
{

    public class CryptoServiceException : Exception
    {
        public CryptoServiceException(string message) : base(message) { }

        public CryptoServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
