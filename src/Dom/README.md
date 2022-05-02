This package contains a standalne and self-managed TypeScript DOM system.
You can use this package to programatically create a TypeScript file with types you want, and render contents to file system or console.

The following C# code demonstrate basic using of thie DOM.
```C#
using Nabla.TypeScript;

namespace Test;

public static class Program {

    public static Main() {
        var module = CreateSharedModule();
        PrintModule(module);
    }

    static TypeFile CreateSharedModule()
    {
        TypeFile module = new("Shared");

        var param = new TypeParameter("T");

        module.Add(TS.Alias("Nullable", TS.Union(param.Reference(), TS.Native.Null(), TS.Native.Undefined())));
        module.Add(TS.Interface("Tuple2", null,
            TS.Property("Item1", TS.Parameter("T1").Reference()),
            TS.Property("Item2", TS.Parameter("T2").Reference(), isOptional: true)));

        return module;
    }

    private static void PrintModule(TypeFile module)
    {
        Console.WriteLine($"/** {module.Name}.ts **/");
        TypeWriter writer = new(Console.Out, new(), false);
        module.Write(writer);
        Console.WriteLine();

    }

}
```
Above code produces TypeScript code like below.
```TS
/* Shared.ts */
export type Nullable<T> = T | null | undefined;

export interface Tuple2<T1, T2> {
    Item1: T1,
    Item2?: T2
}
```