using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabla.TypeScript.Tool.Reflection
{
    public abstract class ReflectionModuleNameResolver : IFileNameResolver
    {
        private static T Do<T>(object source, Func<Type, T> func)
        {
            if (source is Type type)
                return func(type);

            throw new ArgumentException("The source object must be System.Type.");

        }
        public abstract string ResolveModuleName(Type type);

        public static string? ResolveNamespace(Type type)
        {
            return type.Namespace;
        }

        string IFileNameResolver.ResolveFileName(object source)
        {
            return Do(source, ResolveModuleName);
        }


        string? IFileNameResolver.ResolveNamespace(object source)
        {
            return Do(source, ResolveNamespace);
        }
    }

    public sealed class AssemblyModuleNameResolver : ReflectionModuleNameResolver
    {
        public override string ResolveModuleName(Type source)
        {
            return source.Assembly.GetName().Name ?? throw new InvalidOperationException("Cannot retrieve assembly name.");
        }

    }

    public sealed class NamespaceModuleNameResolver : ReflectionModuleNameResolver
    {
        public override string ResolveModuleName(Type type)
        {
            return type.Namespace ?? throw new CodeException("No namespace associated with type " + type.FullName);
        }
    }

}
