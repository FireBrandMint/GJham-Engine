using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;


public class Canvas : Form
{
    public bool IsClosed = false;

    List <int> KeysPressed = new List<int>();

    public Canvas (int width, int height, string name)
    {
        Size = new Size(width, height);
        Text = name;

        FormClosed += new FormClosedEventHandler(_FormClosed);
        KeyDown += _KeyDown;
        KeyUp += _KeyUp;
        LostFocus += _LostFocus;
        Paint += _Render;

        
        int style = NativeWinAPI.GetWindowLong(this.Handle, NativeWinAPI.GWL_EXSTYLE);
        style |= NativeWinAPI.WS_EX_COMPOSITED;
        NativeWinAPI.SetWindowLong(this.Handle, NativeWinAPI.GWL_EXSTYLE, style);
    }

    int y2 = 40;

    void _Render (object sender, PaintEventArgs e)
    {
        var g = CreateGraphics();

        g.Clear(Color.Gray);

        g.DrawLine(new Pen(Color.Black), 20, 20, 40, y2);

        ++y2;
    }

    public int[] GetKeys ()
    {
        
        lock (KeysPressed)
        {
            return KeysPressed.ToArray();
        }
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