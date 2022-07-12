using System.Drawing;

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
    public static readonly double MaxTPS = 60;

    ///<summary>
    ///The set amount of rendered frames the program is set to try to process every second.
    ///</summary>
    public static readonly double MaxFPS = 180;

    ///<summary>
    ///Is the FPS limited to the 'MaxFPS' field on this same class.
    ///</summary>
    public static bool FPSLimiter = true;

    //This is the amount of ticks that are about to be executed very soon to conpensate for a slowdown
    public static int TPSSlowdown = 0;
}