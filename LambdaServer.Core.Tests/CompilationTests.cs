using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LambdaServer.Core.Tests
{
    [TestClass]
    public class CompilationTests
    {
        [TestMethod]
        public void Can_a_simple_lambda_be_compiled_and_invoked()
        {
            var testName = "TestLambda";
            var compiler = new LambdaCompiler();
            compiler.Compile(testName, "()=> $\"Hello world!\"");

            var container = LambdaRegistry.Current[testName];
            Assert.AreEqual("Hello world!", container.Invoke());
        }

        [TestMethod]
        public void Can_a_lambda_with_one_argument_be_compiled_and_invoked()
        {
            var testName = "TestLambda";
            var compiler = new LambdaCompiler();
            compiler.Compile(testName, "(string arg) => $\"{arg}\"");

            var container = LambdaRegistry.Current[testName];
            Assert.AreEqual("Hello world!", container.Invoke(new KeyValuePair<string, object>("arg", "Hello world!")));
        }
    }
}