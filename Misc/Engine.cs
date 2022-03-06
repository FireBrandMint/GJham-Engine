using System.Drawing;

public static class Engine
{
    public static Color BackgroundColor = Color.Gray;

    public static int TPS { get => MainClass.TPS;}

    //The ideal ticks per second.
    public static readonly double MaxTPS = 60;

    //The ideal frames per second
    public static readonly double MaxFPS = 180;

    public static int TPSSlowdown = 0;
}