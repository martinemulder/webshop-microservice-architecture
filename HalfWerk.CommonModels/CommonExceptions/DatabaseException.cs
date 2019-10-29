using System;
using System.Runtime.Serialization;

namespace HalfWerk.CommonModels.CommonExceptions
{
    [Serializable]
    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base(message)
        {
        }

        protected DatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}