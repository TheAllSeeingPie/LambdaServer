using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CSharp;

namespace LambdaServer.Core
{
    public class LambdaCompiler
    {
        public void Compile(string name, string lambda)
        {
            var source =
                $@"
                using LambdaServer.Core;
                using System;
                using System.Linq.Expressions;
                namespace {name}
                {{
                    public class {name}
                    {{
                        public LambdaContainer Container = new LambdaContainer(Expressions.Create({lambda}));
                    }}
                }}
            ";

            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof (object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof (LambdaExpression).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location)
            };
            var compilation = CSharpCompilation.Create(name, new[] {syntaxTree}, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly.Load(ms.ToArray());
                }
            }

            var type = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.FullName.StartsWith(name)).GetType($"{name}.{name}");
            var instance = Activator.CreateInstance(type);
            var containerMember = type.GetField("Container");
            var container = (LambdaContainer) containerMember.GetValue(instance);

            LambdaRegistry.Add(name, container);
        }
    }

    public class LambdaRegistry
    {
        private static readonly ConcurrentDictionary<string, LambdaContainer> _dictionary = new ConcurrentDictionary<string, LambdaContainer>();

        public static IReadOnlyDictionary<string, LambdaContainer> Current
        {
            get { return _dictionary; }
        }

        public static void Add(string name, LambdaContainer container)
        {
            _dictionary.TryAdd(name, container);
        }
    }
}