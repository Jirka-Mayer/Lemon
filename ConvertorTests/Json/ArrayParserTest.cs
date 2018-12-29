using NUnit.Framework;
using System;
using Lemon;
using Convertor.Json;

namespace ConvertorTests.Json
{
    [TestFixture]
    public class ArrayParser
    {
        private Parser<JsonArray> parser;

        [SetUp]
        public void SetUp()
        {
            parser = JP.Array().CreateAbstractValuedParser();
        }

        [TestCase]
        public void ItParsesEmptyArray()
        {
            parser.Parse("[]");
            Assert.True(parser.Success);
            Assert.IsEmpty(parser.Value.Items);
        }

        [TestCase]
        public void ItParsesArrayWithANumber()
        {
            parser.Parse("[42]");
            Assert.True(parser.Success);
            Assert.AreEqual(42.0, ((JsonNumber)parser.Value.Items[0]).Value);
        }

        [TestCase]
        public void ItParsesArrayWithAnArray()
        {
            parser.Parse("[[]]");
            Assert.True(parser.Success);
            Assert.IsEmpty(((JsonArray)parser.Value.Items[0]).Items);
        }

        [TestCase]
        public void ItParsesArrayWithMultipleItems()
        {
            parser.Parse("[42, false, null, {}]");
            Assert.True(parser.Success);
            
            Assert.AreEqual(42.0, ((JsonNumber)parser.Value.Items[0]).Value);
            Assert.AreEqual(false, ((JsonBoolean)parser.Value.Items[1]).Value);
            Assert.AreEqual(typeof(JsonNull), parser.Value.Items[2].GetType());
            Assert.IsEmpty(((JsonObject)parser.Value.Items[3]).Items);
        }
    }
}
