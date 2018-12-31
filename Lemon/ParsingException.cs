using System;
using System.Text;
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

        public Parser[] GetParserStack()
        {
            return parserStack.ToArray();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(nameof(Lemon));
            builder.Append(".");
            builder.Append(nameof(ParsingException));
            builder.Append(": ");
            builder.Append(this.Message);
            builder.Append("\n");

            builder.Append(
                "At position: " + Position +
                $" (Line: { GetLine() } Char: { GetCharacter() })"
            );

            builder.Append("\n\nParser stack:\n");

            foreach (Parser p in parserStack)
                builder.Append(p.ToString()); // newline is inside the ToString

            return builder.ToString();
        }

        public int GetLine()
        {
            int line = 1;
            
            for (int i = 0; i <= Position; i++)
            {
                if (Input[i] == '\n')
                    line++;
            }

            return line;
        }

        public int GetCharacter()
        {
            int character = 0;
            
            for (int i = 0; i <= Position; i++)
            {
                if (Input[i] == '\n')
                    character = 0;
                else
                    character++;
            }

            return character;
        }
    }
}
