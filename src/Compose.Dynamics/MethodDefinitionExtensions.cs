using Compose.Dynamics.Definitions;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

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

            definition.MethodBody = (il, parameters) =>
            {
                if (methodInfo.IsStatic)
                    il.Emit(OpCodes.Ldnull);
                else
                {
                    for (var i = 1; i <= parameters.Length; i++)
                        il.Emit(OpCodes.Ldarg, i);

                    var voidType = Type.GetType("System.Void");
                    if (methodInfo.ReturnType == voidType)
                    {
                        var actionType = typeof(Action<>).MakeGenericType(parameters);
                        il.DeclareLocal(actionType);
                        if (methodInfo.DeclaringType == actionType)
                        {
                            il.Emit(OpCodes.Ldftn, methodInfo);
                            il.Emit(OpCodes.Newobj, actionType.GetConstructors()[0]);
                            il.Emit(OpCodes.Stloc_0);
                            il.Emit(OpCodes.Ldloc_0);
                        }
                    }
                    else
                    {
                        var funcType = typeof(Func<>).MakeGenericType(parameters.Concat(new[] { methodInfo.ReturnType }).ToArray());
                        if (methodInfo.DeclaringType == funcType)
                        {
                            il.DeclareLocal(funcType);
                            il.Emit(OpCodes.Newobj, funcType.GetConstructors()[0]);
                            il.Emit(OpCodes.Stloc_0);
                            il.Emit(OpCodes.Ldloc_0);
                        }
                    }
                }

                il.DeclareLocal(typeof(object[]));
                il.Emit(OpCodes.Ldc_I4, parameters.Length);
                il.Emit(OpCodes.Newarr, typeof(object));
                il.Emit(OpCodes.Stloc_1);

                for (var i = 1; i <= parameters.Length; i++)
                {
                    il.Emit(OpCodes.Ldloc_1);
                    il.Emit(OpCodes.Ldarg, i);
                    il.Emit(OpCodes.Stelem, i - 1);
                }

                il.Emit(OpCodes.Callvirt, methodInfo);
                il.Emit(OpCodes.Ret);
            };

            return definition;
        }

        public static IMethodDefinition WithMethodBody(this IMethodDefinition definition, Action<ILGenerator, Type[]> methodBody)
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
