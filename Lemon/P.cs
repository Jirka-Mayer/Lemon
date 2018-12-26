using System;
using System.Text.RegularExpressions;

namespace Lemon
{
    /// <summary>
    /// Basic set of parser factory builders
    /// </summary>
    public static class P
    {
        /// <summary>
        /// Matches a literal string and returns the string as the value
        /// </summary>
        /// <param name="literal">String to match</param>
        public static ParserFactory<LiteralParser<string>, string> Literal(string literal)
        {
            return new ParserFactory<LiteralParser<string>, string>(() => {
                return new LiteralParser<string>(literal);
            }).Process((p) => p.GetMatchedString());
        }

        /// <summary>
        /// Matches a regular expression and allows you to provide a processor to type TValue
        /// </summary>
        /// <param name="pattern">Regex patter</param>
        /// <param name="options">Regex options</param>
        /// <typeparam name="TValue">Parser value type</typeparam>
        public static ParserFactory<RegexParser<TValue>, TValue> Regex<TValue>(
            string pattern, RegexOptions options = RegexOptions.None
        )
        {
            return new ParserFactory<RegexParser<TValue>, TValue>(() => {
                return new RegexParser<TValue>(pattern, options);
            });
        }

        /// <summary>
        /// Matches a regular expression and returns the regex Match object
        /// </summary>
        /// <param name="pattern">Regex pattern</param>
        /// <param name="options">Regex options</param>
        public static ParserFactory<RegexParser<Match>, Match> Regex(
            string pattern, RegexOptions options = RegexOptions.None
        )
        {
            return new ParserFactory<RegexParser<Match>, Match>(() => {
                return new RegexParser<Match>(pattern, options);
            }).Process((p) => p.Match);
        }

        /// <summary>
        /// Matches an array of parsers in succession
        /// </summary>
        /// <param name="parsers">Array of sub-parsers</param>
        /// <typeparam name="TValue">Return type of the composite parser</typeparam>
        public static ParserFactory<ConcatParser<TValue>, TValue> Concat<TValue>(params ParserFactory[] parsers)
        {
            return new ParserFactory<ConcatParser<TValue>, TValue>(() => {
                return new ConcatParser<TValue>(parsers);
            });
        }
    }
}
