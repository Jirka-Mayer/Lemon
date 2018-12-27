using System;
using System.Collections.Generic;

namespace Lemon
{
    /// <summary>
    /// Matches a given parser for as long as it's successful
    /// </summary>
    /// <typeparam name="TValue">Return type of the whole composite parser</typeparam>
    /// <typeparam name="TSubValue">Return type of the internal parser that's being repeated</typeparam>
    public class RepeatParser<TValue, TSubValue> : Parser<TValue>
    {
        private ParserFactory<TSubValue> factory;
        private Quantification quantification;
        
        /// <summary>
        /// How many times the parser matched
        /// </summary>
        public int MatchCount => Matches.Count;

        /// <summary>
        /// List of sub-parsers that matched successfuly in order
        /// </summary>
        public List<Parser<TSubValue>> Matches { get; private set; }

        public RepeatParser(ParserFactory<TSubValue> part, Quantification quantification)
        {
            factory = part;
            this.quantification = quantification;

            quantification.Validate();
        }

        protected override ParsingException PerformParsing(int from, string input)
        {
            Matches = new List<Parser<TSubValue>>();
            MatchedLength = 0;

            Parser<TSubValue> p = null;

            // while below upper limit
            while (quantification.max == -1 || MatchCount < quantification.max)
            {
                p = factory.CreateAbstractValuedParser();
                
                p.Parse(from + MatchedLength, input);

                if (!p.Success)
                    break;

                Matches.Add(p);
                MatchedLength += p.MatchedLength;
            }

            // not enough matches
            if (MatchCount < quantification.min)
            {
                if (p == null)
                    throw new NullReferenceException(
                        $"Parser has gotten into unexpected state. Variable { nameof(p) } is null. " +
                        "Check value of the quantifier if it's valid."
                    );

                p.Exception.PushParser(this);
                return p.Exception;
            }

            return null;
        }
    }

    /// <summary>
    /// How many times to repeat a parser
    /// </summary>
    public struct Quantification
    {
        /// <summary>
        /// Repeat as many times as possible, "0-times" is considered ok
        /// </summary>
        public static Quantification Star = new Quantification(0, -1);

        /// <summary>
        /// Repeat as many times as possible, but at least once
        /// </summary>
        public static Quantification Plus = new Quantification(1, -1);

        /// <summary>
        /// Repeat zero or once
        /// </summary>
        public static Quantification QuestionMark = new Quantification(0, 1);

        public int min;
        public int max;

        public Quantification(int min, int max)
        {
            this.min = min;
            this.max = max;

            this.Validate();
        }

        /// <summary>
        /// Match at least N times
        /// </summary>
        public static Quantification AtLeast(int count)
        {
            return new Quantification(count, -1);
        }

        /// <summary>
        /// Match exactly N times
        /// </summary>
        public static Quantification Exactly(int count)
        {
            return new Quantification(count, count);
        }

        /// <summary>
        /// Validates the values and throws an exception if not correct
        /// </summary>
        public void Validate()
        {
            if (min < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(min),
                    $"Minimum has to be non-negative. Quantifier: { this.ToString() }."
                );

            if (max != -1 && max < min)
                throw new ArgumentOutOfRangeException(
                    nameof(max),
                    $"Maximum has to be at lest minimum. Quantifier: { this.ToString() }."
                );
        }

        public override string ToString()
        {
            return "{" + min + "," + (max == -1 ? "" : max.ToString()) + "}";
        }
    }
}
