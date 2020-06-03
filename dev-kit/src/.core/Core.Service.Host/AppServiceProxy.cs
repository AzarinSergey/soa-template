using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Core.Service.Host
{
    //https://habr.com/ru/company/skbkontur/blog/262711/
    //https://www.codeproject.com/Articles/3778/Introduction-to-IL-Assembly-Language
    //public static class AppServiceProxy
    //{
    //    public static TService GetClient<TService>()
    //    {
    //        var serviceType = typeof(TService);

    //        if (!serviceType.IsInterface)
    //        {
    //            throw new ArgumentException($"'TService' - interface type only allowed.");
    //        }
    //    }

    //    private static void GenerateMethods(List<string> usedNames, Type interfaceType, TypeBuilder tb, List<MethodInfo> propAccessors)
    //    {
    //        foreach (MethodInfo mi in interfaceType.GetMethods())
    //        {
    //            var parameterInfoArray = mi.GetParameters();
    //            var genericArgumentArray = mi.GetGenericArguments();

    //            if (!propAccessors.Contains(mi))
    //            {
    //                var mb = tb.DefineMethod(mi.Name, MethodAttributes.Public, mi.ReturnType, 
    //                    parameterInfoArray.Select(pi => pi.ParameterType).ToArray());

    //                if (genericArgumentArray.Any())
    //                {
    //                    mb.DefineGenericParameters(genericArgumentArray.Select(s => s.Name).ToArray());
    //                }

    //                var ilGenerator = mb.GetILGenerator();

    //                mb.

    //                EmitInvokeMethod(mi, mb);

    //                tb.DefineMethodOverride(mb, mi);
    //            }
    //        }
    //    }
    //}
}
