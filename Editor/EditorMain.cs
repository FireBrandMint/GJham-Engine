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
    }

    public static void Loop ()
    {
        //Loops without frame compensation
        //because this is the editor, it doesn't need
        //frame accuracy

        while(Running)
        {
            

            Thread.Sleep(16);
        }
    }
}