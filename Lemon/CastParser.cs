using System;

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
            var e = base.PerformParsing(from, input);

            if (e != null)
                e.PushParser(this);

            return e;
        }
    }
}
