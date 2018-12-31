using System;

namespace Lemon
{
    /// <summary>
    /// A wrapper around a parser-creating function
    /// </summary>
    /// <typeparam name="TParser">Type of the parser returned</typeparam>
    /// <typeparam name="TValue">Return type of the parser</typeparam>
    public class ParserFactory<TParser, TValue> : ParserFactory<TValue> where TParser : Parser<TValue>
    {
        /// <summary>
        /// Parser processor delegate with specific TParser argument
        /// </summary>
        public delegate TValue SpecificParserProcessor(TParser parser);

        /// <summary>
        /// The function that actually creates the parser
        /// </summary>
        private Func<TParser> creator;

        /// <summary>
        /// The parser processor to be attached after parser creation
        /// </summary>
        private SpecificParserProcessor processor;

        /// <summary>
        /// Name of the parser
        /// </summary>
        private string name;

        /// <summary>
        /// Creates new parser factory instance
        /// </summary>
        /// <param name="parserCreator">The function that actually creates the parser</param>
        public ParserFactory(Func<TParser> parserCreator)
        {
            this.creator = parserCreator;
        }

        /// <summary>
        /// Sets a processor to be attached to the parser after creation
        /// </summary>
        /// <param name="processor">The processor function</param>
        /// <returns>Itself to be chainable</returns>
        public ParserFactory<TParser, TValue> Process(SpecificParserProcessor processor)
        {
            this.processor = processor;

            return this;
        }

        /// <summary>
        /// Sets a name to be assigned to the parser after creation
        /// </summary>
        /// <param name="nema">Name of the parser</param>
        /// <returns>Itself to be chainable</returns>
        public ParserFactory<TParser, TValue> Name(string name)
        {
            this.name = name;

            return this;
        }

        /// <summary>
        /// Creates a new parser instance
        /// </summary>
        public TParser CreateParser()
        {
            TParser parser = this.creator();

            // set the processor with a casting adapter lambda
            // if a processor was given
            if (this.processor != null)
                parser.Processor = (Parser<TValue> p) => this.processor((TParser)p);

            // set parser name
            if (name != null)
                parser.Name = name;

            return parser;
        }

        /// <summary>
        /// Creates an abstract valued parser when the factory is abstract as well
        /// </summary>
        public override Parser<TValue> CreateAbstractValuedParser()
        {
            return this.CreateParser();
        }

        /// <summary>
        /// Creates an abstract parser when the factory is abstract as well
        /// </summary>
        public override Parser CreateAbstractParser()
        {
            return this.CreateParser();
        }
    }
}
