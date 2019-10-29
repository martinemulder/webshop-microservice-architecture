using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.CommonModels.CommonExceptions
{
    [Serializable]
    public class EmailAllreadyExistsException : Exception
    {
        public EmailAllreadyExistsException(string message) : base(message)
        {
        }

        public EmailAllreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
