using NUnit.Framework;
using System;
using Convertor.Json;

namespace ConvertorTests.Json
{
    [TestFixture]
    public class StringifyTest
    {
        [TestCase]
        public void ItSerializesStrings()
        {
            var s = new JsonString("lorem");
            Assert.AreEqual(@"""lorem""", s.Stringify());

            s = new JsonString("lor\nem");
            Assert.AreEqual(@"""lor\nem""", s.Stringify());
        }

        [TestCase]
        public void ItSerializesObjects()
        {
            var o = new JsonObject();
            o.Items.Add("foo", new JsonString("bar"));

            var inner = new JsonObject();
            inner.Items.Add("asd", new JsonString("bsd"));

            o.Items.Add("inner", inner);


            Assert.AreEqual(
                @"{""foo"": ""bar"", ""inner"": {""asd"": ""bsd""}}",
                o.Stringify(new StringifyOptions())
            );

            Assert.AreEqual(
                "{\n  \"foo\": \"bar\",\n  \"inner\": {\n    \"asd\": \"bsd\"\n  }\n}",
                o.Stringify(new StringifyOptions() {
                    indentSize = 2,
                    currentIndent = 0,
                    indentCharacter = " "
                })
            );

            Assert.AreEqual(
                "{\n\t\"foo\": \"bar\",\n\t\"inner\": {\n\t\t\"asd\": \"bsd\"\n\t}\n}",
                o.Stringify(new StringifyOptions() {
                    indentSize = 1,
                    currentIndent = 0,
                    indentCharacter = "\t"
                })
            );

            Assert.AreEqual(
                "{}",
                new JsonObject().Stringify()
            );

            Assert.AreEqual(
                "{\n}",
                new JsonObject().Stringify(new StringifyOptions() {
                    indentSize = 2,
                    currentIndent = 0,
                    indentCharacter = " "
                })
            );
        }

        [TestCase]
        public void ItSerializesArrays()
        {
            var a = new JsonArray();
            a.Items.Add(new JsonNull());
            a.Items.Add(new JsonNumber(42));
            a.Items.Add(new JsonObject());
            a.Items.Add(new JsonBoolean(false));

            Assert.AreEqual(
                "[null, 42, {}, false]",
                a.Stringify(new StringifyOptions())
            );

            Assert.AreEqual(
                "[\n  null,\n  42,\n  {\n  },\n  false\n]",
                a.Stringify(new StringifyOptions() {
                    indentSize = 2,
                    currentIndent = 0,
                    indentCharacter = " "
                })
            );
        }
    }
}
