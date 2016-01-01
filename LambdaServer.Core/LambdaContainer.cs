using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CSharp;

namespace LambdaServer.Core
{
    /// <summary>
    /// The base class for executing provided LambdaExpressions
    /// </summary>
    public class LambdaContainer
    {
        /// <summary>
        /// Constructs an instance of the class using the supplied <paramref name="expression"/>
        /// </summary>
        /// <param name="expression"></param>
        public LambdaContainer(LambdaExpression expression)
        {
            Expression = expression;
            CompiledDelegate = expression.Compile();
        }

        private LambdaExpression Expression { get; }
        private Delegate CompiledDelegate { get; }

        /// <exception cref="ArgumentException"><see cref="ArgumentException"/> will be thrown if supplied parameters do not match required parameters</exception>
        /// <exception cref="TargetInvocationException">The method represented by the delegate is an instance method and the target object is null.-or- One of the encapsulated methods throws an exception. </exception>
        /// <exception cref="MemberAccessException">The caller does not have access to the method represented by the delegate (for example, if the method is private).-or- The number, order, or type of parameters listed in <paramref name="args" /> is invalid. </exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public object Invoke(params KeyValuePair<string, object>[] parameters)
        {
            if (Expression.Parameters.Count != parameters.Length)
                throw new ArgumentException($"Incorrect number of arguments supplied for expression. Was expecting {Expression.Parameters.Count} but instead recevied {parameters.Length}");

            var @params = Expression.Parameters.Select(p => p.Name).Select(parameter => parameters.Single(p => p.Key.Equals(parameter, StringComparison.OrdinalIgnoreCase)).Value);

            return CompiledDelegate.DynamicInvoke(@params.ToArray());
        }
    }

    public class LambdaCompilerRuntime
    {
        public static LambdaCompilerRuntime Current = new LambdaCompilerRuntime(new [] { "System.dll", "System.Core.dll", "LambdaServer.Core.dll"});

        public LambdaCompilerRuntime(IEnumerable<string> referencedAssemblies)
        {
            ReferencedAssemblies = referencedAssemblies;
        }

        public IEnumerable<string> ReferencedAssemblies { get; } 
    }

    public class LambdaCompiler
    {
        public CompilerResults Compile(string name, string lambda)
        {
            var provider = new CSharpCodeProvider();
            var options = new CompilerParameters();

            foreach (var assembly in LambdaCompilerRuntime.Current.ReferencedAssemblies)
            {
                options.ReferencedAssemblies.Add(assembly);
            }

            options.OutputAssembly = $"{name}.dll";
            options.GenerateInMemory = true;

            var source =
                $@"
                using LambdaServer.Core;
                namespace {name}
                {{
                    public class {name}
                    {{
                        public LambdaContainer Container = new LambdaContainer(Expressions.Create({lambda}));
                    }}
                }}
            ";

            var result = provider.CompileAssemblyFromSource(options, source);

            return result;
        }
    }
}