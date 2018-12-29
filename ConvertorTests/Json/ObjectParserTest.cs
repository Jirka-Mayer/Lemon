using NUnit.Framework;
using System;
using Lemon;
using Convertor.Json;
using System.Linq;

namespace ConvertorTests.Json
{
    [TestFixture]
    public class ObjectParserTest
    {
        private Parser<JsonObject> parser;

        [SetUp]
        public void SetUp()
        {
            parser = JP.Object().CreateAbstractValuedParser();
        }

        [TestCase]
        public void ItParsesSimpleObject()
        {
            parser.Parse(@"{""foo"": ""bar""}");
            Assert.True(parser.Success);
            Assert.AreEqual("foo", parser.Value.Items.Keys.First());
            Assert.AreEqual("bar", ((JsonString)parser.Value.Items["foo"]).Value);
        }

        [TestCase]
        public void ItParsesNestedObjects()
        {
            parser.Parse(@"{""foo"": {""bar"": ""baz""}}");
            Assert.True(parser.Success);
            Assert.AreEqual("baz", ((JsonString)((JsonObject)parser.Value.Items["foo"]).Items["bar"]).Value);
        }

        [TestCase]
        public void ItFailsOnMissingColon()
        {
            parser.Parse(@"{""foo"" {""bar"": ""baz""}}");
            Assert.False(parser.Success);
        }

        [TestCase]
        public void ItSucceedsOnTrailingComma()
        {
            // WHY: because the comma is optional after an object item
            parser.Parse(@"{""foo"": {""bar"": ""baz""}, }");
            Assert.True(parser.Success);
        }

        [TestCase]
        public void ItFailsOnMissingClosingBrace()
        {
            parser.Parse(@"{""foo"": {""bar"": ""baz""}");
            Assert.False(parser.Success);
        }

        [TestCase]
        public void ItSucceedsOnStuffAfterObject()
        {
            // WHY: because the comma is optional after an object item
            parser.Parse(@"{""foo"": {""bar"": ""baz""}}   lorem  ");
            Assert.True(parser.Success);
        }

        [TestCase]
        public void ItParsesNull()
        {
            var parser = JP.Null().CreateAbstractValuedParser();
            parser.Parse("null");
            Assert.True(parser.Success);
            Assert.AreEqual(typeof(JsonNull), parser.Value.GetType());

            parser = JP.Null().CreateAbstractValuedParser();
            parser.Parse("foo");
            Assert.False(parser.Success);
        }

        [TestCase]
        public void ItParsesBoolean()
        {
            var parser = JP.Boolean().CreateAbstractValuedParser();
            parser.Parse("true");
            Assert.True(parser.Success);
            Assert.True(parser.Value.Value);

            parser = JP.Boolean().CreateAbstractValuedParser();
            parser.Parse("false");
            Assert.True(parser.Success);
            Assert.False(parser.Value.Value);

            parser = JP.Boolean().CreateAbstractValuedParser();
            parser.Parse("bar");
            Assert.False(parser.Success);
        }
    }
}
