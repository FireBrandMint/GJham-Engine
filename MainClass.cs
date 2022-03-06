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

    public static int FPS = 0;

    public static List<int> CurrentKeys;

    //entity list below!

    private static List<Entity> Entities;

    private static List<Entity> DrawableEntities;

    public static void Main(string[] args)
    {
        Window = new Canvas(512, 512, "default");

        Entities = new List<Entity>();
        DrawableEntities = new List<Entity>();

        //TEST

        Vector2 a = new Vector2(20, 20);
        Vector2 b = new Vector2(40, 40);

        for (int i = 0; i< 50; ++i)
        {
            var lp = new DTestLineProvider();

            lp.line = new Vector2[] {a, b};

            EntityCommand.Instance(lp);

            b = new Vector2(b.x, b.y + (FInt) 3);
        }

        //TEST AREA END

        MainThread = Thread.CurrentThread;

        ProcessThread = new Thread(Loop);

        ProcessThread.Start();

        Application.Run(Window);
    }

    public static void Loop ()
    {
        double ticksPerSecond = Engine.MaxTPS;

        double rendersPerSecond = Engine.MaxFPS;

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
                --delta;
                Engine.TPSSlowdown = (int) delta;
                GetInputs();
                Tick();
                ++TickCount;
            }

            if (renderDelta >= 1.0)
            {
                Render(delta > 1.0? delta - ((int) delta): delta);

                //If there's a slowdown, the pending 'Render' calls are set to 0 to not overwelm the program
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

        foreach (Entity e in Entities)
        {
            e.Tick();
        }
    }

    public static void Render(double lerp)
    {
        //If the window is still rendering something else, just give up on the operation
        if(Window.Updating) return;

        //Sets lerp for the screen itself interpolate between last tick and current tick
        //just in case the screen can render more than 60 FPS
        Window.SetLerp(lerp);

        //Creates buffer for rendering
        DrawableObject[] dObjects = new DrawableObject[DrawableEntities.Count];
        //populates buffer with output from the entities that can be rendered
        for (int i = 0; i< DrawableEntities.Count; ++i)
        {
            dObjects[i] = DrawableEntities[i].GetDrawable();
        }
        //Sends the things that must be rendered to the screen
        Window.SetDraw(dObjects);

        //Asks politely for the screen to actually draw those things
        Window.BeginInvoke( (MethodInvoker) delegate { Window.Refresh(); } );
    }

    public static TickMeasurer measurer = new TickMeasurer();

    public static void OnSecond ()
    {
        measurer.Update();

        TPS = measurer.GetTicksThisSecond();

        FPS = Window.FPS;

        Window.FPS = 0;

        Console.WriteLine($"TPS: {TPS}");

        Console.WriteLine($"FPS: {FPS}");
    }

    public static void AddEntity (Entity entity)
    {
        if (entity.IsTickable) Entities.Add(entity);
        if (entity.IsDrawable) DrawableEntities.Add(entity);

        entity.EnterTree();
    }

    public static void RemoveEntity (Entity entity)
    {
        if (entity.IsDestroyed) return;

        if (entity.IsTickable) Entities.Remove(entity);

        if(entity.IsDrawable) DrawableEntities.Remove(entity);
        entity.IsDestroyed = true;
    }
}
