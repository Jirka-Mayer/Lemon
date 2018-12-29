using System;
using System.IO;

namespace Convertor.Json
{
    public class JsonBoolean : JsonEntity
    {
        public Boolean Value { get; private set; }

        public JsonBoolean(bool value)
        {
            this.Value = value;
        }

        public override void Stringify(StreamWriter writer, StringifyOptions options)
        {
            if (Value)
                writer.Write("true");
            else
                writer.Write("false");
        }
    }
}
