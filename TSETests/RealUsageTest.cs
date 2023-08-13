using System;

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
}