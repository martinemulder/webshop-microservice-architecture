using System;
using System.Runtime.Serialization;

namespace HalfWerk.CommonModels.CommonExceptions
{
    [Serializable]
    public class InvalidUpdateException : Exception
    {
        public InvalidUpdateException()
        {
        }

        protected InvalidUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public InvalidUpdateException(string message) : base(message)
        {
        }
    }
}