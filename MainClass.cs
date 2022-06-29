using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using SFML.System;

class MainClass
{

    //The actual window of 'MainThread'
    public static Canvas Window;

    //The amount of ticks since the engine started, can also be returned by 'Engine.Ticks'.
    public static Int64 TickCount = 0;

    // Ticks per second.
    public static int TPS = 0;

    // Frames per second.
    public static int FPS = 0;

    public static List<int> CurrentKeys;

    //entity list below!

    public static List<Entity> Entities;

    private static List<Entity> DrawableEntities;

    public static void Main(string[] args)
    {
        for (int i = 0; i< args.Length; ++i)
        {
            if (args[i] == "doListing")
            {
                EntityTypeList.ListAll();
                Console.WriteLine("\nEntity instancer class complete.");

                GC.Collect();
                return;
            }
        }

        Window = new Canvas(512, 512, "default");

        Entities = new List<Entity>();
        DrawableEntities = new List<Entity>();

        //TEST

        Vector2 a = new Vector2(20, 20);
        Vector2 b = new Vector2(40, 40);

        for (int i = 0; i< 100; ++i)
        {
            var lp = new DTestLineProvider();

            lp.line = new Vector2[] {a, b};

            EntityCommand.Instance(lp);

            b = new Vector2(b.x, b.y + (FInt) 3);
        }

        //TEST AREA END

        Loop();
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

        while (!Window.IsClosed)
        {
            Int64 currTick = stopwatch.ElapsedTicks;

            double elapsedTicks = currTick - LastTick;

            delta += elapsedTicks/tickExecute;

            renderDelta += elapsedTicks/renderExecute;

            secondTickCount += currTick - LastTick;

            LastTick = currTick;

            bool operationExecuted = false;

            if (delta >= 1.0)
            {
                --delta;
                //Sets 'TPSSlowdown' to the amount of ticks that will ocurr in this moment to compensate for a slowdown
                //if it's 0, it means there's no slowdown and everything is fine.
                Engine.TPSSlowdown = (int) delta;
                GetInputs();
                Tick();
                ++TickCount;

                operationExecuted = true;
            }


            if (renderDelta >= 1.0)
            {
                Render(delta);

                //If there's a slowdown, the pending 'Render' calls are set to 0 to not overwelm the program
                if (renderDelta >=2.0) renderDelta=0.0;
                else --renderDelta;

                operationExecuted = true;
            }

            if (secondTickCount >= swFrequency)
            {
                OnSecond();
                secondTickCount-= swFrequency;

                GC.Collect(0, GCCollectionMode.Forced, false);

                operationExecuted = true;
            }

            if (!operationExecuted && delta < 1.0 && renderDelta < 1.0 && secondTickCount < swFrequency)
            {
                double tTime = delta != 0.0 ? ((1.0 - delta) * tickExecute)/ swFrequency : 0.0;
                double rTime = renderDelta != 0.0 ? ((1.0 - renderDelta) * renderExecute)/ swFrequency : 0.0;
                double sTime = secondTickCount != 0 ?(double) (swFrequency - secondTickCount) / swFrequency : 0.0;

                char chosenTime;

                double sleepTime;

                if (tTime < rTime)
                {
                    sleepTime = tTime;

                    chosenTime = 't';
                }
                else
                {
                    sleepTime = rTime;

                    chosenTime = 'r';
                }

                if (sleepTime > sTime)
                {
                    sleepTime = sTime;

                    chosenTime = 's';
                }

                int sleepMsec = (int)(sleepTime * 1000.0);

                if (sleepMsec > 0)
                {
                    if(AntiConsoleSpam.antiConsoleSpam.CanWriteLine(23, 200))
                    {
                        string chosenMsg;

                        if (chosenTime == 't') chosenMsg = $"of tick time with inverse delta {1.0 - delta}, wich is {sleepTime * 1000.0}MS";
                        else if (chosenTime == 'r') chosenMsg = $"of render time with inverse delta {1.0 - renderDelta}, wich is {sleepTime * 1000.0}MS";
                        else chosenMsg = $"of render time with inverse ticks {swFrequency - secondTickCount}, wich is {sleepTime * 1000.0}MS";

                        Console.WriteLine($"Slept for {sleepMsec} miliseconds {chosenMsg}!");
                    }

                    Thread.Sleep(sleepMsec);
                }
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

        if(CurrentKeys.Contains((int)SFML.Window.Keyboard.Key.Escape))
        {
            Window.Close();
        }

        ProcessEntities();
    }

    public static void ProcessEntities ()
    {
        foreach (Entity e in Entities)
        {
            if (e.CantProcess) continue;
            e.Tick();
        }
    }

    private static long LastTickCount = -1;

    public static void Render(double lerp)
    {
        //If the window is still rendering something else, just give up on the operation
        if(Window.Updating) return;

        //This 'if' prevents the program from updating the array of things to render
        //multiple times between ticks, wich isn't necessary
        if (LastTickCount != TickCount)
        {
            //Creates buffer for rendering
            DrawableObject[] dObjects = new DrawableObject[DrawableEntities.Count];

            //the count of objects that aren't null on the array
            int count = 0;

            //populates buffer with output from the entities that can be rendered
            for (int i = 0; i< DrawableEntities.Count; ++i)
            {
                var drawable = DrawableEntities[i].GetDrawable();

                if (drawable != null)
                {
                    dObjects[count] = drawable;
                    ++count;
                }
            }
            //Sends the things that must be rendered to the screen
            Window.SetDraw(dObjects, count);

            LastTickCount = TickCount;
        }

        //Sets lerp for the screen itself to interpolate between last tick and current tick
        //just in case the screen can render more than the amount of TPS
        Window.SetLerp(lerp < 1.0? lerp : 1.0);

        //Asks politely for the screen to actually draw those things
        // I ASKED POLITELY, SCREEN
        Window.Refresh();
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
