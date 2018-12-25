using System;

namespace Lemon
{
    /// <summary>
    /// Represents a parser with a return value
    /// </summary>
    /// <typeparam name="TValue">Type of the returned value</typeparam>
    public abstract class Parser<TValue> : Parser
    {
        /// <summary>
        /// Value processor method type
        /// </summary>
        /// <param name="parser">Instance of the parser</param>
        /// <returns>The processed final value</returns>
        public delegate TValue ParserProcessor(Parser<TValue> parser);

        /// <summary>
        /// The actual processed value that the parser has matched
        /// </summary>
        public TValue Value {
            get {
                if (valueNotAvailable)
                {
                    this.value = this.ProcessValue();
                    this.valueNotAvailable = false;
                }

                return this.value;
            }
        }
        private TValue value;
        private bool valueNotAvailable = true;
        
        /// <summary>
        /// Processor function used to obtain the processed value
        /// </summary>
        public ParserProcessor Processor { get; set; }

        /// <summary>
        /// Calls the processing function whenever the parser value is required for the first time
        /// </summary>
        private TValue ProcessValue()
        {
            if (Processor == null)
                throw new ArgumentNullException(
                    $"The property '{ nameof(Processor) }' has not been set, so asking for parser value makes no sense."
                );

            return Processor(this);
        }
    }
}
