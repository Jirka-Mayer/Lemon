using System;
using System.IO;

namespace Convertor.Json
{
    /// <summary>
    /// Represents any JSON entity
    /// </summary>
    public abstract class JsonEntity
    {
        /// <summary>
        /// Serializes the entity into a string written to a string writer
        /// </summary>
        public abstract void Stringify(StreamWriter writer, StringifyOptions options);

        /// <summary>
        /// Serializes the entity into a string
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
            return "Json(" + this.Stringify() + ")";
        }
    }
}
