using SFML.Graphics;
using SFML.System;
using System;

public sealed class DrawableSprite2D : DrawableObject
{

    bool SprStatic;
    bool TextureUpdated = true;

    public string TexturePath;

    public int TexturePathHash;

    Vector2 LastPos, CurrPos;

    Vector2 Scale;

    Vector2u[] Bounderies;

    bool BSet;

    FInt Rotation;

    TextureHolder CurrTexture = null;

    VertexArray Result = null;

    Color Modulate = Color.White;

    public int z {get;set;}

    public DrawableSprite2D (int _z, string pathToTexture, Vector2 lastPos, Vector2 currPos, Vector2 scale, Vector2u[] bounderies, bool bounderiesSet, FInt rotationDegrees, bool isStatic)
    {
        z = _z;

        TexturePathHash = pathToTexture.GetHashCode();
        TexturePath = pathToTexture;
        
        LastPos = lastPos;
        CurrPos = currPos;

        Scale = scale;

        Bounderies = bounderies;
        
        BSet = bounderiesSet;

        Rotation = rotationDegrees;

        SprStatic = isStatic;

        Result = new VertexArray(PrimitiveType.Quads, 4);
    }

    public void Draw(RenderArgs args)
    {
        RenderStates states = new RenderStates();

        Texture texture;

        var cashedTextr = TryCashTexture();

        if(cashedTextr == null) return;

        lock(cashedTextr)
        {
            if(cashedTextr.Disposed) return;
            texture = cashedTextr.texture;
        

            states.Texture = texture;

            states.Transform = Transform.Identity;

            states.BlendMode = BlendMode.None;

            if(states.Texture == null)
            {
                return;
            }

            TryRecalculateVertex(states.Texture.Size, args.lerp, args.cameraPos);

            args.w.Draw(Result, states);
        }
    }

    public bool Optimizable (DrawableObject obj)
    {
        return obj is DrawableSprite2D spr && TexturePathHash == spr.TexturePathHash && TexturePath == spr.TexturePath;
    }

    public void DrawOptimizables(RenderArgs args, DrawableObject[] dObjects, uint index, uint count)
    {
        Texture texture;

        var cashedTextr = TryCashTexture();

        if(cashedTextr == null) return;

        lock(cashedTextr)
        {
            if(cashedTextr.Disposed) return;
            texture = cashedTextr.texture;

            float lerp = args.lerp;
            Vector2u texSize = texture.Size;

            RenderStates states = new RenderStates()
            {
                Texture = texture,
                Transform = Transform.Identity,
                BlendMode = BlendMode.None
            };

            VertexArray arr = new VertexArray(PrimitiveType.Quads, count * 4);

            uint vertIndex = 0;

            for(uint i = index; i < index + count; ++i)
            {
                DrawableSprite2D curr = (DrawableSprite2D)dObjects[i];

                curr.FillBatch(arr, vertIndex, texSize, lerp, args.cameraPos);

                vertIndex+=4;
            }

            args.w.Draw(arr, states);

            arr.Dispose();
        }
    }

    public void FillBatch(VertexArray arr, uint index, Vector2u texSize, float lerp, Vector2 cameraPos)
    {
        TryRecalculateVertex(texSize, lerp, cameraPos);

        var res0 = Result[0];
        var res1 = Result[1];
        var res2 = Result[2];
        var res3 = Result[3];

        arr[index] = res0;
        arr[index + 1] = res1;
        arr[index + 2] = res2;
        arr[index + 3] = res3;
    }

    public void DisposeResources()
    {
        CurrTexture = null;

        Result.Dispose();
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

    public void TryRecalculateVertex(Vector2u texSize, float lerp, Vector2 camPosition)
    {

        ResolveNoBoundries(texSize);

        if(SprStatic) lerp = 1f;

        Vector2 halves = new Vector2((FInt) (Bounderies[1].X - Bounderies[0].X) / 2 * Scale.x, (FInt) (Bounderies[1].Y - Bounderies[0].Y) / 2 * Scale.y);

        Vector2 pos;

        Vector2f tempPos = (CurrPos - camPosition).ToVectorF();

        if(SprStatic) pos = (Vector2) tempPos;
        else
        {
            Vector2f tempPosPast = (LastPos - camPosition).ToVectorF();

            pos = (Vector2) RenderMath.Lerp(tempPosPast, tempPos, lerp);

            //pos = Vector2.Lerp(LastPos, CurrPos, (FInt) lerp) - camPosition;
        }

        Vector2f texTopLef = (Vector2f)Bounderies[0];
        Vector2f texBotRig = (Vector2f)Bounderies[1];

        FInt angle = (FInt)Rotation;

        //Top left
        Result[0] = new Vertex(Vector2.RotateVec(pos - halves, pos, angle).ToVectorF(), Modulate, texTopLef);
        //Bottom left
        Result[1] = new Vertex(Vector2.RotateVec(new Vector2(pos.x - halves.x, pos.y + halves.y), pos, angle).ToVectorF(), Modulate, new Vector2f(texTopLef.X, texBotRig.Y));
        //Bottom right
        Result[2] = new Vertex(Vector2.RotateVec(pos + halves, pos, angle).ToVectorF(), Modulate, texBotRig);
        //Top right
        Result[3] = new Vertex(Vector2.RotateVec(new Vector2(pos.x + halves.x, pos.y - halves.y), pos, angle).ToVectorF(), Modulate, new Vector2f(texBotRig.X, texTopLef.Y));
    }

    public TextureHolder TryCashTexture()
    {
        if(TextureUpdated)
        {
            CurrTexture = TextureHolder.GetTexture(ref TexturePath);

            TextureUpdated = false;
        }

        return CurrTexture;
    }

    public void SetPosValues(Vector2 current, Vector2 last)
    {
        CurrPos = current;
        LastPos = last;
    }

    public void SetScale(Vector2 newScale)
    {
        Scale = newScale;
    }

    public void SetRotation(FInt rot)
    {
        Rotation = rot;
    }

    public void ChangeTexturePath(string newPath)
    {
        TextureUpdated = true;

        TexturePathHash = newPath.GetHashCode();
        TexturePath = newPath;
    }

    public void ChangeBoundries(Vector2u texTopLef, Vector2u texBotRig)
    {
        Bounderies[0] = texTopLef;
        Bounderies[1] = texBotRig;
    }

    public void ChangeModulate(Color color)
    {
        Modulate = color;
    }
}