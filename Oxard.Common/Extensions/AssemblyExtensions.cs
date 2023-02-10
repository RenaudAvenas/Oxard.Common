using System;
using System.Reflection;

namespace Oxard.Common.Extensions
{
    public static class AssemblyExtensions
    {
        public static void AssemblyParse(this object instanceInAssembly, Func<Type, bool> typeFilter, Action<Type> parseAction, Assembly assemblySource = null)
        {
            assemblySource = assemblySource ?? instanceInAssembly.GetType().GetTypeInfo().Assembly;
            assemblySource.Parse(typeFilter, parseAction);
        }

        public static void Parse(this Assembly assemblySource, Func<Type, bool> typeFilter, Action<Type> parseAction)
        {
            foreach (var typeInfo in assemblySource.DefinedTypes)
            {
                var type = typeInfo.AsType();
                if (typeFilter(type))
                    parseAction(type);
            }
        }
    }
}
