using System;
using System.IO;
using System.Collections.Generic;

namespace Convertor.Json
{
    public class JsonArray : JsonEntity
    {
        public List<JsonEntity> Items { get; private set; }

        public JsonArray()
        {
            Items = new List<JsonEntity>();
        }

        public override void Stringify(StreamWriter writer, StringifyOptions options)
        {
            if (Items.Count == 0)
            {
                if (options.Inlined)
                {
                    writer.Write("[]");
                }
                else
                {
                    writer.Write("[");
                    options.BreakLine(writer);
                    writer.Write("]");
                }

                return;
            }

            writer.Write('[');
            
            options.currentIndent++;
            options.BreakLine(writer);

            int index = 0;
            foreach (JsonEntity i in Items)
            {
                i.Stringify(writer, options);

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

            options.currentIndent--;
            options.BreakLine(writer);

            writer.Write(']');
        }
    }
}
