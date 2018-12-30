using System;
using System.IO;
using System.Text;

namespace Convertor.Json
{
    public class JsonString : JsonEntity
    {
        /// <summary>
        /// Actual value of the string
        /// </summary>
        public string Value { get; private set; }

        public JsonString(string value)
        {
            this.Value = value;
        }

        public override void Stringify(StreamWriter writer, StringifyOptions options)
        {
            writer.Write('"');

            for (int i = 0; i < Value.Length; i++)
            {
                switch (Value[i])
                {
                    case '"': writer.Write("\\\""); break;
                    case '\\': writer.Write("\\\\"); break;
                    case '/': writer.Write("\\/"); break;
                    case '\n': writer.Write("\\n"); break;
                    case '\r': writer.Write("\\r"); break;
                    case '\t': writer.Write("\\t"); break;
                    default: writer.Write(Value[i]); break;
                }
            }

            writer.Write('"');
        }
    }
}
