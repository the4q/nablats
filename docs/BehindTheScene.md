For SPA programing, developers have to write same code of data models for both server and client side. This is absolutely nightmare, specially for full stack developers, like me. 

Couple of weeks ago, a thought just came to my mind, why don't I develope a tool to convert .NET types to TypeScript automatically. This is where things getting started.

About the name. **Nabla** is the code name of a project which containts sets of utilities that I have developed and been using for years. It is shame that I cannot recall the meaning of this word any more. By combining _nabla_ and _ts_, short for TypeScript, the tool name cames up with **Nablats**, and it looks pretty natural for me.

TypeScript namespace is not an original part of the DOM system design, it came to me after the whole thing completed. Adding namespace support is harder than I have imaged.

Nablats is written in C# and built with .NET 6. It uses a few new features shipped with .NET 6, so that the source code may not be build under previous versions of .NET, unless you make slight changes to the source code yourself.

There're still two kinds of work should be done.
* Document the whole project.
* Unit test everything.

I am not sure that I can get them done all by myself, it's massive work to do. If anyone found interesting in this project, maybe you can help.