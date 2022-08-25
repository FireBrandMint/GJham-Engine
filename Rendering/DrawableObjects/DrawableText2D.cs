using SFML.Graphics;

public class DrawableText2D : DrawableObject
{
    public int z {get; set;}

    bool Changed = true;

    string FontPath;

    string TextString = "";

    int TextSpacing = 1;

    int LineSpacing = 1;

    int TextSize = 12;

    Vector2 Position, PastPosition;

    Vector2 Scale = new Vector2(1,1);

    FInt Rotation = (FInt)0;

    Font TFont = null;

    Color TextColor = Color.White;

    Text DObject = null;

    public DrawableText2D(string fontPath, Vector2 position)
    {
        FontPath = fontPath;

        Position = position;
        PastPosition = position;
    }

    public void Draw(RenderArgs args)
    {
        if(TFont == null)
        {
            TFont = FontHolder.GetFont(FontPath);

            if(TFont == null) return;
        }

        if(DObject == null)
        {
            DObject = new Text();
        }

        if(Changed)
        {
            if(DObject.Font != TFont) DObject.Font = TFont;

            var currPos = (Position - args.cameraPos).ToVectorF();
            var lastPos = (PastPosition - args.cameraPos).ToVectorF();

            var finalPos = RenderMath.Lerp(lastPos, currPos, args.lerp);

            DObject.Position = finalPos;

            DObject.Scale = Scale.ToVectorF();

            DObject.Rotation = Rotation.ToFloat();

            DObject.FillColor = TextColor;

            DObject.DisplayedString = TextString;

            DObject.LetterSpacing = TextSpacing;

            DObject.LineSpacing = LineSpacing;

            DObject.CharacterSize = (uint)TextSize;

            Changed = false;
        }

        args.w.Draw(DObject);
    }

    public void DrawOptimizables(RenderArgs args, DrawableObject[] dObjects, uint index, uint count)
    {
        throw new System.NotImplementedException();
    }

    public bool Optimizable(DrawableObject obj)
    {
        return false;
    }

    public void SetPosition(Vector2 currVec, Vector2 pastVec)
    {
        Position = currVec;
        PastPosition = pastVec;

        Changed = true;
    }

    public void ChangeFont(string font)
    {
        FontPath = font;
        TFont = null;

        Changed = true;
    }

    public void ChangeScale(Vector2 scale)
    {
        Scale = scale;
        Changed = true;
    }
    public void ChangeRotation (FInt rotation)
    {
        Rotation = rotation;
        Changed = true;
    }

    public void ChangeColor (Color color)
    {
        TextColor = color;
        Changed = true;
    }

    public void ChangeText (string text)
    {
        TextString = text;
        Changed = true;
    }

    public void ChangeLetterSpacing(int pixels)
    {
        TextSpacing = pixels;
        Changed = true;
    }

    public void ChangeLineSpacing(int pixels)
    {
        LineSpacing = pixels;
        Changed = true;
    }

    public void ChangeCharacterSize(int pixels)
    {
        TextSize = pixels;
        Changed = true;
    }
}