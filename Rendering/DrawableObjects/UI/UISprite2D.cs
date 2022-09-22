using System;
using SFML.System;
using SFML.Graphics;

public class UISprite2D : DrawableObject
{
    int _z = 0;
    public int z { get => _z; set => _z = value; }

    TextureHolder CurrTexture = null;

    FInt Rotation = (FInt) 0;

    bool SprChanged = true;

    bool WholeSprite = true;

    string TexturePath;

    Vector2 [] SprDrawData = null;

    Vector2f [] SprSectionData = null;

    Color Modulate = Color.White;

    UIAdjustmentMode Mode;
    /// <summary>
    /// centerPercent is the coords of the center of the sprite position in the screen
    /// where X and Y have a 0 to 1000 range.
    /// sprLengthsPercent is the height and lenght of the sprite display in the screen
    /// where X and Y have a 0 to 1000 range.
    /// mode is the way the UI sprite will be displayed on screen.
    /// </summary>
    public UISprite2D (string texturePath, Vector2 centerPercent, Vector2 sprLengthsPercent, UIAdjustmentMode mode)
    {
        ChangeTexture(ref texturePath);

        SprDrawData = new Vector2[]
        {
            centerPercent, sprLengthsPercent
        };

        Mode = mode;
    }
    /// <summary>
    /// centerPercent is the coords of the center of the sprite position in the screen
    /// where X and Y have a 0 to 100 range.
    /// sprLengthsPercent is the height and lenght of the sprite display in the screen
    /// where X and Y have a 0 to 100 range.
    /// mode is the way the UI sprite will be displayed on screen.
    /// sprSectionTopLeft and sprSectionBottomRight are the section in the texture
    /// this sprite is meant to render.
    /// </summary>
    public UISprite2D (string texturePath, Vector2 center,
    Vector2 sprLengthsPercent, UIAdjustmentMode mode, Vector2 sprSectionTopLeft, Vector2 sprSectionBottomRight)
    {
        ChangeTexture(ref texturePath, sprSectionTopLeft, sprSectionBottomRight);

        SprDrawData = new Vector2[]
        {
            center, sprLengthsPercent
        };

        Mode = mode;
    }

    public void Draw(RenderArgs args)
    {

        Texture texture;

        var cashedTextr = TryCasheTexture();

        if(cashedTextr == null) return;

        lock(cashedTextr)
        {
            if(cashedTextr.Disposed) return;
            texture = cashedTextr.texture;

            VertexArray verts = new VertexArray(PrimitiveType.Quads, 4u);

            SolveNoSprSection(texture.Size);

            var center = SprDrawData[0].ToVectorF();

            center *= 0.01f;

            var slp = (SprDrawData[1]).ToVectorF();

            slp *= 0.01f;

            var vSize = args.windowView;

            center = new Vector2f(vSize.X * center.X, vSize.Y * center.Y);

            if(Mode == UIAdjustmentMode.Extended)
            {
                slp = new Vector2f(vSize.X * slp.X, vSize.Y * slp.Y);
            }
            else if (Mode == UIAdjustmentMode.Compact)
            {
                float factor = vSize.X;
                if(factor > vSize.Y) factor = vSize.Y;

                slp = new Vector2f(factor * slp.X, factor * slp.Y);
            }

            //Console.WriteLine(slp);

            var centerTrue = new Vector2((FInt)center.X, (FInt) center.Y);
            var slpTrue = new Vector2((FInt)slp.X, (FInt) slp.Y) >> 1;

            //Top left
            verts[0] = new Vertex(
                Vector2.RotateVec(new Vector2(centerTrue.x - slpTrue.y, centerTrue.y - slpTrue.y), centerTrue, Rotation).ToVectorF(),
                Modulate,
                SprSectionData[0]
                );
            //Bottom left
            verts[1] = new Vertex(
                Vector2.RotateVec(new Vector2(centerTrue.x - slpTrue.x, centerTrue.y + slpTrue.y), centerTrue, Rotation).ToVectorF(),
                Modulate,
                SprSectionData[1]
                );
            //Bottom right
            verts[2] = new Vertex(
                Vector2.RotateVec(new Vector2(centerTrue.x + slpTrue.x, centerTrue.y + slpTrue.y), centerTrue, Rotation).ToVectorF(),
                Modulate,
                SprSectionData[2]
                );
            //Top right
            verts[3] = new Vertex(
                Vector2.RotateVec(new Vector2(centerTrue.x + slpTrue.x, centerTrue.y - slpTrue.y), centerTrue, Rotation).ToVectorF(),
                Modulate,
                SprSectionData[3]
                );

            RenderStates state = new RenderStates();

            state.Texture = texture;

            state.BlendMode = BlendMode.None;

            state.Transform = Transform.Identity;

            args.w.Draw(verts, state);

            verts.Dispose();
        }
    }

