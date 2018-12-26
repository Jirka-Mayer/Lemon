using NUnit.Framework;
using System;
using Lemon;

namespace LemonTests
{
    [TestFixture]
    public class AnyParserTest
    {
        private AnyParser<string> parser;

        [SetUp]
        public void SetUp()
        {
            parser = P.Any<string>(
                P.Literal("foo"),
                P.Literal("bar")
            )
            .Process(p => p.GetMatchedString())
            .CreateParser();
        }

        [TestCase]
        public void ItMatchesFirst()
        {
            parser.Parse("foo.lorem");
            Assert.True(parser.Success);
            Assert.AreEqual(0, parser.MatchedParserIndex);
            Assert.AreEqual("foo", parser.Value);
        }

        [TestCase]
        public void ItMatchesSecond()
        {
            parser.Parse("bar.lorem");
            Assert.True(parser.Success);
            Assert.AreEqual(1, parser.MatchedParserIndex);
            Assert.AreEqual("bar", parser.Value);
        }

        [TestCase]
        public void ItDoesNotMatch()
        {
            parser.Parse("baz.lorem");
            Assert.False(parser.Success);
            Assert.AreEqual(-1, parser.MatchedParserIndex);

            Assert.AreSame(parser.Exception, parser.Parts[1].Exception);
        }
    }
}
