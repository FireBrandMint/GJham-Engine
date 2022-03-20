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

    double Lerp = 1.0;

    string Status = "OPEN";

    uint WIDTH, HEIGHT;
    string NAME;

    public Canvas (uint width, uint height, string name)
    {
        //Size = new Size(width, height);
        //Text = name;

        WIDTH = width;
        HEIGHT = height;
        NAME = name;

        /*Window = new RenderWindow(new VideoMode(WIDTH, HEIGHT), NAME);

        Window.Closed += _FormClosed;
        Window.KeyPressed += _KeyDown;
        Window.KeyReleased += _KeyUp;
        Window.LostFocus += _LostFocus;

        Window.SetActive(true);

        Window.RequestFocus();*/

        Thread runThread = new Thread(Run);
        runThread.Start();
    }

    void _Render ()
    {
        Updating = true;

        Window.Clear(Color.Black);

        //Draws next frame
        lock (ToDraw)
        {
            var qry = from p in ToDraw
            orderby p.z
            select p;

            DrawableObject[] toDraw = qry.ToArray<DrawableObject>();


            for (int i = 0; i< toDraw.Length; ++i)
            {
                toDraw[i]?.Draw(Window, Lerp);
            }
        }

        Window.DispatchEvents();
        Window.Display();
        
        ++FPS;
        Updating = false;
    }

    public int[] GetKeys ()
    {
        
        lock (KeysPressed)
        {
            return KeysPressed.ToArray();
        }
    }

    public void SetDraw (DrawableObject[] objs)
    {
        //set objects to draw this frame
        lock (ToDraw)
        {

            ToDraw = objs;
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
        Window = new RenderWindow(new VideoMode(WIDTH, HEIGHT), NAME);

        Window.Closed += _FormClosed;
        Window.KeyPressed += _KeyDown;
        Window.KeyReleased += _KeyUp;
        Window.LostFocus += _LostFocus;

        Window.RequestFocus();

        while(!IsClosed)
        {

            AllowRendering.WaitOne();

            lock(Status) if(Status == "CLOSED") break;

            _Render();

            lock(Status) if(Status == "CLOSED") break;

            AllowRendering.Reset();
        }
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