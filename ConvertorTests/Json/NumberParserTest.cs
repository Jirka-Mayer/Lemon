using NUnit.Framework;
using System;
using Lemon;
using Convertor.Json;

namespace ConvertorTests.Json
{
    [TestFixture]
    public class NumberParserTest
    {
        private Parser<JsonNumber> parser;

        [SetUp]
        public void SetUp()
        {
            parser = JP.Number().CreateAbstractValuedParser();
        }

        [TestCase]
        public void DoubleNumbersAreStringifiedProperly()
        {
            Assert.AreEqual("2", 2.0.ToString());
            Assert.AreEqual("2.42", 2.42.ToString());
            Assert.AreEqual("1.4E+50", 1.4e50.ToString());
        }

        [TestCase]
        public void ItParsesIntegers()
        {
            parser.Parse("42");
            Assert.True(parser.Success);
            Assert.AreEqual(42.0, parser.Value.Value);
        }

        [TestCase]
        public void ItParsesZero()
        {
            parser.Parse("0");
            Assert.True(parser.Success);
            Assert.AreEqual(0.0, parser.Value.Value);
        }

        [TestCase]
        public void ItParsesNegativeIntegers()
        {
            parser.Parse("-42");
            Assert.True(parser.Success);
            Assert.AreEqual(-42.0, parser.Value.Value);
        }

        [TestCase]
        public void NumberCannotStartWithZeros()
        {
            // succeeds but parses only the leading zero
            parser.Parse("042");
            Assert.True(parser.Success);
            Assert.AreEqual(1, parser.MatchedLength);
        }

        [TestCase]
        public void ItParsesDecimals()
        {
            parser.Parse("1.42");
            Assert.True(parser.Success);
            Assert.AreEqual(1.42, parser.Value.Value);
        }

        [TestCase]
        public void ItParsesDecimalsWithExponent()
        {
            parser.Parse("1.42e-50");
            Assert.True(parser.Success);
            Assert.AreEqual(1.42e-50, parser.Value.Value);
        }

        [TestCase]
        public void ItParsesDecimalsWithPositiveExponent()
        {
            parser.Parse("1.42E50");
            Assert.True(parser.Success);
            Assert.AreEqual(1.42e50, parser.Value.Value);
        }
    }
}
