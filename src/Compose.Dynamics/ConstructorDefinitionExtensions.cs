using Compose.Dynamics.Definitions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Compose.Dynamics
{
    public static class ConstructorDefinitionExtensions
    {
        public static IConstructorDefinition Takes<T>(this IConstructorDefinition constructorDefinition, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
                throw new ArgumentNullException(nameof(parameterName));

            constructorDefinition.Parameters.Add(new ParameterDefinition(parameterName, typeof(T).GetTypeInfo()));
            return constructorDefinition;
        }

        public static IConstructorDefinition Takes(this IConstructorDefinition constructorDefinition, IParameterDefinition parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            constructorDefinition.Parameters.Add(parameter);
            return constructorDefinition;
        }

        public static IConstructorDefinition Takes(this IConstructorDefinition constructorDefinition, IEnumerable<IParameterDefinition> parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var result = constructorDefinition;
            foreach(var parameter in parameters)
            {
                result = result.Takes(parameter);
            }
            return result;
        }
    }
}
