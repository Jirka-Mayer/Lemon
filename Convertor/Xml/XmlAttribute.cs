using System;
using System.IO;

namespace Convertor.Xml
{
    public class XmlAttribute : Stringifiable
    {
        public string Name { get; private set; }
        public XmlText Value { get; private set; }

        public XmlAttribute(string name, XmlText value)
        {
            this.Name = name;
            this.Value = value;
        }

        public override void Stringify(StreamWriter writer, StringifyOptions options)
        {
            writer.Write(Name);
            writer.Write("=\"");
            Value.Stringify(writer, options);
            writer.Write("\"");
        }
    }
}
