using System;
using System.Collections.Generic;

namespace Compose.Dynamics.Definitions
{
    internal sealed class MethodDefinition : IMethodDefinition
    {
        private readonly IParameterDefinition[] _parameterDefinition;
        private readonly Type _returnType;
        private readonly TypeDefinition _parentDefinition;
        private VisibilityScope _currentVisbilityScope;

        public string MethodName { get; set; }
        public IEnumerable<IParameterDefinition> Parameters => _parameterDefinition;
        public Type ReturnType => _returnType;
        public VisibilityScope Scope => _currentVisbilityScope;

        public TypeDefinition And => _parentDefinition;

        public MethodDefinition(TypeDefinition parentDefinition, VisibilityScope currentVisbilityScope, Type returnType, IParameterDefinition[] parameterDefinition)
        {
            _parameterDefinition = parameterDefinition;
            _currentVisbilityScope = currentVisbilityScope;
            _returnType = returnType;
            _parameterDefinition = parameterDefinition;
        }
    }
}
