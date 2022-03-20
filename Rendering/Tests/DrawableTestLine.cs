using SFML.Graphics;

public class DrawableTestLine : DrawableObject
{
    public int z {get;set;}

    Vertex A, B;

    public DrawableTestLine(Vector2 a, Vector2 b, Color color)
    {
        A = a.ToVertex();
        B = b.ToVertex();

        A.Color = color;
        B.Color = color;
    }

    public DrawableTestLine(Vector2 a, Vector2 b, Color color, Color color2)
    {
        A = a.ToVertex();
        B = b.ToVertex();

        A.Color = color;
        B.Color = color2;
    }

    public void Draw(RenderWindow w, double lerp)
    {
        //g.DrawLine(new Pen(Color.Black), A.X, A.Y, B.X, B.Y);
        w.Draw(new Vertex[2] {A, B}, PrimitiveType.Lines);
    }
}