using System;
using System.Runtime.Serialization;

namespace HalfWerk.CommonModels.CommonExceptions
{
    [Serializable]
    public class StartupException : Exception
    {
        public StartupException(string message) : base(message)
        {
        }

        protected StartupException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}