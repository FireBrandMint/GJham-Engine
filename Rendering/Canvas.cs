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

    uint WIDTH, HEIGHT;
    string NAME;

    public Canvas (uint width, uint height, string name)
    {
        //Size = new Size(width, height);
        //Text = name;

        WIDTH = width;
        HEIGHT = height;
        NAME = name;

        Window = new RenderWindow(new VideoMode(WIDTH, HEIGHT), NAME);

        Window.Closed += _FormClosed;
        Window.KeyPressed += _KeyDown;
        Window.KeyReleased += _KeyUp;
        Window.LostFocus += _LostFocus;

        Window.SetActive(true);

        Window.RequestFocus();

        //Paint += _Render;

        //DoubleBuffered = true;

        
        //int style = NativeWinAPI.GetWindowLong(this.Handle, NativeWinAPI.GWL_EXSTYLE);
        //style |= NativeWinAPI.WS_EX_COMPOSITED;
        //NativeWinAPI.SetWindowLong(this.Handle, NativeWinAPI.GWL_EXSTYLE, style);

        //SetDoubleBuffer(this, true);
    }

    //Stops it from flickering, found at 
    //https://stackoverflow.com/questions/181374/visual-c-sharp-form-update-results-in-flickering

    // EDIT: IT DOESN'T WORK COMPLETELY, WHY?
    //protected override CreateParams CreateParams
    //{
    //    get
    //    {
    //        CreateParams cp = base.CreateParams;
    //        cp.ExStyle |= 0x02000000; //WS_EX_COMPOSITED
    //        return cp;
    //    }
    //}

    //static void SetDoubleBuffer (Control ctr, bool doubleBuffered)
    //{
    //    try
    //    {
    //        typeof(Control).InvokeMember("DoubleBuffered",
    //        BindingFlags.NonPublic | BindingFlags.Instance |BindingFlags.SetProperty,
    //        null, ctr, new object[] {doubleBuffered});
    //    }
    //    catch
    //    {
    //        MessageBox.Show("Couldn't double buffer the screen.");
    //    }
    //}

    void _Render ()
    {
        Updating = true;

        Window.Clear(Color.Black);

        //Draws next frame
        var qry = from p in ToDraw
        orderby p.z
        select p;

        DrawableObject[] toDraw = qry.ToArray<DrawableObject>();


        for (int i = 0; i< toDraw.Length; ++i)
        {
            toDraw[i]?.Draw(Window, Lerp);
        }
        
        ++FPS;
        Updating = false;

        Window.DispatchEvents();
        Window.Display();
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
        lock (ToDraw) ToDraw = objs;
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
        _Render();
    }

    public void Run()
    {
        Window = new RenderWindow(new VideoMode(WIDTH, HEIGHT), NAME);

        Window.Closed += _FormClosed;
        Window.KeyPressed += _KeyDown;
        Window.KeyReleased += _KeyUp;
        Window.LostFocus += _LostFocus;

        while(!IsClosed)
        {
            AllowRendering.WaitOne();
            _Render();

            AllowRendering.Reset();
        }
    }

    public void Close()
    {
        Window.Close();
        IsClosed = true;
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