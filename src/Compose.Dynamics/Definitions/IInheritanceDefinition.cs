using System.Reflection;

namespace Compose.Dynamics.Definitions
{
    public interface IInheritanceDefinition
    {
        TypeInfo InheritedFrom { get; }
    }
}
