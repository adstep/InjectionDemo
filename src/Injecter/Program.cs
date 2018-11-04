﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Injecter.Utils;

namespace Injecter
{
    class Program
    {
        static void Main(string[] args)
        {
            var isSharp = true;
            var processName = "Code";

            var exampleExe = "Example.exe";

            var librarySharpModuleName = @"LibrarySharp.dll";
            var libraryPlusPlusPathModuleName = @"LibraryPlusPlus.dll";

            var moduleName = (isSharp) ? librarySharpModuleName : libraryPlusPlusPathModuleName;
            var modulePath = Path.Combine(Directory.GetCurrentDirectory(), moduleName);

            var process = Process.Start(exampleExe);

            try
            {
                Load(process, modulePath);
                Call(process, moduleName, "Run", "helloworld");
            }
            catch
            {
                // ignored
            }
            finally
            {
                process.Kill();
            }
        }

        private static void Load(Process process, string dllPath)
        {
            var loadLibraryFn = NativeExtensions.GetModuleFunction("kernel32.dll", "LoadLibraryA");

            var rawLoaderPath = Encoding.Default.GetBytes(dllPath);
            var allocatedLoaderPath = process.AllocateAndWrite(rawLoaderPath,
                Native.AllocationType.Commit | Native.AllocationType.Reserve, Native.MemoryProtection.ReadWrite);

            var loadLibraryHandle = Native.CreateRemoteThread(process.Handle, 0, 0, loadLibraryFn, allocatedLoaderPath, 0, out ulong _);
            NativeExtensions.WaitForThread(loadLibraryHandle);
        }

        private static void Call(Process process, string moduleName, string functionName, string args)
        {
            var modules = process.Modules.OfType<ProcessModule>();

            var module = modules.FirstOrDefault(m => m.ModuleName == moduleName);

            if (module == null)
                throw new ApplicationException($"Unable to find module with name ");

            var moduleFileName = module.FileName;

            var moduleHandle = process.GetModuleByName(moduleName);

            var localModuleHandle = Native.LoadLibrary(moduleFileName);

            var moduleNameWithoutExtension = Path.GetFileNameWithoutExtension(moduleName);
            var localFn = NativeExtensions.GetModuleFunction(moduleName, functionName);

            if (localFn == 0)
                throw new ApplicationException($"Unable to fine function in module '{moduleName}' with name '{functionName}'");

            var fnOffset = localFn - localModuleHandle;

            var rawArgs = Encoding.Unicode.GetBytes(args);
            var allocatedArgs = process.AllocateAndWrite(rawArgs,
                Native.AllocationType.Commit | Native.AllocationType.Reserve, Native.MemoryProtection.ReadWrite);

            var fn = moduleHandle + fnOffset;

            var fnHandle = Native.CreateRemoteThread(process.Handle, 0, 0, fn, allocatedArgs, 0, out ulong _);
            NativeExtensions.WaitForThread(fnHandle);
        }
    }
}
