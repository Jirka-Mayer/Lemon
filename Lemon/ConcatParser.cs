using System;
using System.Text;
using System.Linq;

namespace Lemon
{
    /// <summary>
    /// Tries to match all sub-parsers (parts) in succession
    /// </summary>
    public class ConcatParser<TValue> : Parser<TValue>
    {
        /// <summary>
        /// Array of sub-parsers to be matched
        /// Contains null if the sub-parser hasn't been instantiated (eg. on failure)
        /// </summary>
        public Parser[] Parts { get; private set; }

        private ParserFactory[] factories;

        /// <summary>
        /// How many parsers were succesfull
        /// = index of the failed parser
        /// </summary>
        public int SuccessfulParsers { get; private set; }

        public ConcatParser(params ParserFactory[] parts)
        {
            factories = parts;
        }

        protected override ParsingException PerformParsing(int from, string input)
        {
            MatchedLength = 0;
            Parts = new Parser[factories.Length];

            for (SuccessfulParsers = 0; SuccessfulParsers < factories.Length; SuccessfulParsers++)
            {
                Parser p = factories[SuccessfulParsers].CreateAbstractParser();
                Parts[SuccessfulParsers] = p;

                p.Parse(from + MatchedLength, input);

                AlmostMatchedLength += p.AlmostMatchedLength;

                if (!p.Success)
                {
                    p.Exception.PushParser(this);
                    return p.Exception;
                }

                MatchedLength += p.MatchedLength;
            }

            return null;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (Name != null)
                builder.Append(Name + ": ");

            builder.Append($"Concat<{ typeof(TValue).FullName }>({ Parts.Length } parsers)\n");
            
            builder.Append($"    AlmostMatchedLength: { AlmostMatchedLength }\n");

            builder.Append("    Succeeded: [");
            builder.Append(String.Join(", ", Parts.Select(
                p => p == null ? "?" : p.Success.ToString()
            )));
            builder.Append("]\n");

            return builder.ToString();
        }
    }
}
