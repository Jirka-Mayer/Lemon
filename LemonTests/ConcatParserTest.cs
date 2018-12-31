using NUnit.Framework;
using System;
using Lemon;

namespace LemonTests
{
    [TestFixture]
    public class ConcatParserTest
    {
        private ConcatParser<double> parser;

        [SetUp]
        public void SetUp()
        {
            parser = P.Concat<double>(
                P.Regex<double>(@"\d+").Process(p => double.Parse(p.GetMatchedString())),
                P.Literal("."),
                P.Regex<double>(@"\d").Process(p => double.Parse(p.GetMatchedString()))
            )
            .Process((p) => ((Parser<double>)p.Parts[0]).Value + ((Parser<double>)p.Parts[2]).Value / 10.0)
            .CreateParser();
        }

        [TestCase]
        public void ItMatchesDecimalNumber()
        {
            parser.Parse("42.5");
            Assert.AreEqual(42.5, parser.Value);
            Assert.AreEqual(3, parser.SuccessfulParsers);
        }

        [TestCase]
        public void ItFailsOnSeparatorMissingSomeSubParsers()
        {
            parser.Parse("42,5");
            
            Assert.False(parser.Success);
            Assert.AreEqual(2, parser.MatchedLength);
            
            Assert.AreEqual(2, parser.Exception.Position);
            Assert.AreEqual(1, parser.SuccessfulParsers);

            // parser stack
            Parser[] stack = parser.Exception.GetParserStack();
            Assert.AreEqual(parser.Parts[1], stack[1]);
            Assert.AreEqual(parser, stack[0]);

            // null last parser
            Assert.NotNull(parser.Parts[0]);
            Assert.NotNull(parser.Parts[1]);
            Assert.Null(parser.Parts[2]);

            // first part succeeded
            Assert.True(parser.Parts[0].Success);

            // second part has the same exception
            Assert.AreSame(parser.Exception, parser.Parts[1].Exception);
        }

        [TestCase]
        public void ItMatchesEmptyParsers()
        {
            ConcatParser<string> p = P.Concat<string>().CreateParser();
            p.Parse("");
            Assert.True(p.Success);
            Assert.AreEqual(0, parser.SuccessfulParsers);
        }
    }
}
