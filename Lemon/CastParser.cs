using System;
using System.Text;

namespace Lemon
{
    /// <summary>
    /// Casts a parser return value to a more general type to harness the power of polymorphism
    /// Casting happens in the processor, so do not override it
    /// </summary>
    /// <typeparam name="TTo">From which type to cast</typeparam>
    /// <typeparam name="TFrom">To which type to cast</typeparam>
    public class CastParser<TTo, TFrom> : RepeatParser<TTo, TFrom> where TFrom : TTo
    {
        public CastParser(ParserFactory<TFrom> parser) : base(parser, Quantification.Exactly(1))
        {
            // default processor
            this.Processor = p => (TTo)this.Matches[0].Value;
        }

        protected override ParsingException PerformParsing(int from, string input)
        {
            // Note: No pushing of parser to the exception because it's already done by the parent class

            return base.PerformParsing(from, input);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (Name != null)
                builder.Append(Name + ": ");

            builder.Append(
                $"Cast<{ typeof(TTo).FullName }, { typeof(TFrom).FullName }>(...)\n"
            );

            return builder.ToString();
        }
    }
}
