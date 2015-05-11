using System.Reflection;

namespace Compose.Dynamics.Definitions
{
    public interface IPropertyDefinition : IFluentDefinition
    {
        bool CanRead { get; }
        bool CanWrite { get; }
        VisibilityScope ReadScope { get; }
        VisibilityScope WriteScope { get; }
        TypeInfo PropertyType { get; }
        string PropertyName { get; }
    }
}
