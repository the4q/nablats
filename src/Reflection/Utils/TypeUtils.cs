using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Nabla.TypeScript.Tool.Reflection;

internal static class TypeUtils
{

    public static bool IsNullable(MemberInfo member, out Type underlyingType)
    {
        var memberType = member.GetPropertyOrFieldType();

        if (memberType.IsValueType)
        {
            var ut = Nullable.GetUnderlyingType(memberType);

            underlyingType = ut ?? memberType;
            return ut != null;
        }

        underlyingType = memberType;

        NullabilityInfo info;
        NullabilityInfoContext context = new();

        if (member is PropertyInfo pi)
            info = context.Create(pi);
        else
            info = context.Create((FieldInfo)member);

        return info.ReadState != NullabilityState.NotNull && info.WriteState != NullabilityState.NotNull;
    }

    private static Type[]? GetGenericArguments(Type type, params Type[] ifDefs)
    {
        //if (!type.IsGenericType)
        //    return null;

        ////if (type.IsGenericTypeDefinition)
        ////    throw new InvalidOperationException("Must not be a generic type definition.");

        //var def = type.GetGenericTypeDefinition();
        //var ifs = type.GetInterfaces();

        ////Type? ExtractInterfaceType(Type ifDef)
        ////{
        ////    if (ifDef == def)
        ////        return type;

        ////    return ifs.Where(x => x.GetGenericTypeDefinition() == ifDef).FirstOrDefault();
        ////}

        //var ifType = ifNames.Select(x => def == x ? type
        //        : ifs.Where(y => y.IsGenericType && y.GetGenericTypeDefinition() == x).FirstOrDefault())
        //    .Where(x => x != null)
        //    .FirstOrDefault();

        //if (ifType != null)
        //{
        //    return ifType.GetGenericArguments();
        //}

        //return null;

        var ifTypes = type.GetInterfaces().Append(type);

        foreach (Type? ifType in ifTypes)
        {
            if (ifType.IsGenericType)
            {
                for (int j = 0; j < ifDefs.Length; j++)
                {
                    Type? ifDef = ifDefs[j];

                    if (ifType == ifDef)
                    {
                        return ifType.GetGenericArguments();

                    }
                    else if (ifType.GetGenericTypeDefinition() == ifDef)
                        return ifType.GetGenericArguments();

                }
            }
        }

        return null;
    }

    public static bool IsGenericDictionary(Type type, [NotNullWhen(true)] out Type? keyType, [NotNullWhen(true)] out Type? valueType)
    {
        var paras = GetGenericArguments(type, typeof(IDictionary<,>));

        if (paras != null)
        {
            keyType = paras[0];
            valueType = paras[1];

            return true;
        }

        keyType = null;
        valueType = null;
        return false;
    }

    public static bool IsCollection(Type type, [NotNullWhen(true)] out CollectionInfo? collectionInfo)
    {
        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            Debug.Assert(elementType != null);

            collectionInfo = new(true, elementType);

            return true;
        }

        if (type != typeof(string))
        {
            var arguments = GetGenericArguments(type, typeof(IEnumerable<>), typeof(IAsyncEnumerable<>));

            if (arguments != null)
            {
                collectionInfo = new(!type.IsGenericType, arguments[0]);
                return true;
            }
        }

        collectionInfo = null;
        return false;
    }

    public static Type GetTypeDefinition(Type type)
    {
        if (type.IsTypeDefinition)
            return type;

        if (type.IsGenericType)
            return type.GetGenericTypeDefinition();

        throw new ArgumentException($"{type.FullName} is not a type definition", nameof(type));
    }

    private static readonly Type[] _dateTypes = new[]
    {
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(TimeSpan),
        typeof(DateOnly),
        typeof(TimeOnly)
    };

    private static readonly Type[] _stringTypes = new[]
    {
        typeof(Guid)
    };

    public static TypeScriptPrimitive? GetPrimitiveType(Type clrType)
    {
        var typeCode = Type.GetTypeCode(clrType);

        switch (typeCode)
        {
            case TypeCode.Empty:
            case TypeCode.DBNull:
                return TypeScriptPrimitive.Never;
            case TypeCode.Boolean:
                return TypeScriptPrimitive.Boolean;
            case TypeCode.Char:
            case TypeCode.String:
                return TypeScriptPrimitive.String;
            case TypeCode.DateTime:
                return TypeScriptPrimitive.Date;
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
                return TypeScriptPrimitive.Number;
        }

        if (_dateTypes.Contains(clrType))
            return TypeScriptPrimitive.Date;

        if (_stringTypes.Contains(clrType))
            return TypeScriptPrimitive.String;

        return null;
    }

    public static Type? GetBaseType(Type clrType)
    {
        if (clrType.IsValueType)
            return null;

        var baseType = clrType.BaseType;
        
        if (baseType != typeof(object))
            return baseType;

        return null;
    }

    public static Type? WalkInheritance(Type type, Func<Type, bool> predicate)
    {
        if (predicate(type))
            return type;

        var baseType = GetBaseType(type);

        if (baseType == null)
            return null;

        return WalkInheritance(baseType, predicate);
    }

    public static bool IsTypeOrSubclass(Type type, string fullName)
    {
        return WalkInheritance(type, t => t.FullName == fullName) != null;
    }

    public static bool IsCustomAttributeDefined(MemberInfo member, string attrTypeName)
    {
        return member.GetCustomAttributes()
            .Any(x => IsTypeOrSubclass(x.GetType(), attrTypeName));

    }

    public static bool IsTuple(Type type, [NotNullWhen(true)]out Type[]? arguments)
    {
        if (type.GetInterfaces().Contains(typeof(ITuple)) && type.IsGenericType)
        {
            arguments = type.GetGenericArguments();
            return true;
        }

        arguments = null;
        return false;
    }

    public static bool HasParameterlessConstructor(Type type)
    {
        return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .Any(x => x.GetParameters().Length == 0);
    }

    // https://stackoverflow.com/questions/63097273/c-sharp-9-0-records-reflection-and-generic-constraints
    public static bool IsRecordType(Type type)
    {
        return ((TypeInfo)type).DeclaredProperties
            .FirstOrDefault(x => x.Name == "EqualityContract")?
            .GetMethod?
            .GetCustomAttribute(typeof(CompilerGeneratedAttribute)) is not null
        && type.GetMethod("<Clone>$") is not null;
    }

    /// <summary>
    /// Creates an instance of specified type by given arguments.
    /// If not suitable constructor found, fallback to parameterless constructor.
    /// </summary>
    /// <param name="type">Type of instance.</param>
    /// <param name="arguments">Constructor arguments.</param>
    /// <returns>Returns a instance represents the specified type.</returns>
    /// <exception cref="InvalidOperationException">Throws when no suitable constructor or parameterless constructor found.</exception>
    public static object CreateInstance(Type type, params object[] arguments)
    {
        var ctors = type.GetConstructors();
        ConstructorInfo? pctor = null;

        foreach (var ctor in ctors)
        {
            var paras = ctor.GetParameters();

            if (paras.Length == 0)
                pctor = ctor;

            if (paras.Length == arguments.Length)
            {
                bool ok = true;
                for (int i = 0; i < paras.Length; i++)
                {
                    var ptype = paras[i].ParameterType;
                    var atype = arguments[i].GetType();

                    if (!ptype.IsAssignableFrom(atype))
                    {
                        ok = false;
                        break;
                    }
                }

                if (ok)
                    return ctor.Invoke(arguments);
            }
        }

        if (pctor == null)
            throw new InvalidOperationException("No suitable constructor found.");
        
        return pctor.Invoke(null);
    }

}

