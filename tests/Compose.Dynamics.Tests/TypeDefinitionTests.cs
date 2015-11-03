using Compose.Dynamics.Definitions;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Compose.Dynamics.Tests
{
    public class TypeDefinitionTests
    {
        public class Constructor
        {
            [Theory]
            [InlineData(VisibilityScope.Public)]
            [InlineData(VisibilityScope.Internal)]
            [InlineData(VisibilityScope.Protectednternal)]
            [InlineData(VisibilityScope.Protected)]
            [InlineData(VisibilityScope.Private)]
            public void WhenConstructorIsPassedVisibilityScopeThenScopeIsSetToCorrectScope(VisibilityScope scope)
            {
                var typeDefinition = new TypeDefinition(scope);

                typeDefinition.CurrentVisibiltyScope.Should().Be(scope);
            }

            [Fact]
            public void WhenConstructorIsNotPassedAVisibiltyScopeThenVisbilityScopeDefaultsToInternal()
            {
                var typeDefinition = new TypeDefinition();

                typeDefinition.CurrentVisibiltyScope.Should().Be(VisibilityScope.Internal);
            }
        }

        public class InheritsFrom
        {
            [Fact]
            public void WhenSuppliedViaGenericTypeThenInheritedFromIsTheSameType()
            {
                var typeDefinition = new TypeDefinition();

                var inheritanceDefinition = typeDefinition.InheritsFrom<UnsealedType>();

                typeDefinition.InheritanceChain.Should().HaveCount(1);
                typeDefinition.InheritanceChain.First().InheritedFrom.Should().Be(typeof(UnsealedType).GetTypeInfo());
            }

            [Fact]
            public void WhenSuppliedTypeThenInheritedFromIsTheSameType()
            {
                var typeDefinition = new TypeDefinition();

                var inheritanceDefinition = typeDefinition.InheritsFrom(typeof(UnsealedType));

                typeDefinition.InheritanceChain.Should().HaveCount(1);
                typeDefinition.InheritanceChain.First().InheritedFrom.Should().Be(typeof(UnsealedType).GetTypeInfo());
            }

            [Fact]
            public void WhenSuppliedTypeInfoThenInheritedFromIsTheSameType()
            {
                var typeDefinition = new TypeDefinition();

                var inheritanceDefinition = typeDefinition.InheritsFrom(typeof(UnsealedType).GetTypeInfo());

                typeDefinition.InheritanceChain.Should().HaveCount(1);
                typeDefinition.InheritanceChain.First().InheritedFrom.Should().Be(typeof(UnsealedType).GetTypeInfo());
            }

            [Fact]
            public void WhenSupplyingTwoClassesThenMustThrowInvalidOperationException()
            {
                var typeDefinition = new TypeDefinition();

                var inheritanceDefinition = typeDefinition.InheritsFrom<UnsealedType>();
                Action act = () => typeDefinition.InheritsFrom<Type>();

                act.ShouldThrow<InvalidOperationException>();
            }

            [Fact]
            public void WhenSupplyingDuplicateDefinitionsThenOnlyOneRemainsInTheInheritanceChain()
            {
                var typeDefinition = new TypeDefinition();

                typeDefinition.InheritsFrom<UnsealedType>();
                typeDefinition.InheritsFrom<UnsealedType>();

                typeDefinition.InheritanceChain.Should().HaveCount(1);
            }

            [Fact]
            public void WhenSuppliedTypeIsNullThenThrowsArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.InheritsFrom(default(Type));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenSuppliedTypeInfoIsNullThenThrowsArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.InheritsFrom(default(TypeInfo));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenSuppliedAnInterfaceAsGenericThenThrowsNotSupportedException()
            {
                var definition = new TypeDefinition();

                Action act = () => definition.InheritsFrom<IFluentDefinition>();

                act.ShouldThrow<NotSupportedException>();
            }

            [Fact]
            public void WhenSuppliedAnInterfaceAsTypeThenThrowsNotSupportedException()
            {
                var definition = new TypeDefinition();

                Action act = () => definition.InheritsFrom(typeof(IFluentDefinition));

                act.ShouldThrow<NotSupportedException>();
            }

            [Fact]
            public void WhenSuppliedAnInterfaceAsTypeInfoThenThrowsNotSupportedException()
            {
                var definition = new TypeDefinition();

                Action act = () => definition.InheritsFrom(typeof(IFluentDefinition).GetTypeInfo());

                act.ShouldThrow<NotSupportedException>();
            }

            [Fact]
            public void WhenSuppliedASealedTypeThenThrowNotSupportedException()
            {
                var definition = new TypeDefinition();

                Action act = () => definition.InheritsFrom<SealedType>();

                act.ShouldThrow<NotSupportedException>();
            }

            private sealed class SealedType { }
            private class UnsealedType { }
        }

        public class HasConstructor
        {
            [Theory]
            [InlineData(VisibilityScope.Public)]
            [InlineData(VisibilityScope.Internal)]
            [InlineData(VisibilityScope.Protectednternal)]
            [InlineData(VisibilityScope.Protected)]
            [InlineData(VisibilityScope.Private)]
            public void WhenNotSuppliedWithVisibilityScopeThenConstructorDefinitionIsScopedToTypeDefinition(VisibilityScope scope)
            {
                var typeDefinition = new TypeDefinition(scope);

                typeDefinition.HasConstructor(scope).Scope.Should().Be(scope);
            }

            [Fact]
            public void WhenConstructorDefinedThenTypeDefinitionContainsTheSameDefinition()
            {
                var typeDefinition = new TypeDefinition();

                var constructorDefinition = typeDefinition.HasConstructor();

                typeDefinition.Constructors.Should().HaveCount(1);
                typeDefinition.Constructors.First().Should().Be(constructorDefinition);
            }
        }

        public class HasProperty
        {
            [Theory]
            [InlineData(VisibilityScope.Public)]
            [InlineData(VisibilityScope.Internal)]
            [InlineData(VisibilityScope.Protectednternal)]
            [InlineData(VisibilityScope.Protected)]
            [InlineData(VisibilityScope.Private)]
            public void WhenNotSuppliedWithVisibilityScopeThenPropertyDefinitionIsScopedToTypeDefinition(VisibilityScope scope)
            {
                var typeDefinition = new TypeDefinition(scope);
                var propertyDefinition = typeDefinition.HasProperty<Guid>();

                propertyDefinition.ReadScope.Should().Be(scope);
                propertyDefinition.WriteScope.Should().Be(scope);
            }

            [Fact]
            public void WhenPropertyDefinedThenTypeDefinitionContainsTheSameDefinition()
            {
                var typeDefinition = new TypeDefinition();

                var PropertyDefinition = typeDefinition.HasProperty<Guid>();

                typeDefinition.Properties.Should().HaveCount(1);
                typeDefinition.Properties.First().Should().Be(PropertyDefinition);
            }

            [Fact]
            public void WhenPropertyDefinedThenPropertyDefinitionMustHaveCorrectProperties()
            {
                const VisibilityScope scope = VisibilityScope.Protected;
                var typeDefinition = new TypeDefinition();

                var propertyDefinition = typeDefinition.HasProperty<Guid>(scope);

                propertyDefinition.ReadScope.Should().Be(scope);
                propertyDefinition.WriteScope.Should().Be(scope);
                propertyDefinition.CanRead.Should().Be(true);
                propertyDefinition.CanWrite.Should().Be(true);
                propertyDefinition.PropertyType.Should().Be(typeof(Guid).GetTypeInfo());
            }
        }

        public class HasMethod
        {
            [Fact]
            public void WhenReturnTypeIsNullThenMethodDefinitionHasVoidReturnType()
            {
                var typeDefinition = new TypeDefinition();

                var methodDefinition = typeDefinition.HasMethod(default(Type));

                methodDefinition.ReturnType.Should().Be(Type.GetType("System.Void"));
            }

            [Fact]
            public void WhenParametersAreNullThenMethodMustThrowArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.HasMethod(typeof(Type), null);

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenProvidedWithMethodNameThenMethodShouldHaveNoParametersAndReturnTypeMustBeVoid()
            {
                var typeDefinition = new TypeDefinition();

                var methodDefinition = typeDefinition.HasMethod();

                methodDefinition.ReturnType.Should().Be(Type.GetType("System.Void"));
                methodDefinition.Parameters.Should().HaveCount(0);
            }

            [Fact]
            public void WhenProvidedWithANullActionOfT1ThenShouldThrowArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.HasMethod(default(Expression<Action<string>>));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenProvidedWithANullActionOfT1T2ThenShouldThrowArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.HasMethod(default(Expression<Action<string, string>>));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenProvidedWithANullActionOfT1T2T3ThenShouldThrowArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.HasMethod(default(Expression<Action<string, string, string>>));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenProvidedWithANullActionOfT1T2T3T4ThenShouldThrowArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.HasMethod(default(Expression<Action<string, string, string, string>>));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenProvidedWithANullFuncOfT1ThenShouldThrowArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.HasMethod(default(Expression<Func<string>>));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenProvidedWithANullFuncOfT1T2ThenShouldThrowArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.HasMethod(default(Expression<Func<string, string>>));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenProvidedWithANullFuncOfT1T2T3ThenShouldThrowArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.HasMethod(default(Expression<Func<string, string, string>>));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenProvidedWithANullFuncOfT1T2T3T4ThenShouldThrowArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.HasMethod(default(Expression<Func<string, string, string, string>>));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenProvidedWithANullFuncOfT1T2T3T4T5ThenShouldThrowArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.HasMethod(default(Expression<Func<string, string, string, string, string>>));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenProvidedWithAnActionOfT1ThenShouldReturnADefinitionWithOneParametersAndVoidReturnType()
            {
                var typeDefinition = new TypeDefinition();

                var methodDefinition = typeDefinition.HasMethod<TestClass>(obj => obj.SomeMethod());

                methodDefinition.ReturnType.Should().Be(Type.GetType("System.Void"));
                methodDefinition.Parameters.Should().HaveCount(1);
            }

            [Fact]
            public void WhenProvidedWithAnActionOfT1T2ThenShouldReturnADefinitionWithTwoParametersAndVoidReturnType()
            {
                var typeDefinition = new TypeDefinition();

                var methodDefinition = typeDefinition.HasMethod<TestClass, TestClass>((obj, obj2) => obj.SomeMethod());

                methodDefinition.ReturnType.Should().Be(Type.GetType("System.Void"));
                methodDefinition.Parameters.Should().HaveCount(2);
            }

            [Fact]
            public void WhenProvidedWithAnActionOfT1T2T3ThenShouldReturnADefinitionWithThreeParametersAndVoidReturnType()
            {
                var typeDefinition = new TypeDefinition();

                var methodDefinition = typeDefinition.HasMethod<TestClass, TestClass, TestClass>((obj, obj2, obj3) => obj.SomeMethod());

                methodDefinition.ReturnType.Should().Be(Type.GetType("System.Void"));
                methodDefinition.Parameters.Should().HaveCount(3);
            }

            [Fact]
            public void WhenProvidedWithAnActionOfT1T2T3T4ThenShouldReturnADefinitionWithFourParametersAndVoidReturnType()
            {
                var typeDefinition = new TypeDefinition();

                var methodDefinition = typeDefinition.HasMethod<TestClass, TestClass, TestClass, TestClass>((obj, obj2, obj3, obj4) => obj.SomeMethod());

                methodDefinition.ReturnType.Should().Be(Type.GetType("System.Void"));
                methodDefinition.Parameters.Should().HaveCount(4);
            }

            [Fact]
            public void WhenProvidedWithAnFuncOfT1ThenShouldReturnADefinitionWithNoParametersAndTestClassReturnType()
            {
                var typeDefinition = new TypeDefinition();

                var methodDefinition = typeDefinition.HasMethod<TestClass>(() => new TestClass());

                methodDefinition.ReturnType.Should().Be(typeof(TestClass));
                methodDefinition.Parameters.Should().HaveCount(0);
            }

            [Fact]
            public void WhenProvidedWithAnFuncOfT1T2ThenShouldReturnADefinitionWithOneParametersAndTestClassReturnType()
            {
                var typeDefinition = new TypeDefinition();

                var methodDefinition = typeDefinition.HasMethod<TestClass, TestClass>((cls) => cls);

                methodDefinition.ReturnType.Should().Be(typeof(TestClass));
                methodDefinition.Parameters.Should().HaveCount(1);
            }

            [Fact]
            public void WhenProvidedWithAnFuncOfT1T2T3ThenShouldReturnADefinitionWithTwoParametersAndTestClassReturnType()
            {
                var typeDefinition = new TypeDefinition();

                var methodDefinition = typeDefinition.HasMethod<TestClass, TestClass, TestClass>((cls, cls1) => cls);

                methodDefinition.ReturnType.Should().Be(typeof(TestClass));
                methodDefinition.Parameters.Should().HaveCount(2);
            }

            [Fact]
            public void WhenProvidedWithAnFuncOfT1T2T3T4ThenShouldReturnADefinitionWithThreeParametersAndTestClassReturnType()
            {
                var typeDefinition = new TypeDefinition();

                var methodDefinition = typeDefinition.HasMethod<TestClass, TestClass, TestClass, TestClass>((cls, cls1, cls2) => cls);

                methodDefinition.ReturnType.Should().Be(typeof(TestClass));
                methodDefinition.Parameters.Should().HaveCount(3);
            }

            [Fact]
            public void WhenProvidedWithAnFuncOfT1T2T3T4T5ThenShouldReturnADefinitionWithFourParametersAndTestClassReturnType()
            {
                var typeDefinition = new TypeDefinition();

                var methodDefinition = typeDefinition.HasMethod<TestClass, TestClass, TestClass, TestClass, TestClass>((cls, cls1, cls2, cls3) => cls);

                methodDefinition.ReturnType.Should().Be(typeof(TestClass));
                methodDefinition.Parameters.Should().HaveCount(4);
            }

            private class TestClass
            {
                internal void SomeMethod() { }
                internal TestClass SomeOtherMethod() => this;
            }
        }

        public class WithVisibilityScope
        {
            [Theory]
            [InlineData(VisibilityScope.Public)]
            [InlineData(VisibilityScope.Internal)]
            [InlineData(VisibilityScope.Protectednternal)]
            [InlineData(VisibilityScope.Protected)]
            [InlineData(VisibilityScope.Private)]
            public void WhenPassedNewVisibilityScopeThenCurrentScopeIsChanged(VisibilityScope newScope)
            {
                var typeDefinition = new TypeDefinition();

                typeDefinition.WithVisibilityScope(newScope);

                typeDefinition.CurrentVisibiltyScope.Should().Be(newScope);
            }

            [Theory]
            [InlineData(VisibilityScope.Internal)]
            [InlineData(VisibilityScope.Protectednternal)]
            [InlineData(VisibilityScope.Protected)]
            [InlineData(VisibilityScope.Private)]
            public void WhenPassedNewVisibilityScopeThenPreviousScopesDoNotChange(VisibilityScope newScope)
            {
                var initialScope = VisibilityScope.Private;
                var typeDefinition = new TypeDefinition(initialScope);

                var methodScope = typeDefinition.HasMethod();

                typeDefinition.WithVisibilityScope(newScope);

                typeDefinition.CurrentVisibiltyScope.Should().Be(newScope);
                methodScope.Scope.Should().Be(initialScope);
            }
        }

        public class Seal
        {
            [Fact]
            public void ByDefaultTypeDefinitionShouldNotBeSealed()
            {
                var typeDefinition = new TypeDefinition();

                typeDefinition.IsSealed.Should().BeFalse();
            }

            [Fact]
            public void WhenSealIsCalledThenTypeDefintionShouldBeMarkedAsSealed()
            {
                var typeDefinition = new TypeDefinition();

                typeDefinition.Seal();

                typeDefinition.IsSealed.Should().BeTrue();
            }

            [Fact]
            public void WhenDefinitionIsMarkedAsAbstractThenThrowInvalidOperationException()
            {
                var typeDefinition = new TypeDefinition();

                typeDefinition.AsAbstract();
                Action act = () => typeDefinition.Seal();

                act.ShouldThrow<InvalidOperationException>();
            }
        }

        public class AsAbstract
        {
            [Fact]
            public void WhenDefinitionIsSealedThenThrowInvalidOperationException()
            {
                var typeDefinition = new TypeDefinition();

                typeDefinition.Seal();
                Action act = () => typeDefinition.AsAbstract();

                act.ShouldThrow<InvalidOperationException>();
            }

            [Fact]
            public void WhenAsAbstractIsCalledThenMarksDefinitionAsAbstract()
            {
                var typeDefinition = new TypeDefinition();

                typeDefinition.AsAbstract();

                typeDefinition.IsAbstract.Should().BeTrue();
            }

            [Fact]
            public void ByDefaultIsAbstractShouldBeFalse()
            {
                var typeDefinition = new TypeDefinition();

                typeDefinition.IsAbstract.Should().BeFalse();
            }
        }

        public class Implements
        {
            [Fact]
            public void WhenSuppliedAClassThenThrowNotSupportException()
            {
                var definition = new TypeDefinition();

                Action act = () => definition.Implements<StandardClass>();

                act.ShouldThrow<NotSupportedException>();
            }

            [Fact]
            public void WhenSuppliedAStructThenThrowsNotSupportedException()
            {
                var definition = new TypeDefinition();

                Action act = () => definition.Implements<StandardStruct>();

                act.ShouldThrow<NotSupportedException>();
            }

            [Fact]
            public void WhenSuppliedAnInterfaceThenDefinitionIsAddedToCollection()
            {
                var definition = new TypeDefinition();

                definition.Implements<IPropertyDefinition>();

                definition.Implementations.Should().HaveCount(1);
            }

            [Fact]
            public void WhenSuppliedAnInterfaceAsATypeThenDefinitionIsAddedToCollection()
            {
                var definition = new TypeDefinition();

                definition.Implements(typeof(IPropertyDefinition));

                definition.Implementations.Should().HaveCount(1);
            }

            [Fact]
            public void WhenSuppliedAnInterfaceAsTypeInfoThenDefinitionIsAddedToCollection()
            {
                var definition = new TypeDefinition();

                definition.Implements(typeof(IPropertyDefinition).GetTypeInfo());

                definition.Implementations.Should().HaveCount(1);
            }

            [Fact]
            public void WhenSuppliedADuplicateInterfaceThenNoExceptionIsThrowAndOnlyInterfaceIsRegestered()
            {
                var definition = new TypeDefinition();

                definition.Implements<IPropertyDefinition>();
                definition.Implements<IPropertyDefinition>();

                definition.Implementations.Should().HaveCount(1);
            }


            private class StandardClass { }
            private struct StandardStruct { }
        }
    }
}
