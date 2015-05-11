using System.Collections.Generic;

namespace Compose.Dynamics.Definitions
{
    public interface IConstructorDefinition : IFluentDefinition
    {
        VisibilityScope Scope { get; }
        List<IParameterDefinition> Parameters { get; }
    }
}
