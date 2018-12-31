using System;

namespace Lemon
{
    /// <summary>
    /// A parser that can match and process some input string
    /// </summary>
    public abstract class Parser
    {
        /// <summary>
        /// Has the parser not been used yet?
        /// </summary>
        public bool IsPristine { get; private set; } = true;

        /// <summary>
        /// The input string given to the parser
        /// </summary>
        private string givenInput;

        /// <summary>
        /// The from index given to the parser
        /// </summary>
        private int givenFromIndex;

        /// <summary>
        /// Was the parsing successful?
        /// </summary>
        public bool Success {
            get {
                if (this.IsPristine)
                    throw new ParserStillPristineException();
                
                return this.Exception == null;
            }
        }

        /// <summary>
        /// Number of matched characters
        /// </summary>
        public int MatchedLength {
            get {
                if (this.IsPristine)
                    throw new ParserStillPristineException();
                
                return matchedLength;
            }
            protected set => matchedLength = value;
        }
        private int matchedLength;

        /// <summary>
        /// Number of characters that almost matched before (if at all) an exception was thrown
        /// </summary>
        public virtual int AlmostMatchedLength { get; protected set; }

        /// <summary>
        /// The exception thrown during parsing
        /// </summary>
        public ParsingException Exception { get; private set; }

        /// <summary>
        /// Name of the parser
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Try to parse the input string
        /// </summary>
        /// <param name="from">Character index of the input to start parsing from</param>
        /// <param name="input">Input string</param>
        public void Parse(int from, string input)
        {
            if (!this.IsPristine)
                throw new ParserAlreadyUsedException();

            this.IsPristine = false;
            
            this.givenInput = input;
            this.givenFromIndex = from;

            if (from < 0)
                throw new ArgumentOutOfRangeException(nameof(from), "Value has to be non negative.");

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            this.Exception = this.PerformParsing(from, input);
        }

        /// <summary>
        /// Try to parse the input string from start
        /// </summary>
        /// <param name="input">Input string</param>
        public void Parse(string input)
        {
            this.Parse(0, input);
        }

        /// <summary>
        /// Performs the actual parsing
        /// 
        /// Values required to set:
        /// - MatchedLength
        /// </summary>
        /// <param name="from">Input starting index</param>
        /// <param name="input">Input string</param>
        /// <returns>Null on success, ParsingException otherwise</returns>
        protected abstract ParsingException PerformParsing(int from, string input);

        /// <summary>
        /// Obtains the matched part of the input string
        /// </summary>
        public string GetMatchedString()
        {
            if (this.IsPristine)
                throw new ParserStillPristineException();

            return this.givenInput.Substring(this.givenFromIndex, this.MatchedLength);
        }
    }
}
