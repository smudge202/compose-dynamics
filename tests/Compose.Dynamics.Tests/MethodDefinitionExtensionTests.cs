﻿using Compose.Dynamics.Definitions;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace Compose.Dynamics.Tests
{
    public class MethodDefinitionExtensionTests
    {
        public class WithMethodBody
        {
            [Fact]
            public void GivenANullMethodDefinitionThenThrowArgumentNullException()
            {
                var methodDefinition = default(IMethodDefinition);

                Action act = () => MethodDefinitionExtensions.WithMethodBody(methodDefinition, GetStandardMethodBody("Test", typeof(string)));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void GivenAMethodInfoThenMethodBodyIsSet()
            {
                var typeDefinition = new TypeDefinition();
                var methodDefinition = typeDefinition.HasMethod();

                methodDefinition.WithMethodBody(GetStandardMethodBody("Test", Type.GetType("System.Void")));

                methodDefinition.MethodBody.Should().NotBeNull();
            }

            [Fact]
            public void GivenANullMethodInfoThenThrowAnArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();
                var methodDefinition = typeDefinition.HasMethod();

                Action act = () => methodDefinition.WithMethodBody(default(MethodInfo));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void GivenANullActionThenThrowAnArgumentNullException()
            {
                var typeDefinition = new TypeDefinition();
                var methodDefinition = typeDefinition.HasMethod();

                Action act = () => MethodDefinitionExtensions.WithMethodBody(methodDefinition, (Action<ILGenerator, Type[]>)null);

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void GivenANullMethodDefinitionOnTheActionOverloadThenThrowArgumentNullException()
            {
                var definition = default(IMethodDefinition);

                Action act = () => MethodDefinitionExtensions.WithMethodBody(definition, (il, count) => { });

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void GivenAnActionThenMethodBodyIsSet()
            {
                var typeDefinition = new TypeDefinition();
                var methodDefinition = typeDefinition.HasMethod();

                methodDefinition.WithMethodBody((il, count) => { });

                methodDefinition.MethodBody.Should().NotBeNull();
            }

            [Fact]
            public void GivenAnActionOfTThenCanInvokeWithoutError()
            {
                var isInvoked = false;
                var definition = new TypeDefinition().HasMethod<Fake>(x => TestInvoke(ref isInvoked));

                try
                {
                    GenerateTestMethodInvokationAction(definition)();
                }
                catch(Exception ex)
                {
                    throw ex;
                }

                isInvoked.Should().BeTrue();
            }

            private static MethodInfo GetStandardMethodBody(string methodName, Type returnType, params Type[] parameters) => new DynamicMethod(methodName, returnType, parameters);

            private static Action GenerateTestMethodInvokationAction(IMethodDefinition definition)
            {
                var parameters = definition.Parameters.Select(x => x.ParameterType).ToArray();
                var dm = new DynamicMethod("Test", definition.ReturnType, parameters);
                definition.MethodBody(dm.GetILGenerator(), parameters);
                return () => dm.Invoke(null, Enumerable.Repeat(default(Fake), definition.Parameters.Count()).ToArray());
            }

            private void TestInvoke(ref bool invoked)
            {
                invoked = true;
            }

            public class Fake
            { }
        }

        public class WithScope
        {
            [Theory]
            [InlineData(VisibilityScope.Internal)]
            [InlineData(VisibilityScope.Private)]
            [InlineData(VisibilityScope.Protected)]
            [InlineData(VisibilityScope.Protectednternal)]
            [InlineData(VisibilityScope.Public)]
            public void GivenASpecificScopeThenScopeIsSet(VisibilityScope scope)
            {
                var typeDefinition = new TypeDefinition();
                var methodDefinition = typeDefinition.HasMethod();

                methodDefinition.WithScope(scope).Scope.Should().Be(scope);
            }

            [Fact]
            public void GivenANullDefinitionThenThrowANullArgumentException()
            {
                var definition = default(IMethodDefinition);

                Action act = () => MethodDefinitionExtensions.WithScope(definition, VisibilityScope.Public);

                act.ShouldThrow<ArgumentNullException>();
            }
        }
    }
}
