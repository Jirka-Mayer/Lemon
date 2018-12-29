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
            // use parser builders (not factories) to help prevent infinite recursion
            return P.AnyB<JsonEntity>(
                () => P.Cast<JsonEntity, JsonObject>(JP.Object()),
                () => P.Cast<JsonEntity, JsonString>(JP.String()),
                () => P.Cast<JsonEntity, JsonNull>(JP.Null())
            );
        }

        public static ParserFactory<JsonObject> Object()
        {
            return P.Concat<JsonObject>(
                P.Literal("{"),
                JP.Whitespace(),

                P.Repeat<JsonObject, KeyValuePair<string, JsonEntity>>(
                    JP.ObjectItem(),
                    Quantification.Star
                ).Process(p => {
                    var o = new JsonObject();
                    foreach (Parser<KeyValuePair<string, JsonEntity>> i in p.Matches)
                        o.Items.Add(i.Value.Key, i.Value.Value);
                    return o;
                }),

                JP.Whitespace(),
                P.Literal("}")
            ).Process(p => ((Parser<JsonObject>)p.Parts[2]).Value);
        }

        private static ParserFactory<KeyValuePair<string, JsonEntity>> ObjectItem()
        {
            // prevent infinite recursion by passing builders instead of factories
            return P.ConcatB<KeyValuePair<string, JsonEntity>>(
                JP.Whitespace,
                JP.String,
                JP.Whitespace,
                () => P.Literal(":"),
                JP.Whitespace,
                JP.Entity,
                JP.Whitespace,
                () => P.Optional<P.Void>(
                    P.Concat<P.Void>(
                        P.Literal(","),
                        JP.Whitespace()
                    )
                ),
                JP.Whitespace
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
                        JP.EscapedStringCharacter()
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

        public static ParserFactory<JsonNull> Null()
        {
            return P.Literal<JsonNull>("null")
                .Process(p => new JsonNull());
        }

        public static ParserFactory<string> Whitespace()
        {
            return P.StringRegex(@"[ \t\n\r]*");
        }
    }
}
