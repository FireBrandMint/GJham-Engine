using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;

class MainClass
{
    public static Thread MainThread;

    public static Thread ProcessThread;

    public static Canvas Window;

    public static Int64 TickCount = 0;

    public static int TPS = 0;

    public static List<int> CurrentKeys;

    static void Main(string[] args)
    {
        Window = new Canvas(512, 512, "default");

        MainThread = Thread.CurrentThread;

        ProcessThread = new Thread(Loop);

        ProcessThread.Start();

        Application.Run(Window);
    }

    public static void Loop ()
    {
        double ticksPerSecond = 60d;

        double rendersPerSecond = 30d;

        Int64 swFrequency = Stopwatch.Frequency;

        double tickExecute = (double)swFrequency/ticksPerSecond;

        double renderExecute = (double)swFrequency/rendersPerSecond;

        double delta = 0.0;

        double renderDelta = 0.0;

        Int64 LastTick = 0L;

        Int64 secondTickCount = 0L;

        Stopwatch stopwatch = Stopwatch.StartNew();

        while (MainThread.IsAlive)
        {
            Int64 currTick = stopwatch.ElapsedTicks;

            double elapsedTicks = currTick - LastTick;

            delta += elapsedTicks/tickExecute;

            renderDelta += elapsedTicks/renderExecute;

            secondTickCount += currTick - LastTick;

            LastTick = currTick;

            if (delta >= 1.0)
            {
                GetInputs();
                Tick();
                ++TickCount;
                --delta;
            }

            if (renderDelta >= 1.0)
            {
                Render(delta > 1.0? delta - ((int) delta): delta);
                if (renderDelta >=2.0) renderDelta=0.0;
                else --renderDelta;
            }

            if (secondTickCount >= swFrequency)
            {
                OnSecond();
                secondTickCount-= swFrequency;
            }
        }
        
        stopwatch.Stop();
    }

    static void GetInputs()
    {
        CurrentKeys = new List<int>(Window.GetKeys());
    }

    static void Tick ()
    {
        if(CurrentKeys.Count!=0)
        {
            Console.Write("Controles sendo pressionados: ");
            foreach(int i in CurrentKeys) Console.Write($"{i} ");
            Console.WriteLine();
        }

        if(CurrentKeys.Contains((int)Keys.Escape))
        {
            Window.Close();
            Window.Dispose();
        }
    }

    public static void Render(double lerp)
    {
        Window.SetLerp(lerp);

        Window.BeginInvoke( (MethodInvoker) delegate { Window.Refresh(); } );
    }

    public static TickMeasurer measurer = new TickMeasurer();

    public static void OnSecond ()
    {
        measurer.Update();

        TPS = measurer.GetTicksThisSecond();

        Console.WriteLine($"TPS: {TPS}");

        Console.WriteLine($"Time it takes for rendering is {Window.RenderDelayMS}ms.");
    }

    
}
