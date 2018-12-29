using System;
using System.IO;

namespace Convertor.Xml
{
    public class XmlText : XmlNode
    {
        public string Value { get; private set; }

        public XmlText(string value)
        {
            this.Value = value;
        }

        public override void Stringify(StreamWriter writer, StringifyOptions options)
        {
            for (int i = 0; i < Value.Length; i++)
            {
                switch (Value[i])
                {
                    case '"': writer.Write("&quot;"); break;
                    case '&': writer.Write("&amp;"); break;
                    case '<': writer.Write("&lt;"); break;
                    case '>': writer.Write("&gt;"); break;
                    default: writer.Write(Value[i]); break;
                }
            }
        }
    }
}
