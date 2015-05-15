using System.Reflection;

namespace Compose.Dynamics.Definitions
{
    public interface IParameterDefinition
    {
        string ParameterName { get; }
        TypeInfo ParameterType { get; }
    }
}
