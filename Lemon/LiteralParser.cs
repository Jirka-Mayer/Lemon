using System;

namespace Lemon
{
    /// <summary>
    /// Tries to match a literal string in the input
    /// </summary>
    /// <typeparam name="TValue">
    /// Makes no sense, but a processor can be attached
    /// This parser extends valued parser simply for convenience of usage
    /// (to allow factory creation and prevent other unnecessary complications)
    /// (you can say it returns the literal string is tries to match)
    /// </typeparam>
    public class LiteralParser<TValue> : Parser<TValue>
    {
        private string literal;

        public LiteralParser(string literal)
        {
            this.literal = literal;
        }

        protected override ParsingException PerformParsing(int from, string input)
        {
            base.PerformParsing(from, input);

            for (int i = 0; i < literal.Length && from + i < input.Length; i++)
                if (input[from + i] != literal[i])
                    return new ParsingException(
                        $"Literal '{ literal }' not matching at input index { from + i }."
                    );

            this.MatchedLength = literal.Length;
            return null;
        }
    }
}
