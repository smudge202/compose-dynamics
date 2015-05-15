using System.Collections.Generic;

namespace Compose.Dynamics.Definitions
{
    internal sealed class ConstructorDefinition : IConstructorDefinition
    {
        private readonly TypeDefinition _parentDefinition;
        private readonly VisibilityScope _visibilityScope;
        private readonly List<IParameterDefinition> _parameters = new List<IParameterDefinition>();

        public VisibilityScope Scope => _visibilityScope;
        public TypeDefinition And => _parentDefinition;
        public List<IParameterDefinition> Parameters => _parameters;

        internal ConstructorDefinition(TypeDefinition parentDefintion, VisibilityScope visibilityScope)
        {
            _parentDefinition = parentDefintion;
            _visibilityScope = visibilityScope;
        }
    }
}
