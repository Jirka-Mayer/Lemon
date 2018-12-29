using NUnit.Framework;
using System;
using Convertor.Xml;
using Lemon;

namespace ConvertorTests.Xml
{
    [TestFixture]
    public class TextParserTest
    {
        private Parser<XmlText> parser;

        [SetUp]
        public void SetUp()
        {
            parser = XP.Text().CreateAbstractValuedParser();
        }

        [TestCase]
        public void ItParsesSimpleString()
        {
            parser.Parse("lorem\nipsum");
            Assert.True(parser.Success);
            Assert.AreEqual("lorem\nipsum", parser.Value.Value);
        }

        [TestCase]
        public void ItParsesCharacterEntities()
        {
            parser.Parse("lorem &amp; ipsum");
            Assert.True(parser.Success);
            Assert.AreEqual("lorem & ipsum", parser.Value.Value);
        }

        [TestCase]
        public void ItFailsOnWeirdEntity()
        {
            parser.Parse("lorem & ipsum");
            Assert.True(parser.Success); // succeeds, but matches only partially
            Assert.AreEqual(6, parser.MatchedLength);
        }
    }
}
