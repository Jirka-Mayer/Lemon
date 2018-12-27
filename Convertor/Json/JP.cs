using System;
using System.Linq;
using Lemon;

namespace Convertor.Json
{
    /// <summary>
    /// Json parsers
    /// </summary>
    public static class JP
    {
        public static ParserFactory<JsonString> String()
        {
            return P.Concat<JsonString>(
                P.Literal("\""),
                
                P.Repeat<string, string>(
                    P.Any<string>(
                        P.StringRegex(@"[^\""\\]+"),
                        EscapedStringCharacter()
                    ),
                    Quantification.Star
                ).Process(p => System.String.Concat(p.Matches.Select(m => m.Value))),

                P.Literal("\"")
            )
            
            .Process(p => {
                return new JsonString(
                    ((Parser<string>)p.Parts[1]).Value
                );
            });
        }

        private static ParserFactory<string> EscapedStringCharacter()
        {
            return P.Concat<string>(
                P.Literal("\\"),
                P.StringRegex(@"[\""\\nrt]|(u[0-9A-Fa-f]{4})")
            ).Process(parser => {
                Parser<string> p = (Parser<string>)parser.Parts[1];

                if (p.Value[0] == 'u')
                {
                    if (p.Value.Length != 5)
                        throw new JsonParsingException(
                            $"Unknown escaped character '{ p.Value }'"
                        );

                    return char.ConvertFromUtf32(
                        int.Parse(
                            p.Value.Substring(1, 4),
                            System.Globalization.NumberStyles.HexNumber
                        )
                    );
                }

                if (p.Value.Length != 1)
                    throw new JsonParsingException(
                        $"Unknown escaped character '{ p.Value }'"
                    );

                switch (p.Value[0])
                {
                    case '"': return "\"";
                    case '\\': return "\\";
                    case 'n': return "\n";
                    case 'r': return "\r";
                    case 't': return "\t";
                }
                
                throw new JsonParsingException(
                    $"Unknown escaped character '{ p.Value }'"
                );
            });
        }
    }
}
