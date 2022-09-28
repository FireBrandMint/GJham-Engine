using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


public class EditorMain
{
    public static bool Running = false;

    public static void Run ()
    {
        Running = true;

        Loop();
    }

    public static void Loop ()
    {
        //Loops without frame compensation
        //because this is the editor, it doesn't need
        //frame accuracy

        while(Running)
        {
            string command = Console.ReadLine();

            if(!TestCommands(command)) return;

            Thread.Sleep(15);
        }
    }

    static bool TestCommands(string command)
    {
        if(command.StartsWith("compile"))
        {
            string[] args = command.Split(' ');
            if(args.Length < 3 || args.Length > 3) Console.WriteLine("bad args");

            try
            {
                AssemblyUtilities.CompileAssembly(args[1], args[2]);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: \n" + e.Message + "\n\n" + e.StackTrace);

                return false;
            }
        }
        else if (command == "end")
        {
            return false;
        }
        else
        {
            Console.WriteLine("Bad command!");
        }

        return true;
    }
}