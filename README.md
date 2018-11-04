# Injection Demo

A demo project outlining how to inject C++/C# dlls into remote processes. Supports both x86 and x64 injection.

## How It Works

The project contains two demo libraries to be injected: LibraryPlusPlus (C++) and LibrarySharp (C#). Each library implements and exports a 'Run' function that expects a single string parameter. These functions are exported using the stdcall implementation which has the added benefit that the callee cleans up the stack before returning. Each of the demo libraries is setup to open a message box with the current process id and argument passed before returning.

The project contains an Example console application to be injected into and an Injecter library to exectue the demo. For a high level overview of the execution:

1. Load requested library into process (using CreateRemoteThread injection)
2. Find offset of 'Run' function in process
3. Call 'Run' function in process passing argument (using CreateRemoteThread again)


## Issues

If you see build error 'Error The RGiesecke.DllExport.MSBuild.DllExportAppDomainIsolatedTask task could not be loaded from the assembly' then install ```Microsoft .NET Framework 3.5```.

## License

MIT
