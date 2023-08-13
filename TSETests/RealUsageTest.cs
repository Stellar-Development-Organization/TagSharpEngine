using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TagSharpEngine;

namespace TSETests {
    public class RealUsageTest {
        public static void Main(string[] args) {
            var verb_var = new Verb("{hello:world}");
            Console.WriteLine("Hi");

            var finalize = $"{verb_var.Declaration}({verb_var.Parameter}):{verb_var.Payload}";
            Console.WriteLine(finalize);
        }
    }

    [TestClass]
    public class TestExample {
        [TestMethod("Basic Verb Test")]
        public void TestBasicVerb() {
            var verb_var = new Verb("{hello:world}");
            Assert.IsInstanceOfType(verb_var, typeof(Verb));

            var bare_verb = new Verb("{test}");
            Console.WriteLine(bare_verb.ToString());
            Assert.AreEqual(bare_verb.Parameter, null);
            Assert.AreEqual(bare_verb.Payload, null);
            Assert.AreEqual(bare_verb.Declaration, "test");

            var another_bare_verb = new Verb("{user(hello)}");
            Assert.AreEqual(another_bare_verb.Parameter, "hello");
            Assert.AreEqual(another_bare_verb.Payload, null);
            Assert.AreEqual(another_bare_verb.Declaration, "user");
        }
    }
}