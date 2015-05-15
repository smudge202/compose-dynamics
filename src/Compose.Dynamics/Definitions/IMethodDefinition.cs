using System;
using System.Collections.Generic;

namespace Compose.Dynamics.Definitions
{
    public interface IMethodDefinition : IFluentDefinition
    {
        Type ReturnType { get; }
        string MethodName { get; set; }
        IEnumerable<IParameterDefinition> Parameters { get; }
        VisibilityScope Scope { get; }
    }
}
