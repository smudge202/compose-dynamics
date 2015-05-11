using Compose.Dynamics.Definitions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Compose.Dynamics.Tests
{
    public class ConstructorDefinitionExtensionTests
    {
        public class Takes
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("  ")]
            public void WhenSuppliedInvalidParameterNameThenThrowsArgumentNullException(string parameterName)
            {
                var definition = new TypeDefinition().HasConstructor();

                Action act = () => definition.Takes<string>(parameterName);

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenSuppliedInvalidParameterThenThrowsArgumentNullException()
            {
                var definition = new TypeDefinition().HasConstructor();

                Action act = () => definition.Takes(default(IParameterDefinition));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenSuppliedInvalidParameterCollectionThenThrowsArgumentNullException()
            {
                var definition = new TypeDefinition().HasConstructor();

                Action act = () => definition.Takes(default(IEnumerable<IParameterDefinition>));

                act.ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void WhenSuppliedParameterByNameThenParameterIsAddedToCollection()
            {
                const string parameterName = "testParameter";

                var definition = new TypeDefinition().HasConstructor().Takes<string>(parameterName);

                definition.Parameters.Should().HaveCount(1);
                definition.Parameters.First().ParameterName.Should().Be(parameterName);
                definition.Parameters.First().ParameterType.Should().Be(typeof(string).GetTypeInfo());
            }

            [Fact]
            public void WhenSuppliedParameterThenParameterIsAddedToCollection()
            {
                var parameterType = typeof(string).GetTypeInfo();
                const string parameterName = "testParameter";

                var definition = new TypeDefinition().HasConstructor().Takes(new ParameterDefinition(parameterName, parameterType));

                definition.Parameters.Should().HaveCount(1);
                definition.Parameters.First().ParameterName.Should().Be(parameterName);
                definition.Parameters.First().ParameterType.Should().Be(parameterType);
            }

            [Fact]
            public void WhenSuppliedParameterCollectionThenParameterIsAddedToCollection()
            {
                var parameterType = typeof(string).GetTypeInfo();
                const string parameterName = "testParameter";

                var definition = new TypeDefinition().HasConstructor().Takes(new[] { new ParameterDefinition(parameterName, parameterType) } );

                definition.Parameters.Should().HaveCount(1);
                definition.Parameters.First().ParameterName.Should().Be(parameterName);
                definition.Parameters.First().ParameterType.Should().Be(parameterType);
            }
        }
    }
}
