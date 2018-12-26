using System;

namespace Lemon
{
    /// <summary>
    /// Abstract container for parser factory with parser returning a specific type
    /// </summary>
    /// <typeparam name="TValue">Return type of the parser</typeparam>
    public abstract class ParserFactory<TValue> : ParserFactory
    {
        /// <summary>
        /// Create the parser this factory is dedicated to
        /// </summary>
        public abstract Parser<TValue> CreateAbstractValuedParser();
    }
}
