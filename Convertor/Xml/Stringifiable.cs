using System;
using System.IO;

namespace Convertor.Xml
{
    public abstract class Stringifiable
    {
        /// <summary>
        /// Serializes the object into a string form
        /// </summary>
        public abstract void Stringify(StreamWriter writer, StringifyOptions options);

        /// <summary>
        /// Serializes the object into a string
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
    }
}
