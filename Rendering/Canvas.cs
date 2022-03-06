using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;


public class Canvas : Form
{
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

    public Canvas (int width, int height, string name)
    {
        Size = new Size(width, height);
        Text = name;

        FormClosed += new FormClosedEventHandler(_FormClosed);
        KeyDown += _KeyDown;
        KeyUp += _KeyUp;
        LostFocus += _LostFocus;
        Paint += _Render;

        //DoubleBuffered = true;

        
        int style = NativeWinAPI.GetWindowLong(this.Handle, NativeWinAPI.GWL_EXSTYLE);
        style |= NativeWinAPI.WS_EX_COMPOSITED;
        NativeWinAPI.SetWindowLong(this.Handle, NativeWinAPI.GWL_EXSTYLE, style);

        SetDoubleBuffer(this, true);
    }

    //Stops it from flickering, found at 
    //https://stackoverflow.com/questions/181374/visual-c-sharp-form-update-results-in-flickering

    // EDIT: IT DOESN'T WORK COMPLETELY, WHY?
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x02000000; //WS_EX_COMPOSITED
            return cp;
        }
    }

    static void SetDoubleBuffer (Control ctr, bool doubleBuffered)
    {
        try
        {
            typeof(Control).InvokeMember("DoubleBuffered",
            BindingFlags.NonPublic | BindingFlags.Instance |BindingFlags.SetProperty,
            null, ctr, new object[] {doubleBuffered});
        }
        catch
        {
            MessageBox.Show("Couldn't double buffer the screen.");
        }
    }

    void _Render (object sender, PaintEventArgs e)
    {
        Updating = true;

        var g = e.Graphics;

        //Clears last frame
        g.Clear(Engine.BackgroundColor);

        //Draws next frame
        lock (ToDraw)
        {
            for (int i = 0; i< ToDraw.Length; ++i)
            {
                ToDraw[i]?.Draw(g, Lerp);
            }
        }
        
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
        lock (ToDraw) ToDraw = objs;
    }

    public void SetLerp (double lerp)
    {
        Lerp = lerp;
    }

    void _FormClosed(object sender, FormClosedEventArgs e)
    {
        IsClosed = true;
    }

    void _KeyDown (object sender, KeyEventArgs e)
    {
        lock (KeysPressed) if (!KeysPressed.Contains((int) e.KeyCode)) KeysPressed.Add((int) e.KeyCode);
    }

    void _KeyUp (object sender, KeyEventArgs e)
    {
        
        lock (KeysPressed) if (KeysPressed.Contains((int) e.KeyCode)) KeysPressed.Remove((int) e.KeyCode);
        
    }

    void _LostFocus (object sender, System.EventArgs e)
    {
        lock (KeysPressed) KeysPressed.Clear();
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