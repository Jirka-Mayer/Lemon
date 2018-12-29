using System;
using System.IO;

namespace Convertor.Json
{
    public class JsonNumber : JsonEntity
    {
        public double Value { get; private set; }

        public JsonNumber(double value)
        {
            this.Value = value;
        }

        public override void Stringify(StreamWriter writer, StringifyOptions options)
        {
            writer.Write(this.Value.ToString());
        }
    }
}
