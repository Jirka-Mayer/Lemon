using NUnit.Framework;
using System;
using Lemon;

namespace LemonTests
{
    [TestFixture]
    public class ExampleClassTest
    {
        [TestCase]
        public void TestIt()
        {
            Assert.AreEqual(42, ExampleClass.ReturnFoo());
        }
    }
}
