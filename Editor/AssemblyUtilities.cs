using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public class AssemblyUtilities
{

    public static void CompileAssembly(string sourcePath, string outputPath)
    {
        ExecuteCommand($"dotnet build -o {outputPath} ", sourcePath);
    }

    public static Assembly LoadAssembly(string path)
    {
        var sembly = Assembly.LoadFile(path);
        return Assembly.LoadFile(path);
    }

    public static Type[] TypesWithInterface(string interfaceName, Assembly sembly)
    {
        //AppDomain.CreateDomain("r").loa

        Type it = sembly.GetType(interfaceName);

        return GetLoadableTypes(sembly).Where(it.IsAssignableFrom).ToList().ToArray();
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