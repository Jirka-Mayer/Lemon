using System;

namespace Lemon
{
    /// <summary>
    /// A wrapper around a parser-creating function
    /// </summary>
    /// <typeparam name="TParser">Type of the parser returned</typeparam>
    /// <typeparam name="TValue">Return type of the parser</typeparam>
    public class ParserFactory<TParser, TValue> where TParser : Parser<TValue>
    {
        /// <summary>
        /// The function that actually creates the parser
        /// </summary>
        private Func<TParser> creator;

        /// <summary>
        /// The parser processor to be attached after parser creation
        /// </summary>
        private Parser<TValue>.ParserProcessor processor;

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
        public ParserFactory<TParser, TValue> Process(Parser<TValue>.ParserProcessor processor)
        {
            this.processor = processor;

            return this;
        }

        /// <summary>
        /// Creates a new parser instance
        /// </summary>
        public TParser CreateParser()
        {
            TParser parser = this.creator();

            parser.Processor = this.processor;

            return parser;
        }
    }
}
