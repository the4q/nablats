
# Type discovery options

`-p`, `--project`

Specify the path to a _.csproj_ file. Nablats can resolve the output assembly and use it as souce to discover .NET types. 
If this argument is omitted, nablats will try to find a _.csproj_ file in current directory.

`-f`, `--configuration`

Name of project configuration to resolve output assembly. Default value is `Debug`.

`-a`, `--assembly`

Path to the .NET assembly to discover source types. If specified, the `--project` argument will be ignored.

`-s`, `--strategy`

Specify how to discover types in the source assembly. Possible values are `Auto`_(DEFAULT)_, `Annotation`, `WebApi`. See [Type discovery](./Manual.md#type-discovery) section for details.

`-S`, `discoverer`

CLR type name of custom discoverer that your want to use for source type discovery. If the discoverer resided in the souce assembly, a full name is satisfied, like `MyApp.Models.TypeDiscoverer`. Otherwise, you may provide a assembly qualified type name _(not tested yet)_.

# Code generating options

`-o`, `--output`

Path to the directory which you want the output TypeScript files to be stored.
If this argument is omitted, current directory is used.

> **CAUTION**
> 
> If you omit both `--project` and `--output` option, the output file will reside in server side directory. This is pointless, avoid doing this, unless you're intend to.

`-r`, `--arrange`

Specify how the generated TypeScript files should be arranged. Possible values are `Explicit`_(DEFAULT)_, `Natural`, `Namespace`, `Single`. See [File arrangement](Manual.md#file-arrangement) section for details.

`-m`, `--file-name`

Default file name for `Explicit` or `Single` arrangement. If omitted, the source assembly name will be used.

`-n`, `--namespace`

Generate code in [TypeScript namespaces](https://www.typescriptlang.org/docs/handbook/namespaces.html) mode rather than Node modules.

# Type handling options

`-c`, `--camel-case`

Use camel-case for properties or enum members. For enum types, this option can be overrided by `TsEnumHandlingAttribute`.

`-d`, `--date`

Specify how date/time types be mapped. Possible values are `String`_(DEFAULT)_, `Date`, `Number`. See [date type handling](Manual.md#date-type) section for details.

`-e`, `--enum`

Specify how enum types be mapped. Possible values are `Number`_(DEFAULT)_, `Object`, `Const`, `Union`. See [enum type handling](Manual.md#enum-type) section for details.

`-u`, `--no-nullable`

Disable nullables. See [nullability handling](Manual.md#nullability-handling) section for details.

`--maping`

Path to an external XML type mapping file. See [type mapping](Manual.md#external-type-mapping) section for details.

# Code styling options

`-P`, `--compact`

Compact output file by eliminating blank lines.

`-D`, `--double-quote`

Use double-quote for string literals.

`-I`, `--indent-size`

Number of spaces to use for indent, will be ignored when using tab indent. Default value is 4.

`-T`, `--tab-indent`

Use tab char `\t` instead of spaces for indent.

`-C`, `--semicolon`

Add semicolons if necessary.

`--header` \| `--footer`

Path to a file which you want its content been embeded into top/bottom of each output file. See [Content injection](Manual.md#content-injection) section for details.

# Behavior control options

`-w`, `--watch`

Watches source assembly and automatically update output files when changed. See [Assembly watching](Manual.md#assembly-watching) section for details.

`--dry-run`

Print generated code to console rather than writing to files.

`--silent`

Do not print any information to console, except generated code with `--dry-run` option set. In silent mode, any prompt will be accepted by default option.

# Letter casing

* Option names are case sensitive.
* Values of enum like option, e.g. `-s`, `-r`, etc, are case insensitive.
* Values of CLR type name option are case sensitive.
* Values of path option depend on the operation system.