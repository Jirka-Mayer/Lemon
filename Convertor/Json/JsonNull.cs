using System;
using System.IO;

namespace Convertor.Json
{
    public class JsonNull : JsonEntity
    {
        public JsonNull() { }

        public override void Stringify(StreamWriter writer, StringifyOptions options)
        {
            writer.Write("null");
        }
    }
}
