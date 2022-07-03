using System;
//using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
//using System.Reflection;
using System.Linq;
using SFML.Window;
using System.Threading;
using SFML.Graphics;
using SFML.System;


public class Canvas
{

    private RenderWindow Window;

    ManualResetEvent AllowRendering = new ManualResetEvent(false);

    public int FPS = 0;

    // wether the window is close or not
    public bool IsClosed = false;

    //Wether or not this window is in the rendering process.
    public bool Updating = false;

    //This buffer stores the keys that are currently pressed.
    List <int> KeysPressed = new List<int>();

    //The list below is the buffer of things the screen needs to draw next in a _Render call.
    private DrawableObject[] ToDraw = new DrawableObject[0];

    //The actual lenght of the ToDraw array
    int ToDrawCount = 0;

    bool ToDrawOrganized = false;

    double Lerp = 1.0;

    string Status = "OPEN";

    Vector2u SIZE;
    string NAME;

    public Canvas (uint width, uint height, string name)
    {
        //Size = new Size(width, height);
        //Text = name;

        SIZE = new Vector2u(width, height);
        NAME = name;

        /*Window = new RenderWindow(new VideoMode(WIDTH, HEIGHT), NAME);

        Window.Closed += _FormClosed;
        Window.KeyPressed += _KeyDown;
        Window.KeyReleased += _KeyUp;
        Window.LostFocus += _LostFocus;

        Window.SetActive(true);

        Window.RequestFocus();*/

        Thread runThread = new Thread(Run);
        runThread.Priority = ThreadPriority.AboveNormal;
        runThread.Start();
    }

    void _Render ()
    {
        Updating = true;

        Stopwatch performanceData = null;
        if (FPS < 2 && !ToDrawOrganized)
        {
            performanceData = Stopwatch.StartNew();
        }

        Window.Clear(Color.Black);

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
        if (performanceData != null) organizeTime = performanceData.ElapsedTicks - organizeTime;

        long drawTime = 0;
        if (performanceData != null) drawTime = performanceData.ElapsedTicks;

        Vector2f wSize = (Vector2f)SIZE;

        float fLerp = (float) Lerp;

        for (int i = 0; i< toDraw.Length; ++i)
        {
            DrawableObject tdr = toDraw[i]; 
            tdr.Draw(Window, fLerp, wSize);
        }
        

        Window.DispatchEvents();
        Window.Display();

        if (performanceData != null) drawTime = performanceData.ElapsedTicks - drawTime;

        //if(!ToDrawOrganized)
        //{
        //    ToDrawOrganized = true;

        //    ToDraw = toDraw;
        //}

        if (performanceData != null)
        {
            double ticksPassed = performanceData.ElapsedTicks;

            if (ticksPassed == 0) Console.WriteLine("Rendering took no time at all.");
            else
            {
                double MSPassed = (ticksPassed / (double) Stopwatch.Frequency) * 1000;

                double performance = (1000d / Engine.MaxFPS) / MSPassed;

                //Console.WriteLine($"Rendering took {MSPassed}MS, it could be executed {performance} times per frame!");
                double dr = drawTime== 0.0 ? 0.0 : (double) drawTime / ticksPassed;
                double ot = organizeTime == 0.0 ? 0.0 : (double) organizeTime / ticksPassed;
                //Console.WriteLine($"Draw time: {dr *100.0}%, OrganizeTime: {ot * 100.0}%");

            }

            performanceData.Stop();
        }
        
        ++FPS;
        Updating = false;
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
            ToDraw = objs;
            ToDrawOrganized = false;
            ToDrawCount = count;
        }
    }

    public void SetLerp (double lerp)
    {
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

    public void Refresh ()
    {
        //_Render();

        AllowRendering.Set();
    }

    public void Run()
    {
        Window = new RenderWindow(new VideoMode(SIZE.X, SIZE.Y), NAME);

        Window.Closed += _FormClosed;
        Window.KeyPressed += _KeyDown;
        Window.KeyReleased += _KeyUp;
        Window.LostFocus += _LostFocus;

        Window.SetVerticalSyncEnabled(true);

        Window.RequestFocus();

        int checkTimer = 180;

        while(!IsClosed)
        {

            AllowRendering.WaitOne();

            lock(Status) if(Status == "CLOSED") break;

            _Render();

            lock(Status) if(Status == "CLOSED") break;

            AllowRendering.Reset();

            if(checkTimer <= 0)
            {
                if(!Window.IsOpen) break;

                checkTimer = 180;
            }

            --checkTimer;
        }

        IsClosed = true;

        lock(Status) Status = "CLOSED";
    }

    public void Close()
    {
        Window.Close();
        IsClosed = true;

        lock (Status) Status = "CLOSED";
        AllowRendering.Set();
    }
}

internal static class NativeWinAPI
{
    internal static readonly int GWL_EXSTYLE = -20;
    internal static readonly int WS_EX_COMPOSITED = 0x02000000;

    [DllImport("user32")]
    internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32")]
    internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}