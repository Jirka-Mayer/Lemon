using NUnit.Framework;
using System;
using Lemon;
using Convertor.Xml;

namespace ConvertorTests.Xml
{
    [TestFixture]
    public class ElementParserTest
    {
        private Parser<XmlElement> parser;

        [SetUp]
        public void SetUp()
        {
            parser = XP.Element().CreateAbstractValuedParser();
        }

        [TestCase]
        public void ItParsesEmptyPairTag()
        {
            parser.Parse("<tag></tag>");
            Assert.True(parser.Success);
            Assert.True(parser.Value.Pair);
            Assert.AreEqual("tag", parser.Value.Tag);
        }

        [TestCase]
        public void ItParsesEmptyNonPairTag()
        {
            parser.Parse("<tag/>");
            Assert.True(parser.Success);
            Assert.False(parser.Value.Pair);
            Assert.AreEqual("tag", parser.Value.Tag);
        }

        [TestCase]
        public void ItParsesTagWithText()
        {
            parser.Parse("<tag>lorem</tag>");
            Assert.True(parser.Success);
            Assert.AreEqual(1, parser.Value.Content.Count);
            Assert.AreEqual("lorem", parser.Value.Content[0].Stringify());
            Assert.AreEqual(0, parser.Value.Attributes.Count);
        }

        [TestCase]
        public void ItParsesTagWithTextAndElement()
        {
            parser.Parse("<tag>lorem<br/>ipsum</tag>");
            Assert.True(parser.Success);
            Assert.AreEqual(3, parser.Value.Content.Count);
            Assert.AreEqual("lorem", parser.Value.Content[0].Stringify());
            Assert.AreEqual("<br/>", parser.Value.Content[1].Stringify());
            Assert.AreEqual("ipsum", parser.Value.Content[2].Stringify());
        }

        [TestCase]
        public void ItParsesTagWithAttributes()
        {
            parser.Parse("<tag foo=\"bar\">lorem</tag>");
            Assert.True(parser.Success);
            Assert.AreEqual(1, parser.Value.Content.Count);
            Assert.AreEqual(1, parser.Value.Attributes.Count);
            Assert.AreEqual("foo", parser.Value.Attributes[0].Name);
        }

        [TestCase]
        public void ItParsesNonPairTagWithAttributes()
        {
            parser.Parse("<tag foo=\"bar\"/>");
            Assert.True(parser.Success);
            Assert.AreEqual(1, parser.Value.Attributes.Count);
            Assert.AreEqual("foo", parser.Value.Attributes[0].Name);
        }

        [TestCase]
        public void ItParsesTagWithSuperCharacters()
        {
            parser.Parse("<tag-name:bar \t/>");
            Assert.True(parser.Success);
            Assert.AreEqual("tag-name:bar", parser.Value.Tag);
        }
    }
}
