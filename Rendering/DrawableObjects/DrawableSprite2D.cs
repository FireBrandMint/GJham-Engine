using SFML.Graphics;
using SFML.System;
using System;

public class DrawableSprite2D : DrawableObject
{
    bool Same = false;

    bool TextureUpdated = true;

    string TexturePath;

    Vector2 LastPos, CurrPos;

    Vector2u[] Bounderies;

    bool BSet;

    FInt Rotation;

    Texture CurrTexture = null;

    VertexArray Result = null;

    public int z {get;set;}

    public DrawableSprite2D (string pathToTexture, Vector2 lastPos, Vector2 currPos, Vector2u[] bounderies, bool bounderiesSet, FInt rotationDegrees)
    {
        TexturePath = pathToTexture;
        Console.WriteLine($"TexturePath is {TexturePath}");
        
        LastPos = lastPos;
        CurrPos = currPos;

        Bounderies = bounderies;
        
        BSet = bounderiesSet;

        Rotation = rotationDegrees;
    }

    public bool IsOptimizable (DrawableObject obj)
    {
        return false;
    }

    public int Optimize (DrawableObject[] objs, int currIndex, float lerp, RenderWindow w)
    {
        throw new NotImplementedException("Method not implemented!");
    }

    public void Draw(RenderArgs args)
    {
        RenderStates states = new RenderStates();

        if(TextureUpdated)
        {
            CurrTexture = TextureHolder.GetTexture(ref TexturePath);

            TextureUpdated = false;
        }

        states.Texture = CurrTexture;

        states.Transform = Transform.Identity;

        states.BlendMode = BlendMode.None;

        if(states.Texture == null)
        {
            return;
        }

        TryRecalculateVertex(states.Texture.Size, args.lerp);

        args.w.Draw(Result, states);
    }

    public bool Optimizable (DrawableObject obj)
    {
        return false;
    }

    public void DrawOptimizables(RenderArgs args, DrawableObject[] dObjects, uint index, uint count)
    {
        throw new MissingMethodException();
    }

    private void ResolveNoBoundries(Vector2u texSize)
    {
        if(!BSet)
        {
            Bounderies[0] = new Vector2u(0,0);
            Bounderies[1] = new Vector2u(texSize.X, texSize.Y);

            BSet = true;
        }
    }

    public void TryRecalculateVertex(Vector2u texSize, float lerp)
    {
        if(Same) return;

        Same = true;
       
        VertexArray quads = new VertexArray(PrimitiveType.Quads, 4);

        ResolveNoBoundries(texSize);

        Vector2 halves = new Vector2((FInt) (Bounderies[1].X - Bounderies[0].X) / 2, (FInt) (Bounderies[1].Y - Bounderies[0].Y) / 2);

        Vector2 pos = Vector2.Lerp(LastPos, CurrPos, (FInt) lerp);

        Vector2f texTopLef = (Vector2f)Bounderies[0];
        Vector2f texBotRig = (Vector2f)Bounderies[1];

        FInt angle = (FInt)Rotation;

        //Top left
        quads[0] = new Vertex((Vector2f)Vector2.RotateVec(pos - halves, pos, angle), Color.White, texTopLef);
        //Bottom left
        quads[1] = new Vertex((Vector2f)Vector2.RotateVec(new Vector2(pos.x - halves.x, pos.y + halves.y), pos, angle), new Vector2f(texTopLef.X, texBotRig.Y));
        //Bottom right
        quads[2] = new Vertex((Vector2f)Vector2.RotateVec(pos + halves, pos, angle), texBotRig);
        //Top right
        quads[3] = new Vertex((Vector2f)Vector2.RotateVec(new Vector2(pos.x + halves.x, pos.y - halves.y), pos, angle), new Vector2f(texBotRig.X, texTopLef.Y));

        if(Result != null) Result.Dispose();

        Result = quads;
    }

    public void SetPosValues(Vector2 current, Vector2 last)
    {
        Same = Same && CurrPos == current && LastPos == last;

        CurrPos = current;
        LastPos = last;
    }

    public void ChangeTexturePath(string newPath)
    {
        if(!TexturePath.Equals(newPath))
        {
            TextureUpdated = true;

            TexturePath = newPath;
        }
    }

    public void ChangeBoundries(Vector2u texTopLef, Vector2u texBotRig)
    {
        Same = Same && texTopLef == Bounderies[0] && texBotRig == Bounderies[1];

        Bounderies[0] = texTopLef;
        Bounderies[1] = texBotRig;
    }
}