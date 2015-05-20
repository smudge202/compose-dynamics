using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Compose.Dynamics.Definitions
{
    public interface IMethodDefinition : IFluentDefinition
    {
        Type ReturnType { get; }
        string MethodName { get; set; }
        Action<ILGenerator, int> MethodBody { get; set; }
        IEnumerable<IParameterDefinition> Parameters { get; }
        VisibilityScope Scope { get; set; }
    }
}
