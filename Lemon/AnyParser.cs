using System;
using System.Text;
using System.Linq;

namespace Lemon
{
    /// <summary>
    /// Matches any of the provided parsers
    /// </summary>
    public class AnyParser<TValue> : Parser<TValue>
    {
        /// <summary>
        /// Array of sub-parsers
        /// Contains null if not instantiated, because the sub-parser was not needed
        /// </summary>
        public Parser<TValue>[] Parts { get; private set; }

        private ParserFactory<TValue>[] factories;

        /// <summary>
        /// Index of the parser that matched the input
        /// </summary>
        public int MatchedParserIndex { get; private set; } = -1;

        /// <summary>
        /// The parser that first matched the input
        /// </summary>
        public Parser<TValue> MatchedParser => Parts[MatchedParserIndex];

        // for this parser the values coincide
        public override int AlmostMatchedLength => MatchedLength;

        public AnyParser(params ParserFactory<TValue>[] parts)
        {
            if (parts.Length == 0)
                throw new ArgumentException("No sub-parsers provided.", nameof(parts));

            factories = parts;

            // default processor
            this.Processor = (p) => this.MatchedParser.Value;
        }

        protected override ParsingException PerformParsing(int from, string input)
        {
            Parts = new Parser<TValue>[factories.Length];

            for (int i = 0; i < factories.Length; i++)
            {
                Parser<TValue> p = factories[i].CreateAbstractValuedParser();
                Parts[i] = p;

                p.Parse(from, input);

                if (p.Success)
                {
                    MatchedParserIndex = i;
                    MatchedLength = p.MatchedLength;
                    return null;
                }
            }

            // get the parser that ALMOST matched the most charaters
            // that one was probbably closest to the truth
            int maxMatched = 0;
            int maxMatchedIndex = Parts.Length - 1;
            for (int i = 0; i < Parts.Length; i++)
            {
                if (Parts[i].AlmostMatchedLength > maxMatched)
                {
                    maxMatchedIndex = i;
                    maxMatched = Parts[i].AlmostMatchedLength;
                }
            }

            var e = Parts[maxMatchedIndex].Exception;
            e.PushParser(this);
            return e;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (Name != null)
                builder.Append(Name + ": ");

            builder.Append($"Any<{ typeof(TValue).FullName }>({ Parts.Length } parsers)\n");
            
            builder.Append("    AlmostMatchedLengths: [");
            builder.Append(String.Join(", ", Parts.Select(p => p.AlmostMatchedLength.ToString())));
            builder.Append("]\n");

            return builder.ToString();
        }
    }
}
