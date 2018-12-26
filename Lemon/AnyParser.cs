using System;

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

            // if no parser succeeds, the last one is to blame
            Parts[Parts.Length - 1].Exception.PushParser(this);
            return Parts[Parts.Length - 1].Exception;
        }
    }
}
