using System.Drawing;

public static class Engine
{
    public static Color BackgroundColor = Color.Gray;

    //Amount of ticks executed last second.
    public static int TPS { get => MainClass.TPS; }

    //Amount of frames executed last second.
    public static int FPS { get=> MainClass.FPS; }

    //Tick count since the engine started
    public static long Ticks {get => MainClass.TickCount;} 

    //The ideal ticks per second.
    public static readonly double MaxTPS = 60;

    //The ideal frames per second
    public static readonly double MaxFPS = 180;

    //Ticks this tick that are about to be executed very soon due to a slowdown
    public static int TPSSlowdown = 0;
}