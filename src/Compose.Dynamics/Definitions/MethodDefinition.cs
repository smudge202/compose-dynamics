using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Compose.Dynamics.Definitions
{
    internal sealed class MethodDefinition : IMethodDefinition
    {
        private readonly IParameterDefinition[] _parameterDefinition;
        private readonly Type _returnType;
        private readonly TypeDefinition _parentDefinition;

        public string MethodName { get; set; }
        public Action<ILGenerator, Type[]> MethodBody { get; set; }
        public IEnumerable<IParameterDefinition> Parameters => _parameterDefinition;
        public Type ReturnType => _returnType;
        public VisibilityScope Scope { get; set; }

        public TypeDefinition And => _parentDefinition;

        public MethodDefinition(TypeDefinition parentDefinition, VisibilityScope currentVisbilityScope, Type returnType, IParameterDefinition[] parameterDefinition)
        {
            _parentDefinition = parentDefinition;
            _returnType = returnType;
            _parameterDefinition = parameterDefinition;
            Scope = currentVisbilityScope;
        }
    }
}
