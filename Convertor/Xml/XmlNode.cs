using System;
using System.IO;

namespace Convertor.Xml
{
    public abstract class XmlNode
    {
        /// <summary>
        /// Serializes the node into a string form
        /// </summary>
        public abstract void Stringify(StreamWriter writer, StringifyOptions options);

        /// <summary>
        /// Serializes the node into a string
        /// </summary>
        public string Stringify(StringifyOptions options = new StringifyOptions())
        {
            using (MemoryStream stream = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(stream))
            using (StreamReader reader = new StreamReader(stream))
            {
                this.Stringify(writer, options);

                writer.Flush();
                
                stream.Position = 0;

                return reader.ReadToEnd();
            }
        }

        public override string ToString()
        {
            return "Xml(" + this.Stringify() + ")";
        }
    }
}
