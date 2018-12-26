using System;

namespace Lemon
{
    /// <summary>
    /// An abstract container for any parser factory
    /// </summary>
    public abstract class ParserFactory
    {
        /// <summary>
        /// Create the parser this factory is dedicated to
        /// </summary>
        public abstract Parser CreateAbstractParser();
    }
}
