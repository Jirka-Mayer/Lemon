using NUnit.Framework;
using System;
using Convertor;
using Convertor.Json;
using Convertor.Xml;

namespace ConvertorTests
{
    [TestFixture]
    public class ToXmlTest
    {
        [TestCase]
        public void ItConvertsStrings()
        {
            var json = new JsonString("lorem & ipsum");
            var xml = ToXml.Convert(json).Stringify();
            Assert.AreEqual("<string>lorem &amp; ipsum</string>", xml);
        }

        [TestCase]
        public void ItConvertsBooleans()
        {
            var json = new JsonBoolean(false);
            var xml = ToXml.Convert(json).Stringify();
            Assert.AreEqual("<boolean>false</boolean>", xml);

            json = new JsonBoolean(true);
            xml = ToXml.Convert(json).Stringify();
            Assert.AreEqual("<boolean>true</boolean>", xml);
        }

        [TestCase]
        public void ItConvertsNulls()
        {
            var json = new JsonNull();
            var xml = ToXml.Convert(json).Stringify();
            Assert.AreEqual("<null/>", xml);
        }

        [TestCase]
        public void ItConvertsNumbers()
        {
            var json = new JsonNumber(42.15);
            var xml = ToXml.Convert(json).Stringify();
            Assert.AreEqual("<number>42.15</number>", xml);
        }

        [TestCase]
        public void ItConvertsArrays()
        {
            var json = new JsonArray();
            json.Items.Add(new JsonNull());
            var xml = ToXml.Convert(json).Stringify();
            Assert.AreEqual("<array><null/></array>", xml);
        }

        [TestCase]
        public void ItConvertsObjects()
        {
            var json = new JsonObject();
            json.Items.Add("foo", new JsonNull());
            var xml = ToXml.Convert(json).Stringify();
            Assert.AreEqual("<object><item key=\"foo\"><null/></item></object>", xml);
        }
    }
}
