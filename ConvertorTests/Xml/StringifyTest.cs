using NUnit.Framework;
using System;
using Convertor.Xml;

namespace ConvertorTests.Xml
{
    [TestFixture]
    public class StringifyTest
    {
        [TestCase]
        public void ItSerializesStrings()
        {
            var s = new XmlText("lorem");
            Assert.AreEqual("lorem", s.Stringify());

            s = new XmlText("lor\nem");
            Assert.AreEqual("lor\nem", s.Stringify());

            s = new XmlText("lorem & ipsum");
            Assert.AreEqual("lorem &amp; ipsum", s.Stringify());

            s = new XmlText("lorem < ipsum");
            Assert.AreEqual("lorem &lt; ipsum", s.Stringify());

            s = new XmlText("lorem > ipsum");
            Assert.AreEqual("lorem &gt; ipsum", s.Stringify());

            s = new XmlText("lorem \" ipsum");
            Assert.AreEqual("lorem &quot; ipsum", s.Stringify());
        }

        [TestCase]
        public void ItSerializesAttributes()
        {
            var a = new XmlAttribute("foo", new XmlText("ipsum & bar"));
            Assert.AreEqual("foo=\"ipsum &amp; bar\"", a.Stringify());
        }
    }
}
