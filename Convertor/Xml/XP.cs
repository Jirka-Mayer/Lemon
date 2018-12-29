using System;
using System.Linq;
using System.Collections.Generic;
using Lemon;

namespace Convertor.Xml
{
    /// <summary>
    /// XML parsers
    /// </summary>
    public static class XP
    {
        public static ParserFactory<XmlNode> Node()
        {
            return P.AnyB<XmlNode>(
                () => P.Cast<XmlNode, XmlElement>(Element()),
                () => P.Cast<XmlNode, XmlText>(Text())
            );
        }

        public static ParserFactory<XmlElement> Element()
        {
            return P.AnyB<XmlElement>(
                PairElement,
                NonPairElement
            );
        }

        public static ParserFactory<XmlElement> NonPairElement()
        {
            return P.Concat<XmlElement>(
                P.Literal("<"),
                XP.ElementName(),
                XP.AttributeList(),
                P.Literal("/>")
            ).Process(p => {
                var e = new XmlElement(
                    ((Parser<string>)p.Parts[1]).Value,
                    false
                );
                e.Attributes.AddRange(
                    ((Parser<XmlAttribute[]>)p.Parts[2]).Value
                );
                return e;
            });
        }

        public static ParserFactory<XmlElement> PairElement()
        {
            return P.Concat<XmlElement>(
                P.Literal("<"),
                XP.ElementName(),
                XP.AttributeList(),
                P.Literal(">"),

                P.Repeat<XmlNode[], XmlNode>(
                    XP.Node(),
                    Quantification.Star
                ).Process(p => p.Matches.Select(m => m.Value).ToArray()),

                P.Literal("</"),
                XP.ElementName(),
                P.Literal(">")
            ).Process(p => {
                var e = new XmlElement(
                    ((Parser<string>)p.Parts[1]).Value,
                    true
                );
                e.Attributes.AddRange(
                    ((Parser<XmlAttribute[]>)p.Parts[2]).Value
                );
                e.Content.AddRange(
                    ((Parser<XmlNode[]>)p.Parts[4]).Value
                );
                string ending = ((Parser<string>)p.Parts[6]).Value;
                if (ending != e.Tag)
                    throw new XmlParsingException(
                        $"Tag <{ e.Tag }> does not end with the same name, but with </{ ending }> instead."
                    );
                return e;
            });
        }

        private static ParserFactory<XmlAttribute[]> AttributeList()
        {
            return P.Concat<XmlAttribute[]>(
                XP.Whitespace(),
                P.Repeat<XmlAttribute[], XmlAttribute>(
                    P.Concat<XmlAttribute>(
                        XP.Attribute(),
                        XP.Whitespace()
                    ).Process(p => ((Parser<XmlAttribute>)p.Parts[0]).Value),
                    Quantification.Star
                ).Process(p => p.Matches.Select(m => m.Value).ToArray())
            ).Process(p => ((Parser<XmlAttribute[]>)p.Parts[1]).Value);
        }

        private static ParserFactory<string> ElementName()
        {
            return P.StringRegex(@"[^\s</>\""]+");
        }

        private static ParserFactory<string> Whitespace()
        {
            return P.StringRegex(@"[ \n\r\t]*");
        }

        public static ParserFactory<XmlAttribute> Attribute()
        {
            return P.Concat<XmlAttribute>(
                P.StringRegex(@"[^\s=]+"),
                P.Literal("=\""),
                XP.Text(false),
                P.Literal("\"")
            ).Process(p => new XmlAttribute(
                ((Parser<string>)p.Parts[0]).Value,
                ((Parser<XmlText>)p.Parts[2]).Value
            ));
        }

        public static ParserFactory<XmlText> Text(bool inDocument = true)
        {
            // Note: When used for attribute value parsing, quotes are not allowed and empty text is allowed

            return P.Repeat<XmlText, string>(
                P.Any<string>(
                    P.StringRegex(inDocument ? "[^\\&\\<\\>]+" : "[^\\\"\\&\\<\\>]+"),
                    XP.CharacterEntity()
                ),
                inDocument ? Quantification.Plus : Quantification.Star
            ).Process(p => new XmlText(String.Concat(p.Matches.Select(m => m.Value))));
        }

        private static ParserFactory<string> CharacterEntity()
        {
            // very simple implementation only for common entities
            return P.Any<string>(
                P.Literal<string>("&amp;").Process(p => "&"),
                P.Literal<string>("&quot;").Process(p => "\""),
                P.Literal<string>("&lt;").Process(p => "<"),
                P.Literal<string>("&gt;").Process(p => ">")
            );
        }
    }
}
