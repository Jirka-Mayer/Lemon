using NUnit.Framework;
using System;
using Convertor;
using Convertor.Json;
using Convertor.Xml;

namespace ConvertorTests
{
    [TestFixture]
    public class ToJsonTest
    {
        [TestCase]
        public void ItConvertsXmlText()
        {
            var text = new XmlText("lorem & ipsum");
            var json = ToJson.Convert(text).Stringify();
            Assert.AreEqual("\"lorem & ipsum\"", json);
        }

        [TestCase]
        public void ItConvertsXmlElement()
        {
            var element = new XmlElement("foo", true);
            element.Attributes.Add(new XmlAttribute("lorem", new XmlText("ipsum")));
            element.Content.Add(new XmlText("dolor"));
            var json = ToJson.Convert(element).Stringify();
            Assert.AreEqual(
                @"{""tag"": ""foo"", ""attributes"": {""lorem"": ""ipsum""}, ""pair"": true, ""content"": [""dolor""]}",
                json
            );
        }

        [TestCase]
        public void ItConvertsXmlNonPairElement()
        {
            var element = new XmlElement("foo", false);
            element.Attributes.Add(new XmlAttribute("lorem", new XmlText("ipsum")));
            var json = ToJson.Convert(element).Stringify();
            Assert.AreEqual(
                @"{""tag"": ""foo"", ""attributes"": {""lorem"": ""ipsum""}, ""pair"": false, ""content"": []}",
                json
            );
        }
    }
}
