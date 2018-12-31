using System;
using System.IO;
using System.Collections.Generic;

namespace Convertor.Xml
{
    public class XmlElement : XmlNode
    {
        public string Tag { get; private set; }

        public List<XmlAttribute> Attributes { get; private set; }

        public bool Pair {
            get => Content.Count > 0 ? true : pair;
            set {
                if (Content.Count > 0 && !value)
                    throw new InvalidOperationException("Non-empty tag cannot be non-pair.");

                pair = value;
            }
        }
        private bool pair;

        public List<XmlNode> Content { get; private set; }

        public XmlElement(string tag, bool pair = true)
        {
            this.Tag = tag;
            this.pair = pair;

            Attributes = new List<XmlAttribute>();
            Content = new List<XmlNode>();
        }

        public override void Stringify(StreamWriter writer, StringifyOptions options)
        {
            bool printPair = Pair;

            if (options.collapseEmptyTags && Content.Count == 0)
                printPair = false;

            writer.Write("<");
            writer.Write(Tag);

            foreach (XmlAttribute a in Attributes)
            {
                writer.Write(" ");
                a.Stringify(writer, options);
            }

            if (!printPair)
            {
                writer.Write("/>");
                return;
            }

            writer.Write(">");

            if (Content.Count > 0)
            {
                options.currentIndent++;
                
                foreach (XmlNode n in Content)
                {
                    options.BreakLine(writer);
                    n.Stringify(writer, options);
                }

                options.currentIndent--;
                options.BreakLine(writer);
            }

            writer.Write("</");
            writer.Write(Tag);
            writer.Write(">");
        }
    }
}
