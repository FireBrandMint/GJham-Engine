using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.IO;

public class AssemblyUtilities
{

    public static void CompileAssembly(string sourcePath, string outputPath)
    {
        ExecuteCommand($"dotnet build -o {outputPath} ", sourcePath);
    }

    public static AssemblyInstance LoadAssembly(string path)
    {
        CollectibleAssemblyLoadContext context;
        Assembly sembly;
        using (var fs = new FileStream(Path.GetFullPath(path), FileMode.Open, FileAccess.Read))
        {
            context = new CollectibleAssemblyLoadContext();
            sembly = context.LoadFromStream(fs);
        }

        return new AssemblyInstance(sembly, context);
    }

    public static Type[] TypesWithInterface(string interfaceName, Assembly sembly)
    {
        Type it = sembly.GetType(interfaceName);

        var lt = GetLoadableTypes(sembly);
        return lt.Where(it.IsAssignableFrom).ToList().ToArray();
    }

    static void ExecuteCommand(string command)
    {
        var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        processInfo.RedirectStandardError = true;
        processInfo.RedirectStandardOutput = true;

        var process = Process.Start(processInfo);

        if (process == null)
        {
            Console.WriteLine("ERROR!!!");
            return;
        }

        process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
        Console.WriteLine("output>>" + e.Data);
        process.BeginOutputReadLine();

        process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
        Console.WriteLine("error>>" + e.Data);
        process.BeginErrorReadLine();

        process.WaitForExit();

        Console.WriteLine("ExitCode: {0}", process.ExitCode);
        process.Close();
    }

    static void ExecuteCommand(string command, string workingDirectory)
    {
        var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        processInfo.RedirectStandardError = true;
        processInfo.RedirectStandardOutput = true;
        processInfo.WorkingDirectory = workingDirectory;

        var process = Process.Start(processInfo);

        if (process == null)
        {
            Console.WriteLine("ERROR!!!");
            return;
        }

        process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
        Console.WriteLine("output>>" + e.Data);
        process.BeginOutputReadLine();

        process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
        Console.WriteLine("error>>" + e.Data);
        process.BeginErrorReadLine();

        process.WaitForExit();

        Console.WriteLine("ExitCode: {0}", process.ExitCode);
        process.Close();
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly) {
        if (assembly == null) throw new ArgumentNullException("assembly");
        try {
            return assembly.GetTypes();
        } catch (ReflectionTypeLoadException e) {
            return e.Types.Where(t => t != null);
        }
    }
}

public class CollectibleAssemblyLoadContext : AssemblyLoadContext
{
    public CollectibleAssemblyLoadContext() : base(isCollectible: true)
    { }
 
    protected override Assembly Load(AssemblyName assemblyName)
    {
        Console.WriteLine("fuck");
        return Assembly.Load(assemblyName);
    }
}

public class AssemblyInstance : IDisposable
{
    public Assembly Asm;

    private CollectibleAssemblyLoadContext Context;

    public AssemblyInstance(Assembly sembly, CollectibleAssemblyLoadContext context)
    {
        Asm = sembly;
        Context = context;
    }

    bool Disposed = false;

    public void Dispose()
    {
        if(Disposed) return;
        Disposed = true;

        Context.Unload();

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

}