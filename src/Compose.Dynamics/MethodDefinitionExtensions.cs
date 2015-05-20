using Compose.Dynamics.Definitions;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Compose.Dynamics
{
    public static class MethodDefinitionExtensions
    {
        public static IMethodDefinition WithMethodBody(this IMethodDefinition definition, MethodInfo methodInfo)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));

            definition.MethodBody = (il, parameterCount) =>
            {
                for (var i = 1; i <= parameterCount; i++)
                    il.Emit(OpCodes.Ldarg, i);

                il.Emit(OpCodes.Callvirt, methodInfo);
                il.Emit(OpCodes.Ret);
            };

            return definition;
        }

        public static IMethodDefinition WithMethodBody(this IMethodDefinition definition, Action<ILGenerator, int> methodBody)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            if (methodBody == null)
                throw new ArgumentNullException(nameof(methodBody));

            definition.MethodBody = methodBody;

            return definition;
        }

        public static IMethodDefinition WithScope(this IMethodDefinition definition, VisibilityScope scope)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            definition.Scope = scope;

            return definition;
        }
    }
}
