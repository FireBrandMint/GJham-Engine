using System;
using System.Collections.Generic;
using System.Threading;
using System.Security.Policy;


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
            Console.Write('>');
            string command = Console.ReadLine();

            if(!TestCommands(command)) return;

            Thread.Sleep(15);
        }
    }

    static bool TestCommands(string command)
    {
        if(command.StartsWith("asmcompile"))
        {
            string[] args = command.Split(' ');
            if(args.Length < 3 || args.Length > 3) goto BadArgs;

            try
            {
                AssemblyUtilities.CompileAssembly(args[1], args[2]);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: \n" + e.Message + "\n\n" + e.StackTrace);

                return true;
            }
        }
        else if (command.StartsWith("asmtest"))
        {
            string[] args = command.Split(' ');
            if(args.Length == 1) goto BadArgs;

            try
            {
                if (args[1] == "help")
                {
                    Console.WriteLine("asm_test <\"assembly path\"> <interface classes to herit>");
                }
                else if(command.Contains('\"'))
                {
                    var ind = command.IndexOf('\"');
                    var ind2 = command.IndexOf('\"', ind + 1);

                    ind +=1;

                    string path = command.Substring(ind, ind2 - ind);

                    string trf = command.Substring(ind2 + 2, (command.Length - 1) - (ind2 + 1));

                    Console.WriteLine(path + '\n' + trf);

                    var asm = AssemblyUtilities.LoadAssembly(path);

                    var it = AssemblyUtilities.TypesWithInterface(trf, asm.Asm);

                    for (int i = 0; i< it.Length; ++i)
                    {
                        Console.WriteLine();
                        Console.Write(it[i].Name + ", ");
                    }

                    Console.WriteLine();

                    asm.Dispose();
                }
                else
                {
                    goto BadArgs;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: \n" + e.Message + "\n\n" + e.StackTrace);

                return true;
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

        BadArgs:

        Console.WriteLine("Bad args!");

        return true;
    }
}