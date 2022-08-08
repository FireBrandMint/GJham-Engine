using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Linq;
using SFML.Window;
using System.Threading;
using SFML.Graphics;
using SFML.System;

///<summary>
///VERY complex rendering class.
///Will take a while to document everything,
///
///</summary>
public class Canvas
{

    private RenderWindow Window;

    ManualResetEvent AllowRendering = new ManualResetEvent(false);

    public int FPS = 0;

    // wether the window is close or not
    public bool IsClosed = false;

    //Wether or not this window is in the rendering process.
    public bool Updating = false;

    public ManualResetEvent WaitRendering = new ManualResetEvent(true);

    //This buffer stores the keys that are currently pressed.
    List <int> KeysPressed = new List<int>();

    //The list below is the buffer of things the screen needs to draw next in a _Render call.
    private DrawableObject[] ToDraw = new DrawableObject[0];

    //The actual lenght of the ToDraw array
    int ToDrawCount = 0;

    double Lerp = 1.0;

    Vector2 CameraPos;

    Vector2u LAST_SIZE;
    Vector2u SIZE;
    string NAME;

    //Rendering time it took.
    long RenderingTs = 0;
    double OrganizePercent = 0;
    double DrawPercent = 0;

    public Canvas (uint width, uint height, string name)
    {
        //Size = new Size(width, height);
        //Text = name;

        SIZE = new Vector2u(width, height);
        LAST_SIZE = SIZE;
        NAME = name;

        Engine.WindowSize = new Vector2((int)SIZE.X, (int)SIZE.Y);

        /*Window = new RenderWindow(new VideoMode(WIDTH, HEIGHT), NAME);

        Window.Closed += _FormClosed;
        Window.KeyPressed += _KeyDown;
        Window.KeyReleased += _KeyUp;
        Window.LostFocus += _LostFocus;

        Window.SetActive(true);

        Window.RequestFocus();*/

        TextureHolder.StartThread();

        Thread runThread = new Thread(Run);
        runThread.Priority = ThreadPriority.AboveNormal;
        runThread.Start();
    }

    private RenderWindow InitializeWindow ()
    {
        var window = new RenderWindow(new VideoMode(SIZE.X, SIZE.Y), NAME);

        window.Closed += _FormClosed;
        window.KeyPressed += _KeyDown;
        window.KeyReleased += _KeyUp;
        window.LostFocus += _LostFocus;

        window.Resized += _Resized;

        

        window.SetVerticalSyncEnabled(true);

        window.RequestFocus();

        return window;
    }

    void _Render ()
    {
        WaitRendering.Reset();

        Stopwatch performanceData = Stopwatch.StartNew();

        Updating = true;

        Window.Clear(new Color(50, 50, 50));

        //Draws next frame
        DrawableObject[] toDraw;

        long organizeTime = 0;
        if (performanceData != null) organizeTime = performanceData.ElapsedTicks;

        lock (ToDraw)
        {

            //if (!ToDrawOrganized)
            //{
                toDraw = Organize(ToDraw, ToDrawCount);
            //}
            //else
            //{
            //    toDraw = new DrawableObject[ToDrawCount];
            //    Array.Copy(ToDraw, toDraw, ToDrawCount);
            //}
        }
        organizeTime = performanceData.ElapsedTicks - organizeTime;

        long drawTime = 0;
        drawTime = performanceData.ElapsedTicks;

        Vector2f wSize = (Vector2f)SIZE;

        float fLerp = (float) Lerp;

        bool optimizing = false;

        uint optIndex = 0;

        uint optCount = 0;

        DrawableObject optObject = null;

        uint tdLenght = (uint)toDraw.Length;

        RenderArgs args = new RenderArgs()
        {
            w = Window,
            lerp = fLerp,
            windowSize = wSize,
            cameraPos = CameraPos,
        };

        for (uint i = 0; i< tdLenght; ++i)
        {
            DrawableObject tdr = toDraw[i];

            if (i + 1 == tdLenght)
            {
                tdr.Draw(args);

                if(optimizing)
                {
                    if(optObject.Optimizable(tdr)) ++optCount;
                    else tdr.Draw(args);

                    optObject.DrawOptimizables(args, toDraw,  optIndex, optCount);
                }
                else tdr.Draw(args);

                continue;
            }

            if (optimizing)
            {
                if(optObject.Optimizable(tdr))
                {
                    ++optCount;
                }
                else
                {
                    tdr.Draw(args);

                    optObject.DrawOptimizables(args, toDraw,  optIndex, optCount);

                    optimizing = false;
                }
            }
            else
            {
                if (tdr.Optimizable(toDraw[i + 1]))
                {
                    optIndex = i;
                    optCount = 1;
                    optimizing = true;

                    optObject = tdr;
                }
            }

            tdr.Draw(args);
        }
        

        Window.DispatchEvents();
        Window.Display();

        drawTime = performanceData.ElapsedTicks - drawTime;

        //if(!ToDrawOrganized)
        //{
        //    ToDrawOrganized = true;

        //    ToDraw = toDraw;
        //}

        long ticksPassed = performanceData.ElapsedTicks;

        performanceData.Stop();

        RenderingTs += ticksPassed;

        DrawPercent += drawTime== 0.0 ? 0.0 : (double) drawTime / ticksPassed;
        DrawPercent/=2;
        OrganizePercent += organizeTime == 0.0 ? 0.0 : (double) organizeTime / ticksPassed;
        OrganizePercent/=2;

        if (FPS == 0)
        {

            if (RenderingTs < 200) Console.WriteLine("Rendering took no time at all.");
            else
            {
                Console.WriteLine($"Rendering took {String.Format("{0:0.000}", ((double)RenderingTs / (double) Stopwatch.Frequency) * 1000)}MS!");
                Console.WriteLine($"Draw time: {DrawPercent *100.0}%, OrganizeTime: {OrganizePercent * 100.0}%");
            }

            RenderingTs = 0;
            DrawPercent = 0;
            OrganizePercent = 0;

        }
        
        ++FPS;
        Updating = false;
        WaitRendering.Set();
    }

