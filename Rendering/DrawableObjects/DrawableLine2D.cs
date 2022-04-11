using SFML.Graphics;
using SFML.System;
using System;

public class DrawableLine2D : DrawableObject
{
    public int z {get;set;}

    Vector2 A, B;

    Color Color1, Color2;

    Vector2f CurrPos, LastPos;

    public DrawableLine2D (Vector2 a, Vector2 b, Color color, Vector2 currPos, Vector2 lastPos, int _z)
    {
        A = a;
        B = b;

        Color1 = color;
        Color2 = color;

        CurrPos = currPos.ToVectorF();
        LastPos = lastPos.ToVectorF();

        z = _z;
    }

    public DrawableLine2D (Vector2 a, Vector2 b, Color color, Color color2, Vector2 currPos, Vector2 lastPos, int _z)
    {
        A = a;
        B = b;

        Color1 = color;
        Color2 = color2;

        CurrPos = currPos.ToVectorF();
        LastPos = lastPos.ToVectorF();

        z = _z;
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

    public bool IsOptimizable(DrawableObject obj)
    {
        return obj is DrawableLine2D;
    }

    public int Optimize (DrawableObject[] objs, int currIndex, double lerp, RenderWindow w)
    {
        float fLerp = (float) lerp;

        int jump = 0;

        Vertex[] verts;

        int optCount = 0;

        for (int i = currIndex; i< objs.Length; ++i)
        {
            if(IsOptimizable(objs[i])) ++optCount;
            else break;
        }

        jump = optCount - 1;

        verts = new Vertex[optCount * 2];

        this.ComputeVert(verts, 0, fLerp);

        int ind = 2;

        for (int i = currIndex + 1; i< optCount + currIndex; ++i)
        {
            (objs[i] as DrawableLine2D).ComputeVert(verts, ind, fLerp);
            ind +=2;
        }

        w.Draw(verts, PrimitiveType.Lines);

        return jump;
    }

    public void ComputeVert(Vertex[] vertArr, int index, float lerp)
    {
        Vector2f p = RenderMath.Lerp(LastPos, CurrPos, lerp);

        Vertex a = new Vertex(A.ToVectorF() + p);
        Vertex b = new Vertex(B.ToVectorF() + p);

        a.Color = Color1;
        b.Color = Color2;

        vertArr[index] = a;
        vertArr[index + 1] = b;
    }
}