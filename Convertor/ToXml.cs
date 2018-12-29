using System;
using System.Collections.Generic;
using Convertor.Json;
using Convertor.Xml;
using System.Linq;

namespace Convertor
{
    public static class ToXml
    {
        public static XmlElement Convert(JsonEntity entity)
        {
            if (entity is JsonString)
                return Convert((JsonString)entity);

            if (entity is JsonBoolean)
                return Convert((JsonBoolean)entity);

            if (entity is JsonNull)
                return Convert((JsonNull)entity);

            if (entity is JsonNumber)
                return Convert((JsonNumber)entity);

            if (entity is JsonArray)
                return Convert((JsonArray)entity);

            if (entity is JsonObject)
                return Convert((JsonObject)entity);

            throw new ArgumentException("Entity type is unknown.", nameof(entity));
        }

        public static XmlElement Convert(JsonString str)
        {
            var e = new XmlElement("string", true);
            e.Content.Add(new XmlText(str.Value));
            return e;
        }

        public static XmlElement Convert(JsonBoolean boolean)
        {
            var e = new XmlElement("boolean", true);
            e.Content.Add(new XmlText(boolean.Stringify()));
            return e;
        }

        public static XmlElement Convert(JsonNull n)
        {
            return new XmlElement("null", false);
        }

        public static XmlElement Convert(JsonNumber num)
        {
            var e = new XmlElement("number", true);
            e.Content.Add(new XmlText(num.Stringify()));
            return e;
        }

        public static XmlElement Convert(JsonArray array)
        {
            var e = new XmlElement("array", true);
            e.Content.AddRange(array.Items.Select(i => Convert(i)));
            return e;
        }

        public static XmlElement Convert(JsonObject obj)
        {
            var e = new XmlElement("object", true);
            foreach (KeyValuePair<string, JsonEntity> pair in obj.Items)
            {
                var item = new XmlElement("item", true);
                item.Attributes.Add(new XmlAttribute("key", new XmlText(pair.Key)));
                item.Content.Add(Convert(pair.Value));
                e.Content.Add(item);
            }
            return e;
        }
    }
}
