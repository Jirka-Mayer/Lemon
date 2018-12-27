using NUnit.Framework;
using System;
using Convertor.Json;
using Lemon;

namespace ConvertorTests
{
    [TestFixture]
    public class StringParserTest
    {
        private Parser<JsonString> parser;

        [SetUp]
        public void SetUp()
        {
            parser = JP.String().CreateAbstractValuedParser();
        }

        [TestCase]
        public void ItParsesSimpleString()
        {
            parser.Parse(@"""lorem""");
            Assert.AreEqual("lorem", parser.Value.Value);
        }

        [TestCase]
        public void ItParsesEscapedQuotes()
        {
            parser.Parse(@"""lor\""em""");
            Assert.AreEqual("lor\"em", parser.Value.Value);
        }

        [TestCase]
        public void ItParsesEscapedNewline()
        {
            parser.Parse(@"""lor\r\nem""");
            Assert.AreEqual("lor\r\nem", parser.Value.Value);
        }

        [TestCase]
        public void ItParsesEscapedBackslash()
        {
            parser.Parse(@"""lor\\em""");
            Assert.AreEqual("lor\\em", parser.Value.Value);
        }

        [TestCase]
        public void ItParsesEscapedTab()
        {
            parser.Parse(@"""lor\tem""");
            Assert.AreEqual("lor\tem", parser.Value.Value);
        }

        [TestCase]
        public void ItParsesEscapedUnicode()
        {
            parser.Parse(@"""lor\u0000em""");
            Assert.AreEqual("lor\0em", parser.Value.Value);
        }

        [TestCase]
        public void ItParsesOtherEscapedUnicode()
        {
            parser.Parse(@"""lor\u0F05em""");
            Assert.AreEqual("lor\x0F05em", parser.Value.Value);
        }
    }
}
