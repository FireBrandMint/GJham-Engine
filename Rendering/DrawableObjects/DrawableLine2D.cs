using SFML.Graphics;
using SFML.System;
using System;

public class DrawableLine2D : DrawableObject
{
    public int z {get;set;}

    Vector2 A, B;

    Color Color1, Color2;

    Vector2f CurrPos, LastPos;

    public DrawableLine2D (Vector2 a, Vector2 b, Color color, Vector2 currPos, Vector2 lastPos)
    {
        A = a;
        B = b;

        Color1 = color;
        Color2 = color;

        CurrPos = currPos.ToVectorF();
        LastPos = lastPos.ToVectorF();
    }

    public DrawableLine2D (Vector2 a, Vector2 b, Color color, Color color2, Vector2 currPos, Vector2 lastPos)
    {
        A = a;
        B = b;

        Color1 = color;
        Color2 = color2;

        CurrPos = currPos.ToVectorF();
        LastPos = lastPos.ToVectorF();
    }

    public void Draw(RenderWindow w, double lerp)
    {

        //Vertex a = av.ToVertex();
        //Vertex b = B.ToVertex();

        Vector2f p = RenderMath.Lerp(LastPos, CurrPos, (float) lerp);

        Vertex a = new Vertex(A.ToVectorF() + p);
        Vertex b = new Vertex(B.ToVectorF() + p);

        a.Color = Color1;
        b.Color = Color2;

        w.Draw(new Vertex[2] {a, b}, PrimitiveType.Lines);
    }
}