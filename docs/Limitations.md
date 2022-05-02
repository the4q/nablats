While developing this software, I found some limitations converting .NET types to TypeScript.
Maybe it is just limited by the knowledge of TypeScript I have learned. Hope sometime or someone can solve them.

# Dictionary types

Any type implements `System.Collection.Generic.IDictionary<,>` interface will be considered as dictionary type. Dictionary type produces [`Record<Keys, Type>`](https://www.typescriptlang.org/docs/handbook/utility-types.html#recordkeys-type) type only. They have follwing limitations.
*   Any property on dictionary type will be ignored due to TS2411.
*   Key type must be primitive type, e.g. string, int, etc.
*   For custom dictionary types, the first generic argument passed to underlying `IDictionary<,>` interface must **NOT** be a generic parameter of custome type.

# Array like types

Any type implements `System.Collection.Generic.IEnumerable<>` or `System.Collection.Generic.IAsyncEnumerable<>`, except `System.String`, will be considered as array like type. Array like type produces [`Array<Type>`](https://www.typescriptlang.org/docs/handbook/2/everyday-types.html#arrays) only. They have following limitations.
*   Any property on array like type type will be ignored.
*   Multi-dimesion array, like `int[,]`, is not supported.

# Tuple types
The CRL provides two kinds of tuple, `System.Tuple<>` and `System.ValueTuple<>`. 
The value tuple is weird, `System.Text.Json` serializs them as an empty object, and either do I.

Try avoid using this technolgy to expose data to the client. If you intend to use them, use `System.Tuple<>` instead of `System.ValueTuple<>`, nablats can map them into a interface.

# Framework support
Nablats is developed and build with .NET 6. I'm not sure that it can support other verions. But I think you can download the source code and build it against your platform.

# Resolving _.csproj_ file
Nablats can resolve assembly path from _.csproj_ file, but this implementation is naive. The `Microsoft.Build` package provides a way to do the full thing, but I found difficulty firing it up, then finally I gave up.

As the implementation is quite simple, there're some restrictions to your project file. If these restrictions conflic with your project settings, use `--assembly` option instead.

*  Project can only have one target framework, and must be explicitly specified.
*   Condition evaluation is not supported.
*   Both `OutputPath` and `OutDir` properties are supported, any other output related properties are not.
*   Only one macro is supported in output property values: `$(Configuration)`.

# Built-in TypeScript DOM

Although the self managed DOM system provides a few validations of type constraint, but I still cannot guarantee whether the generated code is correct. This may relies on external tools like IDE or compiler. If something wrong, please report it.

This DOM system currently support TypeScript file in pure ECMAScript module or namespace mode. Mixing them up is not supported.

# JSON serialization

Nablats assumes that you are using `System.Text.Json` as the JSON serilization solution for web APIs. Other technolgies is not supoorted yet, like Newtonsoft.Json.