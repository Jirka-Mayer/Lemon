using System;

namespace Convertor.Xml
{
    [System.Serializable]
    public class XmlParsingException : System.Exception
    {
        public XmlParsingException() { }
        public XmlParsingException(string message) : base(message) { }
        public XmlParsingException(string message, System.Exception inner) : base(message, inner) { }
        protected XmlParsingException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
