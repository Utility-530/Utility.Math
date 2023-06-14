using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilityMath.WpfApp
{
    public static class MethodInfoHelper
    {
        public static IEnumerable<System.Reflection.MethodInfo> GetMethodsBySignature(this Type type, Type returnType, params Type[] parameterTypes) =>
            type.GetMethods().Where(p =>
              p.GetParameters().Select(q =>
              q.ParameterType).SequenceEqual(parameterTypes) && p.ReturnType == returnType);
    }
}