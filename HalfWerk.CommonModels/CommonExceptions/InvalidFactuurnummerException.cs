using System;

namespace HalfWerk.CommonModels.CommonExceptions
{
    [Serializable]
    public class InvalidFactuurnummerException : Exception
    {
        public InvalidFactuurnummerException(string message) : base(message)
        {
        }

        public InvalidFactuurnummerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}