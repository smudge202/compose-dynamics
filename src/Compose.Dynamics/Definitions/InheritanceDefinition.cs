using System;
using System.Reflection;

namespace Compose.Dynamics.Definitions
{
    public class InheritanceDefinition : IInheritanceDefinition
    {
        private readonly TypeInfo _inheritedFrom;
        public TypeInfo InheritedFrom => _inheritedFrom;

        internal InheritanceDefinition(Type inheritedFrom) : this(inheritedFrom.GetTypeInfo()) { }
        internal InheritanceDefinition(TypeInfo inheritedFrom)
        {
            _inheritedFrom = inheritedFrom;
        }
    }
}
