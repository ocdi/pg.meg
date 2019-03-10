using System;
using System.Runtime.Serialization;

namespace pg.meg.exceptions
{
    public class MegFileMalformedException : Exception
    {
        public MegFileMalformedException()
        {
        }

        public MegFileMalformedException(string message) : base(message)
        {
        }

        public MegFileMalformedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MegFileMalformedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
