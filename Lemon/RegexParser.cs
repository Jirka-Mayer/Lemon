using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Lemon
{
    /// <summary>
    /// Tries to match a regular expression
    /// </summary>
    public class RegexParser<TValue> : Parser<TValue>
    {
        private string pattern;
        private RegexOptions options;

        /// <summary>
        /// The regex match
        /// </summary>
        public Match Match {
            get {
                if (this.IsPristine)
                    throw new ParserStillPristineException();

                return match;
            }
        }
        private Match match;

        // for this parser the values coincide
        public override int AlmostMatchedLength => MatchedLength;

        public RegexParser(string pattern, RegexOptions options = RegexOptions.None)
        {
            this.pattern = pattern;
            this.options = options;
        }

        protected override ParsingException PerformParsing(int from, string input)
        {
            Regex regex = new Regex(pattern, options);
            this.match = regex.Match(input, from);

            if (!match.Success)
                return new ParsingException(
                    $"Regex '{ pattern }' not matched.",
                    input, from, this
                );

            // we want the parser to match immediately even
            // if it's not explicitly said in the pattern
            if (match.Index != from)
                return new ParsingException(
                    $"Regex '{ pattern }' matched, but skipped { match.Index - from } characters.",
                    input, from, this
                );

            this.MatchedLength = match.Length;
            return null;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (Name != null)
                builder.Append(Name + ": ");

            builder.Append(
                $"Regex<{ typeof(TValue).FullName }>(@\"{ pattern }\")\n"
            );

            return builder.ToString();
        }
    }
}
