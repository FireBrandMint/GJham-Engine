using SFML.Graphics;
using SFML.System;
using System;

public sealed class DrawableLine2D : DrawableObject
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

    public void Reinitialize (Vector2 a, Vector2 b, Color color, Vector2 currPos, Vector2 lastPos, int _z)
    {
        A = a;
        B = b;

        Color1 = color;
        Color2 = color;

        CurrPos = currPos;
        LastPos = lastPos;

        z = _z;
    }

    public void Reinitialize (Vector2 a, Vector2 b, Color color, Color color2, Vector2 currPos, Vector2 lastPos, int _z)
    {
        A = a;
        B = b;

        Color1 = color;
        Color2 = color2;

        CurrPos = currPos;
        LastPos = lastPos;

        z = _z;
    }

    public void Draw(RenderArgs args)
    {

        //Vertex a = av.ToVertex();
        //Vertex b = B.ToVertex();

        Vector2 p = Vector2.Lerp(LastPos, CurrPos, (FInt)args.lerp) - args.cameraPos;

        Vertex a = new Vertex((Vector2f)(A + p));
        Vertex b = new Vertex((Vector2f)(B + p));

        a.Color = Color1;
        b.Color = Color2;

        args.w.Draw(new Vertex[2] {a, b}, PrimitiveType.Lines);
    }

    public bool Optimizable (DrawableObject obj) => obj is DrawableLine2D;

    public void DrawOptimizables(RenderArgs args, DrawableObject[] dObjects, uint index, uint count)
    {

        VertexArray array = new VertexArray(PrimitiveType.Lines, count * 2);

        uint arrInd = 0;

        for(uint i = index; i< index + count; ++i)
        {
            ((DrawableLine2D)dObjects[i]).FillVA(array, arrInd, args.lerp, args.cameraPos);

            arrInd+=2;
        }

        array.Dispose();
    }

    public void FillVA(VertexArray arr, uint index, float lerp, Vector2 camPos)
    {
        Vector2 p = Vector2.Lerp(LastPos, CurrPos, (FInt)lerp) - camPos;

        Vertex a = new Vertex((A + p).ToVectorF());
        Vertex b = new Vertex((B + p).ToVectorF());

        a.Color = Color1;
        b.Color = Color2;

        arr[index] = a;
        arr[index + 1] = b;
    }
}