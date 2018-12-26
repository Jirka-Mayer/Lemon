using System;
using System.Text.RegularExpressions;

namespace Lemon
{
    /// <summary>
    /// Basic set of parser factory builders
    /// </summary>
    public static class P
    {
        public static ParserFactory<LiteralParser<string>, string> Literal(string literal)
        {
            return new ParserFactory<LiteralParser<string>, string>(() => {
                return new LiteralParser<string>(literal);
            });
        }

        public static ParserFactory<RegexParser<TValue>, TValue> Regex<TValue>(
            string pattern, RegexOptions options = RegexOptions.None
        )
        {
            return new ParserFactory<RegexParser<TValue>, TValue>(() => {
                return new RegexParser<TValue>(pattern, options);
            });
        }
    }
}
