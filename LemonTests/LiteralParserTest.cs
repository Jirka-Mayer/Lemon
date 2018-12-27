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

        [TestCase]
        public void ItDoesNotMatchWhenHittingEos()
        {
            Parser parser = P.Literal("asd").CreateParser();
            parser.Parse("as");
            Assert.NotNull(parser.Exception);
            Assert.AreEqual(2, parser.Exception.Position);

            parser = P.Literal("asd").CreateParser();
            parser.Parse("");
            Assert.NotNull(parser.Exception);
            Assert.AreEqual(0, parser.Exception.Position);
        }
    }
}
