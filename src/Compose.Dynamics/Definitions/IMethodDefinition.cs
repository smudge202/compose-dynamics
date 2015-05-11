﻿using System;
using System.Collections.Generic;

namespace Compose.Dynamics.Definitions
{
    public interface IMethodDefinition : IFluentDefinition
    {
        Type ReturnType { get; }
        string MethodName { get; }
        IEnumerable<IParameterDefinition> Parameters { get; }
        VisibilityScope Scope { get; }
    }
}
