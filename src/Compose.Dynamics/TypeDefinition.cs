using Compose.Dynamics.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        private readonly List<TypeInfo> _implements = new List<TypeInfo>();
        private static readonly IParameterDefinition[] _emptyParameterTypes = new IParameterDefinition[0];
        private VisibilityScope _currentVisbilityScope;
        internal VisibilityScope CurrentVisibiltyScope => _currentVisbilityScope;
        internal IEnumerable<IConstructorDefinition> Constructors => _constructors.ToArray();
        internal IEnumerable<IPropertyDefinition> Properties => _properties.ToArray();
        internal IEnumerable<IInheritanceDefinition> InheritanceChain => _inheritanceChain.ToArray();
        internal IEnumerable<IMethodDefinition> Methods => _methods.ToArray();
        internal bool IsSealed => _seal;
        internal bool IsAbstract => _abstract;
        internal TypeInfo[] Implementations => _implements.ToArray();


        public TypeDefinition(VisibilityScope visibilityScope)
        {
            _currentVisbilityScope = visibilityScope;
        }
        public TypeDefinition() : this(VisibilityScope.Internal) { }


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


        public IMethodDefinition HasMethod() => HasMethod(default(Type));
        public IMethodDefinition HasMethod(Type returnType) => HasMethod(returnType, _emptyParameterTypes);
        public IMethodDefinition HasMethod(Type returnType, IEnumerable<IParameterDefinition> parameters)
        {
            if (returnType == null)
                returnType = Type.GetType("System.Void");

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var definition = new MethodDefinition(this, VisibilityScope.Private, returnType, parameters.ToArray());
            _methods.Add(definition);
            return definition;
        }
        public IMethodDefinition HasMethod<T>(Expression<Action<T>> methodBody) => CreateMethodFromExpression(methodBody);
        public IMethodDefinition HasMethod<T1, T2>(Expression<Action<T1, T2>> methodBody) => CreateMethodFromExpression(methodBody);
        public IMethodDefinition HasMethod<T1, T2, T3>(Expression<Action<T1, T2, T3>> methodBody) => CreateMethodFromExpression(methodBody);
        public IMethodDefinition HasMethod<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> methodBody) => CreateMethodFromExpression(methodBody);
        public IMethodDefinition HasMethod<TResult>(Expression<Func<TResult>> methodBody) => CreateMethodFromExpression(methodBody); 
        public IMethodDefinition HasMethod<T, TResult>(Expression<Func<T, TResult>> methodBody) => CreateMethodFromExpression(methodBody);
        public IMethodDefinition HasMethod<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> methodBody) => CreateMethodFromExpression(methodBody);
        public IMethodDefinition HasMethod<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> methodBody) => CreateMethodFromExpression(methodBody);
        public IMethodDefinition HasMethod<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>> methodBody) => CreateMethodFromExpression(methodBody);
        private IMethodDefinition CreateMethodFromExpression(Expression methodBody)
        {
            if (methodBody == null)
                throw new ArgumentNullException(nameof(methodBody));

            var definition = HasMethod();
            var lambda = (LambdaExpression)methodBody;
            var target = lambda.Compile();
            var methodInfo = target.GetType().GetRuntimeMethod("Invoke", lambda.Parameters.Select(param => param.Type).ToArray());
            var returnType = methodInfo.ReturnType;
            var parameters = lambda.Parameters.Select(param => new ParameterDefinition(param.Name, param.Type.GetTypeInfo()));

            return HasMethod(returnType, parameters).WithMethodBody(methodInfo);
        }

        public IPropertyDefinition HasProperty<T>() => HasProperty<T>(CurrentVisibiltyScope);
        public IPropertyDefinition HasProperty<T>(VisibilityScope scope)
        {
            var definition = new PropertyDefinition(this, scope, typeof(T).GetTypeInfo());
            _properties.Add(definition);
            return definition;
        }


        public TypeDefinition Implements<T>() => Implements(typeof(T));
        public TypeDefinition Implements(Type implementationType)
        {
            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            return Implements(implementationType.GetTypeInfo());
        }
        public TypeDefinition Implements(TypeInfo implementationType)
        {
            if (!implementationType.IsInterface)
                throw new NotSupportedException("You can only implement interfaces. Please you InheritsFrom() for classes.");

            if (!_implements.Contains(implementationType))
                _implements.Add(implementationType);

            return this;
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

            if (inheritFrom.IsInterface)
                throw new NotSupportedException("Interfaces should be added via the .Implements() API");

            if (inheritFrom.IsSealed)
                throw new NotSupportedException("Sealed classes cannot be inherited");

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
