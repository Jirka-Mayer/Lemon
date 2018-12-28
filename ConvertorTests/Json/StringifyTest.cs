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
        }
    }
}
