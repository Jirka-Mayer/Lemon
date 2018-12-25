using System;

namespace Lemon
{
    [System.Serializable]
    public class ParserStillPristineException : Exception
    {
        public ParserStillPristineException() { }
        public ParserStillPristineException(string message) : base(message) { }
        public ParserStillPristineException(string message, Exception inner) : base(message, inner) { }
        protected ParserStillPristineException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
