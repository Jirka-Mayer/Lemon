using System;

namespace Lemon
{
    [System.Serializable]
    public class ParserAlreadyUsedException : Exception
    {
        public ParserAlreadyUsedException() { }
        public ParserAlreadyUsedException(string message) : base(message) { }
        public ParserAlreadyUsedException(string message, System.Exception inner) : base(message, inner) { }
        protected ParserAlreadyUsedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
