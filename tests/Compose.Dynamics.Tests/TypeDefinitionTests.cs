using FluentAssertions;
using System;
using System.Linq;
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
            public void WhenConstructorIsNotPassedAVisibiltyScopeThenVisbilityScopeDefaultsToPublic()
            {
                var typeDefinition = new TypeDefinition();

                typeDefinition.CurrentVisibiltyScope.Should().Be(VisibilityScope.Public);
            }
        }

        public class InheritsFrom
        {
            [Fact]
            public void WhenSuppliedViaGenericTypeThenInheritedFromIsTheSameType()
            {
                var typeDefinition = new TypeDefinition();

                var inheritanceDefinition = typeDefinition.InheritsFrom<TypeDefinition>();

                typeDefinition.InheritanceChain.Should().HaveCount(1);
                typeDefinition.InheritanceChain.First().InheritedFrom.Should().Be(typeof(TypeDefinition).GetTypeInfo());
            }

            [Fact]
            public void WhenSuppliedTypeThenInheritedFromIsTheSameType()
            {
                var typeDefinition = new TypeDefinition();

                var inheritanceDefinition = typeDefinition.InheritsFrom(typeof(TypeDefinition));

                typeDefinition.InheritanceChain.Should().HaveCount(1);
                typeDefinition.InheritanceChain.First().InheritedFrom.Should().Be(typeof(TypeDefinition).GetTypeInfo());
            }

            [Fact]
            public void WhenSuppliedTypeInfoThenInheritedFromIsTheSameType()
            {
                var typeDefinition = new TypeDefinition();

                var inheritanceDefinition = typeDefinition.InheritsFrom(typeof(TypeDefinition).GetTypeInfo());

                typeDefinition.InheritanceChain.Should().HaveCount(1);
                typeDefinition.InheritanceChain.First().InheritedFrom.Should().Be(typeof(TypeDefinition).GetTypeInfo());
            }

            [Fact]
            public void WhenSupplyingTwoClassesThenMustThrowInvalidOperationException()
            {
                var typeDefinition = new TypeDefinition();

                var inheritanceDefinition = typeDefinition.InheritsFrom<TypeDefinition>();
                Action act = () => typeDefinition.InheritsFrom<Type>();

                act.ShouldThrow<InvalidOperationException>();
            }

            [Fact]
            public void WhenSupplyingDuplicateDefinitionsThenOnlyOneRemainsInTheInheritanceChain()
            {
                var typeDefinition = new TypeDefinition();

                typeDefinition.InheritsFrom<TypeDefinition>();
                typeDefinition.InheritsFrom<TypeDefinition>();

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
                var propertyDefinition = typeDefinition.HasProperty<Guid>("TestProperty");

                propertyDefinition.ReadScope.Should().Be(scope);
                propertyDefinition.WriteScope.Should().Be(scope);
            }

            [Fact]
            public void WhenPropertyDefinedThenTypeDefinitionContainsTheSameDefinition()
            {
                var typeDefinition = new TypeDefinition();

                var PropertyDefinition = typeDefinition.HasProperty<Guid>("TestProperty");

                typeDefinition.Properties.Should().HaveCount(1);
                typeDefinition.Properties.First().Should().Be(PropertyDefinition);
            }

            [Fact]
            public void WhenPropertyDefinedThenPropertyDefinitionMustHaveCorrectProperties()
            {
                const VisibilityScope scope = VisibilityScope.Protected;
                const string propertyName = "TestProperty";
                var typeDefinition = new TypeDefinition();

                var propertyDefinition = typeDefinition.HasProperty<Guid>(scope, propertyName);

                propertyDefinition.ReadScope.Should().Be(scope);
                propertyDefinition.WriteScope.Should().Be(scope);
                propertyDefinition.CanRead.Should().Be(true);
                propertyDefinition.CanWrite.Should().Be(true);
                propertyDefinition.PropertyName.Should().Be(propertyName);
                propertyDefinition.PropertyType.Should().Be(typeof(Guid).GetTypeInfo());
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("  ")]
            public void WhenPropertyNameIsNullOrEmptyThenPropertyDefinitionMustThrowArgumentNullException(string propertyName)
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.HasProperty<Guid>(propertyName);

                act.ShouldThrow<ArgumentNullException>();
            }
        }

        public class HasMethod
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("  ")]
            public void WhenMethodNameIsNullOrEmptyThenMethodMustThrowArgumentNullException(string methodName)
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.HasMethod(methodName);

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenReturnTypeIsNullThenMethodMustThrowArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.HasMethod("TestMethod", null);

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenParametersAreNullThenMethodMustThrowArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();

                Action act = () => typeDefinition.HasMethod("TestMethod", typeof(Type), null);

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenProvidedWithMethodNameThenMethodShouldHaveNoParametersAndReturnTypeMustBeVoid()
            {
                const string methodName = "TestMethod";
                var typeDefinition = new TypeDefinition();

                var methodDefinition = typeDefinition.HasMethod(methodName);

                methodDefinition.MethodName.Should().Be(methodName);
                methodDefinition.ReturnType.Should().Be(Type.GetType("System.Void"));
                methodDefinition.Parameters.Should().HaveCount(0);
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
                var initialScope = VisibilityScope.Public;
                var typeDefinition = new TypeDefinition(initialScope);

                var methodScope = typeDefinition.HasMethod("TestMethod");

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
    }
}
