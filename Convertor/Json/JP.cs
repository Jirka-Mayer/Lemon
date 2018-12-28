using System;
using System.Linq;
using System.Collections.Generic;
using Lemon;

namespace Convertor.Json
{
    /// <summary>
    /// Json parsers
    /// </summary>
    public static class JP
    {
        public static ParserFactory<JsonEntity> Entity()
        {
            // TODO: this sucks, try to solve it!
            return new ParserFactory<Parser<JsonEntity>, JsonEntity>(() => {
                return new AnyParser<JsonEntity>(
                    P<JsonEntity, JsonObject>.Cast(JP.Object()),
                    P<JsonEntity, JsonString>.Cast(JP.String())
                );
            });
        }

        public static ParserFactory<JsonObject> Object()
        {
            return P.Concat<JsonObject>(
                P.Literal("{"),
                Whitespace(),

                P.Repeat<JsonObject, KeyValuePair<string, JsonEntity>>(
                    ObjectItem(),
                    Quantification.Star
                ).Process(p => {
                    var o = new JsonObject();
                    foreach (Parser<KeyValuePair<string, JsonEntity>> i in p.Matches)
                        o.Items.Add(i.Value.Key, i.Value.Value);
                    return o;
                }),

                Whitespace(),
                P.Literal("}")
            ).Process(p => ((Parser<JsonObject>)p.Parts[2]).Value);
        }

        private static ParserFactory<KeyValuePair<string, JsonEntity>> ObjectItem()
        {
            return P.Concat<KeyValuePair<string, JsonEntity>>(
                Whitespace(),
                String(),
                Whitespace(),
                P.Literal(":"),
                Whitespace(),
                Entity(),
                Whitespace(),
                
                // TODO: implement the optional parser and a P.Void struct for no return type
                P.Repeat<P.Void, P.Void>(
                    P.Concat<P.Void>(
                        P.Literal(","),
                        Whitespace()
                    ),
                    Quantification.QuestionMark
                ),

                Whitespace()
            ).Process(p => {
                return new KeyValuePair<string, JsonEntity>(
                    ((Parser<JsonString>)p.Parts[1]).Value.Value,
                    ((Parser<JsonEntity>)p.Parts[5]).Value
                );
            });
        }

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

        public static ParserFactory<string> Whitespace()
        {
            return P.StringRegex(@"[ \t\n\r]*");
        }
    }
}
