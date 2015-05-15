using System.Reflection;

namespace Compose.Dynamics.Definitions
{
    internal sealed class PropertyDefinition : IPropertyDefinition
    {
        private readonly TypeInfo _propertyType;
        private readonly TypeDefinition _parentDefinition;
        private bool _canRead = true;
        private bool _canWrite = true;
        private VisibilityScope _readScope;
        private VisibilityScope _writeScope;

        public TypeDefinition And => _parentDefinition;
        public TypeInfo PropertyType => _propertyType;
        public string PropertyName { get; set; }
        public bool CanRead => _canRead;
        public bool CanWrite => _canWrite;
        public VisibilityScope ReadScope => _readScope;
        public VisibilityScope WriteScope => _writeScope;

        internal PropertyDefinition(TypeDefinition parentDefinition, VisibilityScope visibilityScope, TypeInfo typeInfo)
        {
            _parentDefinition = parentDefinition;
            _readScope = _writeScope = visibilityScope;
            _propertyType = typeInfo;
        }
    }
}
