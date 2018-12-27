using System;

namespace Convertor.Json
{
    [System.Serializable]
    public class JsonParsingException : System.Exception
    {
        public JsonParsingException() { }
        public JsonParsingException(string message) : base(message) { }
        public JsonParsingException(string message, System.Exception inner) : base(message, inner) { }
        protected JsonParsingException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
