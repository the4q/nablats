using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nabla.TypeScript.Tool.Reflection
{
    public static class ReflectionExtensions
    {
        private static T? GetCustomAttribute<T>(this MemberInfo member, bool? inherit)
            where T : Attribute
        {
            if (inherit.HasValue)
                return CustomAttributeExtensions.GetCustomAttribute<T>(member, inherit.Value);
            else
                return member.GetCustomAttribute<T>();
        }

        public static T? CascadeGetCustomAttribute<T>(this MemberInfo member, bool? inherit = null)
            where T : Attribute
        {
            return member.GetCustomAttribute<T>(inherit) ?? member.DeclaringType?.CascadeGetCustomAttribute<T>(inherit);
        }

        public static T? CascadeGetCustomAttribute<T>(this Type type, bool? inherit = null)
            where T : Attribute
        {
            return type.GetCustomAttribute<T>(inherit) ?? type.Assembly.GetCustomAttribute<T>();
        }

        public static Type GetPropertyOrFieldType(this MemberInfo member)
        {
            if (member is PropertyInfo pi)
                return pi.PropertyType;

            if (member is FieldInfo fi)
                return fi.FieldType;

            throw new InvalidOperationException($"The specified member is not a property or field: {member.DeclaringType?.FullName}.{member.Name}");
        }

    }
}
