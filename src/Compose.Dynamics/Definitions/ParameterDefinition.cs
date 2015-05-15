using System.Reflection;

namespace Compose.Dynamics.Definitions
{
    internal sealed class ParameterDefinition : IParameterDefinition
    {
        private readonly string _parameterName;
        private readonly TypeInfo _parameterType;

        public string ParameterName => _parameterName;
        public TypeInfo ParameterType => _parameterType;

        internal ParameterDefinition(string parameterName, TypeInfo parameterType)
        {
            _parameterName = parameterName;
            _parameterType = parameterType;
        }
    }
}
