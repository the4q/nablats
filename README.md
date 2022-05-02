This repository contains the code for `nablats` tool and its dependencies.

Nablats is a command-line tool for converting .NET types to TypeScript types. It can discover .NET types in assembly, map types to built-in TypeScript DOM, and generate corresponding `.ts` files.

# Getting started
## Installation

Nablats is provided as an .NET tool. You can simply install it by `dotnet tool` command.

```PS
dotnet tool install -g nablats
```

After installation completed, type `nablats` or `dotnet ts` command to use.

## Usage

```PS
nablats [options]
```

## Examples

```PS
# Generate TypeScript files from the project in current directory
nablats -o ../client/app/models

# Generate TypeScript files into current directory from a specific project
nablats -p ../server/MyApp.csproj

# Generate TypeScript files into current directory from a specific assembly
nablats -a ../server/bin/Debug/net6.0/MyApp.dll

# Generate TypeScript code in camel-case and produces one single file
nablats -p ../server/MyApp.csproj -r single -c

# Generate code in custom style: double quote, semicolon
nablats -p ../server/MyApp.csproj -D -C

# Watch source assembly and automatically update generated files when changed
nablats -p ../server/MyApp.csproj -w

# Send generated code to console rather than writing files
nablats -p ../server/MyApp.csproj --dry-run
```

For more details of usage, please see [options](docs/Options.md) or [manual](docs/Manual.md) guide.

# Packages

|Package|Version|Description|
|-------|-------|-----------|
|Nabla.TypeScript.Dom||Standalone TypeScript document object model.|
|Nabla.TypeScript.Tool||Core library of nablats.|
|Nabla.TypeScript.Tool.Reflection||Provides .NET reflection support|
|Nabla.TypeScript.Tool.Annotations||Annotation attributes and basic types.|
|nablats||Executable of this tool.|

# Documentation

## Options

* [Type discovery](docs/Options.md#type-discovery-options)
* [Code generating](docs/Options.md#code-generating-options)
* [Code styling](docs/Options.md#code-styling-options)
* [Behavior control](docs/Options.md#behavior-control-options)
* [Letter casing](docs/Options.md#letter-casing)

## Features

*   [.NET type discovery.](docs/Manual.md#type-discovery)
*   [Built-in type converting strategies.](docs/Manual.md#type-handling)
*   [Customizable data type mapping.](docs/Manual.md#type-overriding)
*   [Customizable naming strategy for type/property names.](docs/Manual.md#name-handling)
*   [Nullability information detection and mapping.](docs/Manual.md#nullability-handling)
*   [Four ways to arrange generated TypeScript files.](docs/Manual.md#file-arrangement)
*   [TypeScript namespace support.](docs/Manual.md#namespace-support)
*   [Additional header or footer contents can be embeded into generated files, with variable support.](docs/Manual.md#content-injection)
*   [Watches input assembly change and update generated code automatically.](docs/Manual.md#assembly-watching)
*   Flexible architecture allows you to extend this tool or combine with your own software.

## Limitations

* [Dictionary types](docs/Limitations.md#dictionary-types)
* [Array like types](docs/Limitations.md#array-like-types)
* [Tuple types](docs/Limitations.md#tuple-types)
* [Framework support](docs/Limitations.md#framework-support)
* [Resolving _.csproj_ file](docs/Limitations.md#resolving-csproj-file)
* [Built-in TypeScript DOM](docs/Limitations.md#built-in-typescript-dom)

# License

This project is licensed under the [MIT license](LICENSE.md).