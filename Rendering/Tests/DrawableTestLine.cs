using System.Drawing;

public class DrawableTestLine : DrawableObject
{
    public void Draw(Graphics g, double lerp)
    {
        g.DrawLine(new Pen(Color.Black), 20, 20f, 40f, 40f);
    }
}