using SFML.Graphics;
using SFML.System;
using System;

public class DrawableLine2D : DrawableObject
{
    public int z {get;set;}

    Vector2 A, B;

    Color Color1, Color2;

    Vector2 CurrPos, LastPos;

    public DrawableLine2D (Vector2 a, Vector2 b, Color color, Vector2 currPos, Vector2 lastPos, int _z)
    {
        A = a;
        B = b;

        Color1 = color;
        Color2 = color;

        CurrPos = currPos;
        LastPos = lastPos;

        z = _z;
    }

    public DrawableLine2D (Vector2 a, Vector2 b, Color color, Color color2, Vector2 currPos, Vector2 lastPos, int _z)
    {
        A = a;
        B = b;

        Color1 = color;
        Color2 = color2;

        CurrPos = currPos;
        LastPos = lastPos;

        z = _z;
    }

    public void Draw(RenderWindow w, float lerp, Vector2f windowSize)
    {

        //Vertex a = av.ToVertex();
        //Vertex b = B.ToVertex();

        Vector2 p = Vector2.Lerp(LastPos, CurrPos, (FInt)lerp);

        Vertex a = new Vertex((A + p).ToVectorF());
        Vertex b = new Vertex((B + p).ToVectorF());

        a.Color = Color1;
        b.Color = Color2;

        w.Draw(new Vertex[2] {a, b}, PrimitiveType.Lines);
    }
}