using System;
using Convertor.Json;
using Convertor.Xml;
using System.Linq;

namespace Convertor
{
    /// <summary>
    /// Converts Xml AST into Json AST
    /// </summary>
    public static class ToJson
    {
        public static JsonEntity Convert(XmlNode node)
        {
            if (node is XmlElement)
                return Convert((XmlElement)node);
            
            if (node is XmlText)
                return Convert((XmlText)node);

            throw new ArgumentException("Node has unknown type.", nameof(node));
        }

        public static JsonObject Convert(XmlElement element)
        {
            var o = new JsonObject();

            o.Items.Add("tag", new JsonString(element.Tag));
            
            var attributeObject = new JsonObject();
            foreach (XmlAttribute attr in element.Attributes)
                attributeObject.Items.Add(attr.Name, new JsonString(attr.Value.Value));
            o.Items.Add("attributes", attributeObject);
            
            o.Items.Add("pair", new JsonBoolean(element.Pair));

            var contentArray = new JsonArray();
            contentArray.Items.AddRange(element.Content.Select(i => Convert(i)));
            o.Items.Add("content", contentArray);

            return o;
        }

        public static JsonString Convert(XmlText text)
        {
            return new JsonString(text.Value);
        }
    }
}
