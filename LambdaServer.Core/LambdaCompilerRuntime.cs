using System.Collections.Generic;

namespace LambdaServer.Core
{
    public class LambdaCompilerRuntime
    {
        public static LambdaCompilerRuntime Current = new LambdaCompilerRuntime(new [] { "System.dll", "System.Core.dll", "LambdaServer.Core.dll"});

        public LambdaCompilerRuntime(IEnumerable<string> referencedAssemblies)
        {
            ReferencedAssemblies = referencedAssemblies;
        }

        public IEnumerable<string> ReferencedAssemblies { get; } 
    }
}