using SFML.System;
using SFML.Graphics;

public class UISprite2D : DrawableObject
{
    public int z { get; set; }

    Texture CurrTexture = null;

    bool SprChanged = true;

    bool WholeSprite = true;

    string TexturePath;

    Vector2 [] SprDrawData = null;

    Vector2f [] SprSectionData = null;

    public UISprite2D (string texturePath, Vector2 center, Vector2 sprLengthsPercent)
    {
        ChangeTexture(ref texturePath);

        SprDrawData = new Vector2[]
        {
            center, sprLengthsPercent
        };
    }

    public void Draw(RenderArgs args)
    {
        VertexArray verts = new VertexArray(PrimitiveType.Quads, 4u);

        var texture = TryCasheTexture();

        if(texture == null) return;

        SolveNoSprSection();

        var vSize = args.windowView;

        var center = SprDrawData[0].ToVectorF();

        var slp = (SprDrawData[1]).ToVectorF()/2;

        center = new Vector2f(vSize.X * center.X, vSize.Y * center.Y);

        slp = new Vector2f(vSize.X * slp.X, vSize.Y * slp.Y);

        //Top left
        verts[0] = new Vertex(new Vector2f(center.X - slp.X, center.Y - slp.Y), SprSectionData[0]);
        //Bottom left
        verts[1] = new Vertex(new Vector2f(center.X - slp.X, center.Y + slp.Y), SprSectionData[1]);
        //Bottom right
        verts[2] = new Vertex(new Vector2f(center.X + slp.X, center.Y + slp.Y), SprSectionData[2]);
        //Top right
        verts[3] = new Vertex(new Vector2f(center.X + slp.X, center.Y - slp.Y), SprSectionData[3]);

        RenderStates state = new RenderStates();

        state.Texture = texture;

        state.BlendMode = BlendMode.None;

        args.w.Draw(verts, state);

        verts.Dispose();
    }

    public Texture TryCasheTexture()
    {
        if(SprChanged)
        {
            CurrTexture = TextureHolder.GetTexture(ref TexturePath);

            SprChanged = false;
        }

        return CurrTexture;
    }

    public void SolveNoSprSection()
    {
        if(WholeSprite)
        {
            var size = (Vector2f)CurrTexture.Size;

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

        var actualTexture = TexturePath != "" && TexturePath != null;

        SprChanged = actualTexture;

        WholeSprite = actualTexture;

        if(!actualTexture) CurrTexture = null;
    } 
}