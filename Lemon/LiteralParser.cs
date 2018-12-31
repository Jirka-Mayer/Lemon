using System;
using System.Text;

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
            for (int i = 0; i < literal.Length; i++)
            {
                if (from + i >= input.Length)
                    return new ParsingException(
                        $"Literal '{ literal }' not matching. " +
                        $"Expected character '{ literal[i] }'. Instead reached end of input.",
                        input, from + i, this
                    );

                if (input[from + i] != literal[i])
                    return new ParsingException(
                        $"Literal '{ literal }' not matching. " +
                        $"Expected character '{ literal[i] }'. Instead found '{ input[from + i] }'.",
                        input, from + i, this
                    );

                AlmostMatchedLength = i + 1;
            }

            MatchedLength = literal.Length;

            return null;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (Name != null)
                builder.Append(Name + ": ");

            builder.Append(
                $"Literal<{ typeof(TValue).FullName }>(\"{ literal }\")\n"
            );
            
            builder.Append($"    AlmostMatchedLength: { AlmostMatchedLength }\n");

            return builder.ToString();
        }
    }
}
