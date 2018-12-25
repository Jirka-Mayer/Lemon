using System;

namespace Lemon
{
    /// <summary>
    /// Basic set of parser factory builders
    /// </summary>
    public static class P
    {
        public static ParserFactory<LiteralParser<string>, string> Literal(string literal)
        {
            return new ParserFactory<LiteralParser<string>, string>(() => {
                return new LiteralParser<string>(literal);
            });
        }
    }
}
