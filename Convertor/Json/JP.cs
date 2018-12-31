﻿using System;
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
                () => P.Cast<JsonEntity, JsonArray>(JP.Array()),
                () => P.Cast<JsonEntity, JsonString>(JP.String()),
                () => P.Cast<JsonEntity, JsonNumber>(JP.Number()),
                () => P.Cast<JsonEntity, JsonNull>(JP.Null()),
                () => P.Cast<JsonEntity, JsonBoolean>(JP.Boolean())
            ).Name(nameof(JP.Entity));
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
            )
            .Process(p => ((Parser<JsonObject>)p.Parts[2]).Value)
            .Name(nameof(JP.Object));
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
            })
            .Name(nameof(JP.ObjectItem));
        }

        public static ParserFactory<JsonArray> Array()
        {
            return P.Concat<JsonArray>(
                P.Literal("["),
                JP.Whitespace(),

                P.Repeat<JsonArray, JsonEntity>(
                    P.Concat<JsonEntity>(
                        JP.Entity(),
                        JP.Whitespace(),
                        P.Optional<P.Void>(
                            P.Literal<P.Void>(",")
                        ),
                        JP.Whitespace()
                    ).Process(p => ((Parser<JsonEntity>)p.Parts[0]).Value),
                    Quantification.Star
                ).Process(p => {
                    var a = new JsonArray();
                    a.Items.AddRange(p.Matches.Select(m => m.Value));
                    return a;
                }),

                P.Literal("]")
            )
            .Process(p => ((Parser<JsonArray>)p.Parts[2]).Value)
            .Name(nameof(JP.Array));
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
            })
            .Name(nameof(JP.String));
        }

        private static ParserFactory<string> EscapedStringCharacter()
        {
            return P.Concat<string>(
                P.Literal("\\"),
                P.StringRegex(@"[\""\\nrt/]|(u[0-9A-Fa-f]{4})")
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
                    case '/': return "/";
                    case 'n': return "\n";
                    case 'r': return "\r";
                    case 't': return "\t";
                }
                
                throw new JsonParsingException(
                    $"Unknown escaped character '{ p.Value }'"
                );
            }).Name(nameof(JP.EscapedStringCharacter));
        }

        public static ParserFactory<JsonNumber> Number()
        {
            return P.Regex<JsonNumber>(
                @"(-?)(0|([1-9]\d*))(\.\d+)?([eE][+-]?\d+)?"
            ).Process(p => new JsonNumber(
                Double.Parse(p.GetMatchedString())
            )).Name(nameof(JP.Number));
        }

        public static ParserFactory<JsonNull> Null()
        {
            return P.Literal<JsonNull>("null")
                .Process(p => new JsonNull())
                .Name(nameof(JP.Null));
        }

        public static ParserFactory<JsonBoolean> Boolean()
        {
            return P.Any<JsonBoolean>(
                P.Literal<JsonBoolean>("true").Process(p => new JsonBoolean(true)),
                P.Literal<JsonBoolean>("false").Process(p => new JsonBoolean(false))
            ).Name(nameof(JP.Boolean));
        }

        public static ParserFactory<string> Whitespace()
        {
            return P.StringRegex(@"[ \t\n\r]*").Name(nameof(JP.Whitespace));
        }
    }
}
