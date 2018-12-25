using NUnit.Framework;
using System;
using Lemon;

namespace LemonTests
{
    [TestFixture]
    public class LiteralParserTest
    {
        [TestCase]
        public void ItTogglesPritineness()
        {
            Parser parser = P.Literal("foo").CreateParser();
            Assert.True(parser.IsPristine);
            parser.Parse("foobar");
            Assert.False(parser.IsPristine);
        }

        [TestCase]
        public void ItMatchesStringLiterally()
        {
            Parser parser = P.Literal("foo").CreateParser();
            parser.Parse("foobar");
            Assert.True(parser.Success);
        }

        [TestCase]
        public void ItProducesParsingException()
        {
            Parser parser = P.Literal("foo").CreateParser();
            parser.Parse("asd");
            Assert.NotNull(parser.Exception);
        }
    }
}