    //Method that takes part in organizing the list of render objects
    private DrawableObject[] Organize(DrawableObject[] drawable, int size)
    {
        DrawableObject[] td = new DrawableObject[ToDrawCount];
        Array.Copy(ToDraw, td, size);

        return (from p in td
        orderby p.z
        select p).ToArray<DrawableObject>();
    }

    public int[] GetKeys ()
    {
        lock (KeysPressed)
        {
            return KeysPressed.ToArray();
        }
    }

    public void SetDraw (DrawableObject[] objs, int count)
    {
        //set objects to draw this frame
        lock (ToDraw)
        {
            if (count <= ToDraw.Length)
            {
                for(int i = 0; i< count; ++i)
                {
                    ToDraw[i] = objs[i];
                }

                if(count > ToDraw.Length)
                {
                    for (int i = count; i < ToDraw.Length; ++i)
                    {
                        ToDraw[i] = null;
                    }
                }
            }
            else ToDraw = objs;

            
            ToDrawCount = count;
        }
    }

    public void SetLerp (double lerp)
    {
        if(Engine.MaxTPS > 59)
        {
            Lerp = 1.0;
            return;
        }

        Lerp = lerp;
    }

    void _FormClosed(object sender, EventArgs e)
    {
        Close();
    }

    void _KeyDown (object sender, KeyEventArgs e)
    {
        lock (KeysPressed) if (!KeysPressed.Contains((int) e.Code)) KeysPressed.Add((int) e.Code);
    }

    void _KeyUp (object sender, KeyEventArgs e)
    {
        
        lock (KeysPressed) if (KeysPressed.Contains((int) e.Code)) KeysPressed.Remove((int) e.Code);
    }

    void _LostFocus (object sender, System.EventArgs e)
    {
        lock (KeysPressed) KeysPressed.Clear();
    }

    void _Resized(object sender, SizeEventArgs e)
    {
        var wSize = new Vector2((int)e.Width, (int) e.Height);

        Engine.WindowSize = wSize;

        SIZE = new Vector2u((uint)wSize.x, (uint)wSize.y);
    }

    public void ChangeWindowSize (Vector2 size)
    {
        SIZE = new Vector2u((uint)size.x, (uint)size.y);


    }

    public void Refresh ()
    {
        CameraPos = Engine.ViewPos;
        AllowRendering.Set();
    }

    public void Run()
    {
        Window = InitializeWindow();

        while(!IsClosed)
        {

            AllowRendering.WaitOne();
            AllowRendering.Reset();

            if(LAST_SIZE.X != SIZE.X || LAST_SIZE.Y != SIZE.Y)
            {
                Window.SetView(new View(new FloatRect(0f, 0f, SIZE.X, SIZE.Y)));

                if (Window.Size != SIZE) Window.Size = SIZE;

                LAST_SIZE = SIZE;
            }

            if (Window.IsOpen) _Render();
            else break;
        }

        IsClosed = true;

        Console.WriteLine("Ended window thread.");
    }

    public void Close()
    {
        Window.Close();
        IsClosed = true;
        
        AllowRendering.Set();
    }
}

public class RenderArgs
{
    public RenderWindow w;

    public float lerp;

    public Vector2f windowSize;

    public Vector2 cameraPos;
}

//Old useless code for a windows forms problem.
/*internal static class NativeWinAPI
{
    internal static readonly int GWL_EXSTYLE = -20;
    internal static readonly int WS_EX_COMPOSITED = 0x02000000;

    [DllImport("user32")]
    internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32")]
    internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}*/