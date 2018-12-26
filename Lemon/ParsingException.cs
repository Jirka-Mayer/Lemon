using System;
using System.Collections.Generic;

namespace Lemon
{
    public class ParsingException : Exception
    {
        /// <summary>
        /// Input string on which the parser failed
        /// </summary>
        public string Input { get; private set; }

        /// <summary>
        /// Position in the input where it failed
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Stack of parser calls
        /// </summary>
        public IEnumerable<Parser> ParserStack {
            get => parserStack;
        }

        private Stack<Parser> parserStack = new Stack<Parser>();

        public ParsingException(string message, string input, int position, Parser parser) : base(message)
        {
            this.Input = input;
            this.Position = position;

            this.PushParser(parser);
        }

        public void PushParser(Parser parser)
        {
            parserStack.Push(parser);
        }
    }
}
