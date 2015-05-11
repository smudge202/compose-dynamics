﻿using Compose.Dynamics.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Compose.Dynamics
{
    public sealed class TypeDefinition
    {
        private bool _seal = false;
        private bool _abstract = false;
        private readonly List<IInheritanceDefinition> _inheritanceChain = new List<IInheritanceDefinition>();
        private readonly List<IPropertyDefinition> _properties = new List<IPropertyDefinition>();
        private readonly List<IConstructorDefinition> _constructors = new List<IConstructorDefinition>();
        private readonly List<IMethodDefinition> _methods = new List<IMethodDefinition>();
        private static readonly IParameterDefinition[] _emptyParameterTypes = new IParameterDefinition[0];
        private VisibilityScope _currentVisbilityScope;
        internal VisibilityScope CurrentVisibiltyScope => _currentVisbilityScope;
        internal IEnumerable<IConstructorDefinition> Constructors => _constructors;
        internal IEnumerable<IPropertyDefinition> Properties => _properties;
        internal IEnumerable<IInheritanceDefinition> InheritanceChain => _inheritanceChain;
        internal IEnumerable<IMethodDefinition> Methods => _methods;
        internal bool IsSealed => _seal;
        internal bool IsAbstract => _abstract;


        public TypeDefinition(VisibilityScope visibilityScope)
        {
            _currentVisbilityScope = visibilityScope;
        }
        public TypeDefinition() : this(VisibilityScope.Public) { }


        public TypeDefinition AsAbstract()
        {
            if (IsSealed)
                throw new InvalidOperationException("The class has already been sealed.");

            _abstract = true;
            return this;
        }


        public IConstructorDefinition HasConstructor() => HasConstructor(VisibilityScope.Public);
        public IConstructorDefinition HasConstructor(VisibilityScope scope)
        {
            var definition = new ConstructorDefinition(this, scope);
            _constructors.Add(definition);
            return definition;
        }


        public IMethodDefinition HasMethod(string methodName) => HasMethod(methodName, Type.GetType("System.Void"));
        public IMethodDefinition HasMethod(string methodName, Type returnType) => HasMethod(methodName, returnType, _emptyParameterTypes);
        public IMethodDefinition HasMethod(string methodName, Type returnType, IEnumerable<IParameterDefinition> parameters)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName));

            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var definition = new MethodDefinition(this, _currentVisbilityScope, methodName, returnType, parameters.ToArray());
            _methods.Add(definition);
            return definition;
        }


        public IPropertyDefinition HasProperty<T>(string propertyName) => HasProperty<T>(CurrentVisibiltyScope, propertyName);
        public IPropertyDefinition HasProperty<T>(VisibilityScope scope, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            var definition = new PropertyDefinition(this, scope, typeof(T).GetTypeInfo(), propertyName);
            _properties.Add(definition);
            return definition;
        }


        public TypeDefinition InheritsFrom<T>() => InheritsFrom(typeof(T));
        public TypeDefinition InheritsFrom(Type inheritFrom)
        {
            if (inheritFrom == null)
                throw new ArgumentNullException(nameof(inheritFrom));

            return InheritsFrom(inheritFrom.GetTypeInfo());
        }
        public TypeDefinition InheritsFrom(TypeInfo inheritFrom)
        {
            if (inheritFrom == null)
                throw new ArgumentNullException(nameof(inheritFrom));

            if (InheritanceChain.Any(x => x.InheritedFrom == inheritFrom))
                return this;

            if (inheritFrom.IsClass && InheritanceChain.Any(x => x.InheritedFrom.IsClass))
                throw new InvalidOperationException("Inheritance from multiple classes is not supported");

            var definition = new InheritanceDefinition(inheritFrom);
            _inheritanceChain.Add(definition);
            return this;
        }


        public TypeDefinition Seal()
        {
            if (IsAbstract)
                throw new InvalidOperationException("You cannot seal an abstract class");

            _seal = true;
            return this;
        }


        public TypeDefinition WithVisibilityScope(VisibilityScope newScope)
        {
            _currentVisbilityScope = newScope;
            return this;
        }
    }
}
