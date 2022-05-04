# Table of conent

- [Table of conent](#table-of-conent)
- [Workflow](#workflow)
- [Sample project](#sample-project)
- [Type discovery](#type-discovery)
  - [Annotation discovery](#annotation-discovery)
  - [WebApi discovery](#webapi-discovery)
  - [Custom discovery](#custom-discovery)
  - [Discovery options](#discovery-options)
  - [Examples](#examples)
- [Type handling](#type-handling)
  - [Enum type](#enum-type)
  - [Date type](#date-type)
  - [Variant handling](#variant-handling)
  - [Name handling](#name-handling)
  - [Nullability handling](#nullability-handling)
  - [Special CLR types](#special-clr-types)
  - [TypeScript tuple](#typescript-tuple)
  - [Type overriding](#type-overriding)
  - [External type mapping](#external-type-mapping)
- [File arrangement](#file-arrangement)
- [Namespace support](#namespace-support)
- [Content injection](#content-injection)
- [Assembly watching](#assembly-watching)

# Workflow

Nablats generating TypeScript code by the following three steps.

1. Discovers types in the source assembly which you specified.
2. Constructs DOM tree to represent these types and their relationship.
3. Writes all nodes within the tree to output.

# Sample project

In the [Sample](../Sample/server) folder, there is a project created for demonstration. Following contents of this document base on this project.

Mainly used data models is defined as below.

```C#
/* WeatherForecast.cs */
public class WeatherForecast
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; set; }
    public WeatherKind Kind { get; set; }
}

/* WeatherKind.cs */
public enum WeatherKind
{
    Sunny,
    Cloudy,
    LightRain = 10,
    MediumRain,
    HeavyRain,
    LightSnow = 20,
    MediumSnow,
    HeavySnow
}

/* WeatherReporter.cs */
[TsExport]
public class WeatherReporter
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string[]? Schedules { get; set; }
    public DayOfWeek[]? Workdays { get; set; }
    public WeatherKind? FavoriteWeather { get; set; }
}
```

# Type discovery

Nablats currently provides type discovery by reflection mechanism only. There're three strategies for you to tell nablats how to discover types within the assembly.

## Annotation discovery

Nablats serarches type by `Nabla.TypeScript.Tool.TsExportAttribute`. That means you must explicitly define this attribute to each type that exposed directly to the client. Types that referenced by exposed types can be discovered automatically, no need to define attribute for them.

To use annotation discovery, include `Nabla.TypeScript.Tool.Annotations` package in your project, then introduce `Nabla.TypeScript.Tool.TsExportAttribute` to the classes you want be found.

## WebApi discovery

Nablats has the ability to discover POCO types in WebApi pattern. In this way, package `Nabla.TypeScript.Tool.Annotations` is necessary, unless you want to do some customization works.

WebApi pattern has following rules.

-   Assembly contains a reference to `Microsoft.AspNetCore.Mvc`.
-   Assembly contains at least on API controller. The controller type must be derieved from `Microsoft.AspNetCore.Mvc.Controller` and has `Microsoft.AspNetCore.Mvc.ApiControllerAttribute` defined.
-   Controller should contains one or more actions. Action is a public method, and must have `Microsoft.AspNetCore.Mvc.RouteAttribute` or attribute derieved from `Microsoft.AspNetCore.Mvc.HttpMethodAttribute` defined.

Nablats walks each action's parameters and return type to find out exposed types. Know wrapper type, `System.Threading.Tasks.Task<>`, `Microsoft.AspNetCore.Mvc.ActionResult<>`, can be unwrapped to prevent unexpcted result. Element type of dictionary or collection types can also be walked.

Not all types used by actions would be discovered. Nablats checks to see whether a type is POCO type. A POCO type requires a class contains one public parameterless constructor, or declared as a `record` type. If type does not meets this constraint, it will be ignored.

## Custom discovery

To use custom discovery, introduce a `Nabla.TypeScript.Tool.TsTypeDiscovererAttribute` to your assembly and provide the custom discoverer type as argument.
A custom discoverer must implement `Nabla.TypeScript.Tool.ITypeSourceDiscoverer` interface.

```C#
public interface ITypeSourceDiscoverer
{
    IEnumerable<object> Discover();
}
```

This interface only has one method `Discover`. Despite the return type is `IEnumerable<object>`, you must return `System.Type` as the iteration element, or an exception will throw.

## Discovery options

The command line option `--strategy` is for you to choose the discovery stategy you want. It has three possible values, `Annotation` or `WebApi` enforces nablats to use corresponding strategy. For `Auto` strategy, nablats will first try to find custom discoverer you might defined, then determine whether the assembly is in WebApi pattern, finally fallback to annotation strategy.

The `--discoverer` option has the priority to override this mechanism. If sepcified, the `--strategy` options will be ignored.

## Examples

As for the sample project, different discovery strategy produces different output.

With annotation strategy, only the `WeatherReporter` class is defined by `TsExportAttribute`. So the output contains this class only.

```TS
export interface WeatherReporter  {
    Id: number,
    Name: string,
    Schedules?: string[],
    Workdays?: number[],
    FavoriteWeather?: number
}
```

In WebApi discovery, the `WeatherForecast` class is referenced by MVC action `WeatherForecaseController.Get` as its return value, and `WeatherReporter` is not. So the output contains `WeatherForecase` only.

```TS
/* MyApp.ts */
export interface WeatherForecast  {
    Date: string,
    TemperatureC: number,
    TemperatureF?: number,
    Summary?: string,
    Kind: number
}
```

Although the `WeatherKind` enum is referenced by both classes, still it does not appear in the output. This is because nablats convert it to `number` type not the enum type itself. See [enum handling](#enum-type) for more detail.

# Type handling

Nablats is not able to detect the actual type that data models been serialized to the client. For some specific types, like enum, DateTime, etc, you may need to tell nablats how to convert types.

## Enum type

For enum types, nablats provides three ways to generate code. You can choose which way to apply by command-line argument `--enum`, or introduce `TsEnumHandlingAttribute` to a enum type for specific type or to assembly for all types.

The `Number` handler converts enum types to `number` type, this is the default option.

The `Object` and `Const` handlers convert CLR enum types to TypeScript enum type. `Const` handler generates enum type as const enum. For more information of the difference, see the [official document](https://www.typescriptlang.org/docs/handbook/enums.html#const-enums).

```TS
export enum WeatherKind
{
    Sunny = 0,
    Cloudy = 1,
    LightRain = 10,
    MediumRain = 11,
    HeavyRain = 12,
    LightSnow = 20,
    MediumSnow = 21,
    HeavySnow = 22
}
```

The `Union` handler converts enum types as name-only union type. for `WeatherKind` type, the generated code looks like below.

```TS
export type WeatherKind = 'Sunny' | 'Cloudy' | 'LightRain' | 'MediumRain' | 'HeavyRain' | 'LightSnow' | 'MediumSnow' | 'HeavySnow'
```

## Date type

The CLR provides five major date/time related types: `DateTime`, `DateTimeOffset`, `TimeSpan`, `DateOnly`, `TimeOnly`. Nablats converts them to `string` by default. In case you have a different way to map these types to other types, nablats provides two extra options, `number` or JavaScript native `Date` type.

You can use command-line option `--date` or `TsDateHandlingAttribute` to explicitly specify them.

## Variant handling

If a property is declared as `object`, `JsonDocument` or `JsonElement`, nablats recoginze them as variant type, and convert to `unknown` by default. You have two ways to override this behavior.

The first way is to use type overriding mechanism, see [type overriding](#type-overriding) section for details.

The second way is lift this property to a type parameter. Introduce a `Nabla.TypeScript.Tool.TsVariantTypeHandlingAttribute` to the property, set `Handling` property to `VariantTypeHandling.AsGenericParameter`. Nablats convert the containing type to a generic type, the name of lifted type parameter will be "_property name_ + 'Type'".

## Name handling

By default, nablats does not change the name of type, property, or enum member. But if you want, you change customize names by command-line option or annotation attribute.

Type names can be specified by `Nabla.TypeScript.Tool.TsTypeNameAttribute`. You can specify whatever name you want for a type.

Property names are handled semi-automatically. JSON serialization technolgy provides you a way to explictly specify property name, e.g. `System.Text.Json.Serialization.JsonPropertyNameAttribute`. Nablats can infer names from these attributes, this is the automatic part.

For other properties, nablats does not change the name by default. If you want to apply camel-case naming strategy, use `--camel-case` option in the command-line.

Name of enum type members are treated like property name, but don't do the automatic part. That means the `--camel-case` option can also apply on enum members. If you want to control naming strategy for specific types, you can introduce `Nabla.TypeScript.Tool.TsEnumHandlingAttribute` to your emnu type, and set a value to its `NamingPolicy` property.

To demonstrate the result of name handling described above, make some changes to the source code of MyApp project.

```C#
/* WeatherKind.cs */
using Nabla.TypeScript.Tool;

namespace MyApp;

// Declare this enum type as a const enum, and keep all member names unchanged.
[TsEnumHandling(EnumHandling.Const, NamingPolicy = PropertyNamingPolicy.Unchanged)]
public enum WeatherKind
{
    Sunny,
    Cloudy,
    // ... other members
}

/* WeatherReporter.cs */
using Nabla.TypeScript.Tool;
using System.Text.Json.Serialization;

namespace MyApp;

[TsExport]
// Convert type name to 'Reporter' rather than the orignal name.
[TsTypeName("Reporter")]
public class WeatherReporter
{
    // ... other properties

    // Explicitly define the output property name.
    [JsonPropertyName("favorite")]
    public WeatherKind? FavoriteWeather { get; set; }
}
```

Now run nablats with `--camel-case` option, it preoduces the following result.

```TS
/* MyApp.ts */
export const enum WeatherKind {
    Sunny = 0,
    Cloudy = 1,
    // ... other members
}

export interface Reporter  {
    // ... other properties
    favorite?: WeatherKind
}
```

## Nullability handling

As shown above, nullable properties are converted with a question mark automatically. By taking advantage from .NET nullability technolgy since .NET 6.0, nablats can determine whether a property can be null.

Nablats handles nullable refernece differently. Nullable type directly referenced by property is simply add a question mark to property name. For indirect references, nablats introduces a built-in utility type `OrNull<Type>`, declared as below.

```TS
export type OrNull<Type> = Type | null;
```

Lets make a little change to `WeatherReporter`, just mark the element type of `Schedules` property as nullable.

```C#
[TsExport, TsTypeName("Reporter")]
public class WeatherReporter
{
    // ...
    // other properties

    public string?[]? Schedules { get; set; }
}
```

Now the output will be like this.

```TS
import type { OrNull } from './NablatsUtilities'

export interface Reporter  {
    // ...
    // other properties

    schedules?: OrNull<string>[],
}
```

If you don't want to use the utility type `OrNull<Type>`, you can specify the command-line option `--no-nullable`.

## Special CLR types

There're three kinds of special types, dictionaries, collections, tuples.

Dictionary type means type which implements `System.Collection.Generic.IDictionary<,>` interface. Nablats convert them to TypeScript utility type `Record<Keys, Type>`.

Collection type means type which implements `IEnumerable<>` or `IAsyncEnumerable<>` of `System.Collection.Generic` namespace, except `System.String`. Nablats converts them to arrays.

Tuple type refer to `System.Tuple<>` or `System.ValueTuple<>`. Nablats coverts theme to a user-defined interface type `TupleN<>`, where the `N` is the count of generic arguments.

Using of these sepcial types has some limitations. See [limitation guid](./Limitations.md) for details.

## TypeScript tuple

The [TypeScript tuple types](https://www.typescriptlang.org/docs/handbook/2/objects.html#tuple-types) is different than the CLR tuple types. It is declared by an array like expression, and supports destruction. Nablats won't convert CLR tuples to TypeScript tuples due to the default behavior of JSON serialization.

If you want to use TypeScript tuples, you have two ways to achieve that.

-   Use `Nabla.TypeScript.Tool.TsTupleAttribute` to define a type as tuple type.
-   Use type overriding attribute on properties. See [type overriding](#type-overriding) section for details.

If a type is defined with `TsTupleAttribute`, nablats converts it to a tuple type. Type parameters inferred from properties. If you want to change the order of type parameters, you can simply change the appearance order of properties in source code, or introduce a `TsTupleOrderAttribute` to certain properties.

Now lets add a new type `Location` to MyApp, and reference by the `WeatherForecast` class.

```C#
/* Location.cs */
using Nabla.TypeScript.Tool;

namespace MyApp;

[TsTuple]
public struct Location
{
    [TsTupleOrder(3)]
    public string Address { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
}

/* WeatherForecast.cs */
public class WeatherForecast
{
    // ... other properties

    public Location? Location { get; set; }
}

```

The output should be like this.

```TS
export type Location = [number, number, string]

export interface WeatherForecast  {
    // ... other properties
    Location?: Location,
}
```

## Type overriding

If above type handling mechanisms do not satisfy your needs, you can use type overriding to convert properties to any type you want. The `Nabla.TypeScript.Tool.TsTypeOverrideAttribute` can be used to achieve that purpose.

If you want the property be overrided as a primitive or utility type, provide type name as the first argument of `TsTypeOverrideAttribute`, and supply appropriate type parameters.

Otherwise, if you want to use a custom type, you must specify the `ModuleName` property as well. Nablats assumes that you have a type declared in the module already.

To use a TypeScript tuple, you just assign `TypeConstants.TypeScriptTuple` to the `typeName` argument, and leave `ModuleName` property null. In this case, type parameters must be supplied, or a error will occured while converting.

The type parameter is supported in three types, `string`, `Nabla.TypeScript.Tool.TypeScriptPrimitive` enum, or `System.Type`. For `string` parameter, nablats use it directly, you must be sure that the type name is correctly supplied. Other types can be converted in way nablats does.

If you want the overriden type been in array form, just assign a value to `ArrayDepth` property.

Lets add a property to `WeatherForecast` class to demonstrate.

```C#
public class WeatherForecast
{
    // ... other properties

    [TsTypeOverride("Tuple", typeof(int), "string | undefined", ArrayDepth = 1)]
    public object? OverrideDemo { get; set; }

}
```

The output will be like this.

```TS
export interface WeatherForecast {
    // ... other properties

    OverrideDemo?: [number, string | undefined][]
}
```

## External type mapping

Since version 1.2.0, nablats provides external type mapping feature.

In case you don't like referencing the runtime irrelevant package `Nabla.TypeScript.Tool.Annotations`, you can now use a external mapping file to specify how you want to override the default behavior of converting types.

The mapping file is in XML format, you can find the schema [here](../src/Core/Mapping/Mapping.xsd). Using of this file is similar to annotation attributes. 

The following example demonstrate the equivalent to the previous section.

```XML
<Mapping xmlns="http://the4q.com/schemas/nablats/mapping">
    <Type Source="MyApp.WeatherForecast">
        <Property Source="OverrideDemo">
            <Overriding TypeName="Tuple" ArrayDepth="1">
                <Parameter Kind="Clr" Value="System.Int32" />
                <Parameter Value="string | undefined" />
            </Overriding>
        </Property>
    </Type>
</Mapping>
```

To use a external mapping file, specify the command-line option `--mapping`.

# File arrangement

The command-line option `--arrange` specifies the strategy nablats organizes types in .ts file. There're four strategies you can use.

In `Explicit` strategy names the .ts file by the assembly name which the source type declared, by default. To use a different name, you introduce a `TsFileNameAttribute` to certain type or assembly. All types that has not been explicitly specified file name, will remain in the .ts file which reference them.

The `Nature` strategy is similar to `Explicit`. The difference is the types that has no `TsFileNameAttribute` defined will remain in the .ts file corresponding to its container assembly.

The `Single` strategy produces one single .ts file contains all types that discovered or referenced, including the built-in utility types.

The `Namespace` strategy names file by the namespace of source type.

The command-line option `--filename` applies on `Explicit` and `Single` strategy. It has the priority to override the first found `TsFileNameAttribute`.

# Namespace support

In large project, name of data models may not be identical, thus pack them into one single .ts file is not possible. Thanks for the [TypeScript namespace](https://www.typescriptlang.org/docs/handbook/namespaces.html) mechanism, we can do it now.

The command-line option `--namepsace` instructs nablats to generate output file as namespaces rather than typical ECMAScript modules. A namespaced .ts file contains neither `import` nor `export` keyword in the top level, referencing a external namespaced .ts file can be done by using `reference` directive.

```TS
namespace MyApp {
    export const enum WeatherKind {
        Sunny = 0,
        Cloudy = 1,
        LightRain = 10,
        MediumRain = 11,
        HeavyRain = 12,
        LightSnow = 20,
        MediumSnow = 21,
        HeavySnow = 22
    }

    export type Location = [number, number, string]

    export interface WeatherForecast  {
        Date: string,
        TemperatureC: number,
        TemperatureF?: number,
        Summary?: string,
        Kind: MyApp.WeatherKind,
        Location?: MyApp.Location,
        OverrideDemo?: [number, string | undefined][]
    }

}
```

# Content injection

Nablats provides a feature that let you embed custom content in each generated .ts file, at the top or bottom. Use command-line option `--header` or `--footer` to specify the path to files which you want their to content to be embeded.

In the source content file, you can you varialbes that nablats provided. This is useful when the content is not static. The format of varialbe declaration is same to JavaScript template literals, like `${var_name}` Nablats provides the following variables.

| Variable name | Description                                             | Example                         |
| ------------- | ------------------------------------------------------- | ------------------------------- |
| fileName      | The output filename.                                    | MyApp.ts                        |
| filePath      | The absolute path to the output file.                   | c:\MyApp\client\models\MyApp.ts |
| moduleName    | The output filename without extension.                  | MyApp                           |
| outputPath    | The absolute path to the output directory.              | c:\MyApp\client\models          |
| outputDir     | The directory name of output.                           | models                          |
| date          | The date, in short format, that the file generated.     | 2022-5-1                        |
| time          | The time, in long format, that the file generated.      | 00:24:32                        |
| dateTime      | Combination of `${date}` and `${time}`                  | 2022-5-1 00:24:32               |
| user          | The name of current user logged in to operation system. | nabla                           |
| machine       | The name of your computer you are using.                | MyLaptop                        |
| version       | The version of nablats you are using.                   | 1.1.0                           |

In addition to the above variables, you can also use evnironment varialbes. Just introduce them by `env_` prefix. For example, you have a varialbe named `${env_SystemRoot}`, it may produce a value like `C:\Windows`.

# Assembly watching

The command-line option `--watch` instructs nablats to continuously running, and watch the source assembly. When source assembly changed, update the output files automatically.

Nablats won't directly write the output file, it only writes file when its content changed. So, don't worry about unnecesary recompling/repacking of frontend tools.
