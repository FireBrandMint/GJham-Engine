using System.Drawing;
using System;

public static class Engine
{
    public static Color BackgroundColor = Color.Gray;

    //Amount of ticks executed last second.
    public static int TPS { get => MainClass.TPS; }

    //Amount of frames executed last second.
    public static int FPS { get=> MainClass.FPS; }

    ///<summary>
    ///Tick count since the engine started
    ///</summary>
    public static long Ticks {get => MainClass.TickCount;} 

    ///<summary>
    ///The set amount of ticks the program MUST process every second.
    ///</summary>
    public static readonly int MaxTPS = 60;

    ///<summary>
    ///The set amount of rendered frames the program is set to try to process every second.
    ///</summary>
    public static readonly int MaxFPS = 120;

    ///<summary>
    ///Is the FPS limited to the 'MaxFPS' field on this same class.
    ///</summary>
    public static bool FPSLimiter = true;

    //This is the amount of ticks that are about to be executed very soon to conpensate for a slowdown
    public static int TPSSlowdown = 0;

    ///<summary>
    ///Functions to be executed when the program closes.
    ///</summary>
    public static event Action ExecuteOnClose = null;

    public static void ExecuteOnCloseProgram (Action func)
    {
        ExecuteOnClose += func;
    }

    ///<summary>
    ///NEVER USE THIS FUNCTION!!!
    ///</summary>
    public static void EOC() => ExecuteOnClose?.Invoke();

    public static Vector2 WindowSize;

    public static Vector2 ViewSize;

    public static Vector2 ViewPos = new Vector2();
}