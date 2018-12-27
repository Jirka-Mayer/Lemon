using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using Lemon;

namespace LemonTests
{
    [TestFixture]
    public class RepeatParserTest
    {
        private RepeatParser<string, string> parser;

        [SetUp]
        public void SetUp()
        {
            parser = P.Repeat<string, string>(
                P.Literal("asd"),
                Quantification.Plus
            )
            .Process(p => String.Concat(p.Matches.Select(m => m.Value)))
            .CreateParser();
        }

        [TestCase]
        public void ItMatchesOneRepeat()
        {
            parser.Parse("asd");
            Assert.True(parser.Success);
            Assert.AreEqual(1, parser.MatchCount);

            Assert.AreEqual("asd", parser.Value);
        }

        [TestCase]
        public void ItMatchesManyRepeats()
        {
            parser.Parse("asdasdasdlorem");
            Assert.True(parser.Success);
            Assert.AreEqual(3, parser.MatchCount);
        }

        [TestCase]
        public void ItMatchesManyRepeatsEndingWithEos()
        {
            parser.Parse("asdasdasd");
            Assert.True(parser.Success);
            Assert.AreEqual(3, parser.MatchCount);

            Assert.AreEqual("asdasdasd", parser.Value);
        }

        [TestCase]
        public void ItFailsWithNoOccurences()
        {
            parser.Parse("lorem");
            Assert.False(parser.Success);
            Assert.AreEqual(0, parser.MatchCount);
        }

        [TestCase]
        public void ItFailsWithTooFewOccurences()
        {
            parser = P.Repeat<string, string>(
                P.Literal("asd"),
                Quantification.AtLeast(5)
            ).CreateParser();

            parser.Parse("asd");
            Assert.False(parser.Success);
            Assert.AreEqual(1, parser.MatchCount);
        }
    }
}