    public TextureHolder TryCasheTexture()
    {
        if(SprChanged)
        {
            CurrTexture = TextureHolder.GetTexture(ref TexturePath);

            SprChanged = false;
        }

        return CurrTexture;
    }

    public void SolveNoSprSection(Vector2u sizeU)
    {
        if(WholeSprite)
        {
            var size = (Vector2f)sizeU;

            SprSectionData = new Vector2f[]
            {
                //Top left
                new Vector2f(0f,0f),
                //Bottom left
                new Vector2f(0f,size.Y),
                //Bottom right
                size,
                //Top right
                new Vector2f(size.X,0f),
            };

            WholeSprite = false;

            Console.WriteLine("WHOLE");
        }
    }

    public void DrawOptimizables(RenderArgs args, DrawableObject[] dObjects, uint index, uint count)
    {
        throw new System.NotImplementedException();
    }

    public bool Optimizable(DrawableObject obj)
    {
        return false;
    }

    public void ChangeTexture(ref string newTexture)
    {
        TexturePath = newTexture;

        bool actualTexture = TexturePath != "" && TexturePath != null;

        SprChanged = actualTexture;

        WholeSprite = actualTexture;

        if(!actualTexture)
        {
            CurrTexture = null;
        }
    }

    public void ChangeTexture(ref string newTexture, Vector2 sprSectionTopLeft, Vector2 sprSectionBottomRight)
    {
        TexturePath = newTexture;

        var actualTexture = TexturePath != "" && TexturePath != null;

        SprChanged = actualTexture;

        WholeSprite = actualTexture;

        if(!actualTexture)
        {
            CurrTexture = null;
            return;
        }
        
        TreatSprSections(sprSectionTopLeft, sprSectionBottomRight);
    }

    public void ChangeTexture(ref string newTexture, Vector2u sprSectionTopLeft, Vector2u sprSectionBottomRight)
    {
        TexturePath = newTexture;

        var actualTexture = TexturePath != "" && TexturePath != null;

        SprChanged = actualTexture;

        WholeSprite = actualTexture;

        if(!actualTexture)
        {
            CurrTexture = null;
            return;
        }

        TreatSprSections(new Vector2((int)sprSectionTopLeft.X, (int)sprSectionTopLeft.Y), new Vector2((int)sprSectionBottomRight.X, (int)sprSectionBottomRight.Y));
    }

    public void ChangePosition (Vector2 newPos)
    {
        SprDrawData[0] = newPos;
    }

    public void ChangeSize (Vector2 newSize)
    {
        SprDrawData[1] = newSize;
    }

    public void ChangeModulate (Color color)
    {
        Modulate = color;
    }

    public void ChangeRotation (FInt rotDegrees)
    {
        Rotation = rotDegrees;
    }

    public void ChangeBoundries (Vector2u topLeft, Vector2u bottomRight)
    {
        TreatSprSections(new Vector2((int)topLeft.X, (int)topLeft.Y), new Vector2((int)bottomRight.X, (int)bottomRight.Y));
    }

    private void TreatSprSections(Vector2 topLeft, Vector2 bottomRight)
    {
        var TLF = topLeft.ToVectorF();
        var BRF = bottomRight.ToVectorF();

        SprSectionData = new Vector2f[]
        {
            //Top left
            new Vector2f(TLF.X,TLF.Y),
            //Bottom left
            new Vector2f(TLF.X,BRF.Y),
            //Bottom right
            BRF,
            //Top right
            new Vector2f(BRF.X,TLF.Y),
        };

        WholeSprite = false;
    }
}

public enum UIAdjustmentMode
{
    ///<summary>
    ///Scales the UI against the smallest axis of the screen border.
    ///</summary>
    Compact = 0,
    ///<summary>
    ///Scales the UI against both axis of the screen border.
    ///</summary>
    Extended = 1
}