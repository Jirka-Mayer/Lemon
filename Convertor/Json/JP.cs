using System;
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
                /*P.Repeat<string>(
                    P.Any<string>(
                        P.StringRegex("[^\"\\]+"),
                        EscapedStringCharacter()
                    )
                ).Process(p => p.Occurences.JoinOrSomething()),*/
                P.Literal("\"")
            ).Process(p => {
                return null;
                //return new JsonString(p.Parts[1]);
            });
        }

        private static ParserFactory<string> EscapedStringCharacter()
        {
            return null;
        }
    }
}
