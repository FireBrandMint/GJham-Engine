using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;


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

        DoubleBuffered = true;
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
        lock (KeysPressed)
        {
            if (!KeysPressed.Contains((int) e.KeyCode)) KeysPressed.Add((int) e.KeyCode);
        }
    }

    void _KeyUp (object sender, KeyEventArgs e)
    {
        lock (KeysPressed)
        {
            if (KeysPressed.Contains((int) e.KeyCode)) KeysPressed.Remove((int) e.KeyCode);
        }
    }

    void _LostFocus (object sender, System.EventArgs e)
    {
        lock (KeysPressed)
        {
            KeysPressed.Clear();
        }
    }
}