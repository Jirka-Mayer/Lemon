using NUnit.Framework;
using System;
using System.Collections.Generic;
using Lemon;

namespace LemonTests
{
    [TestFixture]
    public class RegexParserTest
    {
        private RegexParser<int> parser;

        [SetUp]
        public void SetUp()
        {
            parser = P.Regex<int>("[0-9]+")
                .Process((p) => {
                    return int.Parse(p.GetMatchedString());
                })
                .CreateParser();
        }

        [TestCase]
        public void ItMatchesIntegers()
        {
            parser.Parse("42");
            Assert.AreEqual(42, parser.Value);
        }

        [TestCase]
        public void ItMatchesNumbersOnly()
        {
            parser.Parse("42asd");
            Assert.AreEqual(42, parser.Value);
        }

        [TestCase]
        public void ItFailsWithException()
        {
            parser.Parse("asd");
            Assert.False(parser.Success);
            Assert.AreEqual(0, parser.Exception.Position);
        }

        [TestCase]
        public void ItFailsFurtherDownTheInput()
        {
            parser.Parse(2, "asdbsd");
            Assert.AreEqual(2, parser.Exception.Position);
        }

        [TestCase]
        public void ItDoesNotSkipCharacters()
        {
            parser.Parse("asd42");
            Assert.False(parser.Success);
        }
    }
}
