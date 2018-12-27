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
        /// Matches a regular expression and returns the matched string
        /// </summary>
        /// <param name="pattern">Regex pattern</param>
        /// <param name="options">Regex options</param>
        public static ParserFactory<RegexParser<string>, string> StringRegex(
            string pattern, RegexOptions options = RegexOptions.None
        )
        {
            return Regex<string>(pattern, options).Process(p => p.GetMatchedString());
        }

        /// <summary>
        /// Matches an array of parsers in succession
        /// </summary>
        /// <param name="parts">Array of sub-parsers</param>
        /// <typeparam name="TValue">Return type of the composite parser</typeparam>
        public static ParserFactory<ConcatParser<TValue>, TValue> Concat<TValue>(params ParserFactory[] parts)
        {
            return new ParserFactory<ConcatParser<TValue>, TValue>(() => {
                return new ConcatParser<TValue>(parts);
            });
        }

        /// <summary>
        /// Matches any of the provided parsers
        /// </summary>
        /// <param name="parts">Array of sub-parsers</param>
        /// <typeparam name="TValue">Return type of the composite parser</typeparam>
        public static ParserFactory<AnyParser<TValue>, TValue> Any<TValue>(params ParserFactory<TValue>[] parts)
        {
            // Note: AnyParser has processor already attached
            
            return new ParserFactory<AnyParser<TValue>, TValue>(() => {
                return new AnyParser<TValue>(parts);
            });
        }

        /// <summary>
        /// Matches a parser as many times as possible
        /// </summary>
        /// <param name="part">Parser to be repeated</param>
        /// <param name="quantification">How many times to repeat</param>
        /// <typeparam name="TValue">Return type of the whole composite parser</typeparam>
        /// <typeparam name="TSubValue">Return type of the internal parser that's being repeated</typeparam>
        public static ParserFactory<RepeatParser<TValue, TSubValue>, TValue> Repeat<TValue, TSubValue>(
            ParserFactory<TSubValue> part, Quantification quantification
        )
        {
            return new ParserFactory<RepeatParser<TValue, TSubValue>, TValue>(() => {
                return new RepeatParser<TValue, TSubValue>(part, quantification);
            });
        }
    }
}
