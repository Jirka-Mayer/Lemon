using NUnit.Framework;
using System;
using Convertor.Json;

namespace ConvertorTests
{
    [TestFixture]
    public class StringParserTest
    {
        [TestCase]
        public void ProjectsWorkTogeter()
        {
            Assert.AreEqual(42, JP.String());
        }
    }
}
