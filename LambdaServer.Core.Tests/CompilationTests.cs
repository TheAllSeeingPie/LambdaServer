using System;
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
            var results = compiler.Compile(testName, "()=> \"Hello world!\"");

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Console.WriteLine(assembly.FullName);
            }

            foreach (var error in results.Errors)
            {
                Console.WriteLine(error);
            }

            var type = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.FullName.StartsWith(testName)).GetType($"{testName}.{testName}");
            var instance = Activator.CreateInstance(type);
            var containerMember = type.GetField("Container");
            var container = (LambdaContainer)containerMember.GetValue(instance);
            Assert.AreEqual("Hello world!", container.Invoke());
        }
    }
}