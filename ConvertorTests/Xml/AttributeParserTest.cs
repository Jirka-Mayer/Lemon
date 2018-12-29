using NUnit.Framework;
using System;
using Convertor.Xml;
using Lemon;

namespace ConvertorTests.Xml
{
    [TestFixture]
    public class AttributeParserTest
    {
        private Parser<XmlAttribute> parser;

        [SetUp]
        public void SetUp()
        {
            parser = XP.Attribute().CreateAbstractValuedParser();
        }

        [TestCase]
        public void ItParsesSimpleAttributes()
        {
            parser.Parse("foo=\"bar\"");
            Assert.True(parser.Success);
            Assert.AreEqual("foo", parser.Value.Name);
            Assert.AreEqual("bar", parser.Value.Value.Value);
        }

        [TestCase]
        public void ItParsesNamespacedAttributes()
        {
            parser.Parse("app:foo-asd=\"bar\"");
            Assert.True(parser.Success);
            Assert.AreEqual("app:foo-asd", parser.Value.Name);
            Assert.AreEqual("bar", parser.Value.Value.Value);
        }

        [TestCase]
        public void ItParsesEntitiesInValuePart()
        {
            parser.Parse("x=\"bar &amp; baz\"");
            Assert.True(parser.Success);
            Assert.AreEqual("bar & baz", parser.Value.Value.Value);
        }

        [TestCase]
        public void NameMustBeProvided()
        {
            parser.Parse("=\"bar\"");
            Assert.False(parser.Success);
        }

        [TestCase]
        public void ValueMustBeProvided()
        {
            parser.Parse("x=");
            Assert.False(parser.Success);
        }

        [TestCase]
        public void ValueCanBeEmpty()
        {
            parser.Parse("x=\"\"");
            Assert.True(parser.Success);
            Assert.AreEqual("", parser.Value.Value.Value);
        }
    }
}
