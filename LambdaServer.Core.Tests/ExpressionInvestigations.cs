using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LambdaServer.Core.Tests
{
    [TestClass]
    public class ExpressionInvestigations
    {
        [TestMethod]
        public void Can_Lambdas_be_created_into_a_variable_of_Type_Expression1()
        {
            LambdaExpression expression = Expressions.Create(()=> "Hello world!" );
            var container = new LambdaContainer(expression);

            Assert.AreEqual("Hello world!", container.Invoke());
        }

        [TestMethod]
        public void Can_Lambdas_be_created_into_a_variable_of_Type_Expression2()
        {
            LambdaExpression expression = Expressions.Create((string arg1) => arg1);
            var container = new LambdaContainer(expression);

            Assert.AreEqual("Hello world!", container.Invoke(new KeyValuePair<string, object>("ARG1", "Hello world!")));
        }

        [TestMethod]
        public void Can_Lambdas_be_created_into_a_variable_of_Type_Expression2_with_interpolation()
        {
            LambdaExpression expression = Expressions.Create((string arg1) => $"{arg1}");
            var container = new LambdaContainer(expression);

            Assert.AreEqual("Hello world!", container.Invoke(new KeyValuePair<string, object>("ARG1", "Hello world!")));
        }

        [TestMethod]
        public void Can_Lambdas_be_created_into_a_variable_of_Type_Expression3()
        {
            LambdaExpression expression = Expressions.Create((string arg1, string arg2) => $"{arg1} {arg2}");
            var container = new LambdaContainer(expression);

            Assert.AreEqual("Hello world!", container.Invoke(new KeyValuePair<string, object>("ARG1", "Hello"), new KeyValuePair<string, object>("aRg2", "world!")));
        }

        [TestMethod]
        public void Attempt_to_use_type_expressions()
        {
            LambdaExpression expression = Expressions.Create((string arg1, string arg2) => $"{arg1} {arg2}");
            var container = new LambdaContainer(expression);

            Assert.AreEqual("Hello world!", container.Invoke(new KeyValuePair<string, object>("ARG1", "Hello"), new KeyValuePair<string, object>("aRg2", "world!")));
        }
    }
}
