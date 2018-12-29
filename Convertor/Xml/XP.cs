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
        public static ParserFactory<XmlText> Text()
        {
            return P.Repeat<XmlText, string>(
                P.Any<string>(
                    P.StringRegex("[^\\\"\\&\\<\\>]"),
                    XP.CharacterEntity()
                ),
                Quantification.Star
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
