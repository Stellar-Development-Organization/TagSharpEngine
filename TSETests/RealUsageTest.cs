using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TagSharpEngine;
using TagSharpEngine.Interface;
using TagSharpEngine.Blocks;
using TagSharpEngine.Adapters;

namespace TSETests {
    public class RealUsageTest {
        public static async Task Main() {
            var verb_var = new Verb("{hello:world}");
            Console.WriteLine(verb_var.ToString());

            var verb_no_pl = new Verb("{test}");
            Console.WriteLine(verb_no_pl.ToString());

            var full_verb = new Verb("{test(param):hello}");
            Console.WriteLine(full_verb.ToString());

            IBaseBlock[] blocks = { 
                new MathBlock(), 
                new UpperBlock(), 
                new LowerBlock(), 
                new RandomBlock(),
                new StrictVarGetterBlock(),
				new IfControl(),
				new AllControl(),
            };
            
            var engine = new Interpreter(blocks.ToList());

            Dictionary<string, IAdapter> adapters = new()
            {
                { "test", new StringAdapter("Hello World") }
            };
			
			Response test = await engine.Process("{if(1<2):Yes|Go back to school}", 2000, seedVar: adapters);
            Console.WriteLine(test.Body);
        }
    }

    [TestClass]
    public class TestExample {
        [TestMethod("Basic Verb Test")]
        public void TestBasicVerb() {
            var verb_var = new Verb("{hello:world}");
            Assert.IsInstanceOfType(verb_var, typeof(Verb));

            var bare_verb = new Verb("{test}");
            Assert.AreEqual(bare_verb.Parameter, null);
            Assert.AreEqual(bare_verb.Payload, null);
            Assert.AreEqual(bare_verb.Declaration, "test");

            var another_bare_verb = new Verb("{user(hello)}");
            Assert.AreEqual(another_bare_verb.Parameter, "hello");
            Assert.AreEqual(another_bare_verb.Payload, null);
            Assert.AreEqual(another_bare_verb.Declaration, "user");
        }

        [TestMethod("Basic Interpreter with block test.")]
        public async Task InterpreterTest() {
            IBaseBlock[] blocks = { 
                new UpperBlock(), 
            };
            
            var engine = new Interpreter(blocks.ToList());
    
			Response test = await engine.Process("{upper(test)}", 2000);
            Assert.AreEqual("TEST", test.Body);
        }
    }
}