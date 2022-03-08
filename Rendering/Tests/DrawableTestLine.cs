using System.Drawing;

public class DrawableTestLine : DrawableObject
{
    public int z {get;set;}

    PointF A, B;

    public DrawableTestLine(Vector2 a, Vector2 b)
    {
        A = a.ToPoint();
        B = b.ToPoint();
    }

    public void Draw(Graphics g, double lerp)
    {
        g.DrawLine(new Pen(Color.Black), A.X, A.Y, B.X, B.Y);
    }
}