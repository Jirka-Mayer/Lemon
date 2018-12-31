using System;
using System.Text;

namespace Lemon
{
    /// <summary>
    /// Parser that may match the inner parser or not
    /// </summary>
    /// <typeparam name="TValue">Return type of the inner and the entire parser</typeparam>
    public class OptionalParser<TValue> : Parser<TValue>
    {
        private ParserFactory<TValue> factory;

        /// <summary>
        /// Instance of the inner parser
        /// </summary>
        public Parser<TValue> InnerParser { get; private set; }

        /// <summary>
        /// True if the parser inner has succeded with matching
        /// </summary>
        public bool Matched => InnerParser.Success;

        /// <summary>
        /// Value of the inner parser if it matched succesfully
        /// </summary>
        public TValue MatchedValue => InnerParser.Value;

        public OptionalParser(ParserFactory<TValue> parser, TValue defaultValue = default(TValue))
        {
            factory = parser;

            // default processor
            this.Processor = p => Matched ? InnerParser.Value : defaultValue;
        }

        protected override ParsingException PerformParsing(int from, string input)
        {
            InnerParser = factory.CreateAbstractValuedParser();

            InnerParser.Parse(from, input);

            if (InnerParser.Success)
                MatchedLength = InnerParser.MatchedLength;
            else
                MatchedLength = 0;

            AlmostMatchedLength = InnerParser.AlmostMatchedLength;

            return null;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (Name != null)
                builder.Append(Name + ": ");

            builder.Append($"Optional<{ typeof(TValue).FullName }>(...)\n");
            builder.Append($"    Matched: { this.Matched }\n");

            return builder.ToString();
        }
    }
}
