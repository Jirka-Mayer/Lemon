using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Convertor.Json
{
    public class JsonObject : JsonEntity
    {
        public Dictionary<string, JsonEntity> Items { get; private set; }

        public JsonObject()
        {
            Items = new Dictionary<string, JsonEntity>();
        }

        public override void Stringify(StreamWriter writer, StringifyOptions options)
        {
            writer.Write('{');
            
            options.currentIndent++;
            options.BreakLine(writer);

            int index = 0;
            foreach (KeyValuePair<string, JsonEntity> i in Items)
            {
                new JsonString(i.Key).Stringify(writer, options);
                writer.Write(": ");
                i.Value.Stringify(writer, options);

                // if not the last line
                if (index < Items.Count - 1)
                {
                    writer.Write(",");
                    
                    if (options.Inlined)
                        writer.Write(" ");
                    else
                        options.BreakLine(writer);
                }

                index++;
            }

            // on last line
            options.currentIndent--;
            options.BreakLine(writer);

            writer.Write('}');
        }
    }
}
