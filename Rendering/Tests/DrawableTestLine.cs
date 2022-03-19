using SFML.Graphics;
using System.Drawing;

public class DrawableTestLine : DrawableObject
{
    public int z {get;set;}

    Vertex A, B;

    public DrawableTestLine(Vector2 a, Vector2 b)
    {
        A = a.ToVertex();
        B = b.ToVertex();
    }

    public void Draw(RenderWindow w, double lerp)
    {
        //g.DrawLine(new Pen(Color.Black), A.X, A.Y, B.X, B.Y);
        w.Draw(new Vertex[2] {A, B}, PrimitiveType.Lines);
    }
}