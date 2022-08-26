using SFML.Graphics;
using SFML.System;

public class UIText2D
{
    public int z {get; set;}

    bool Changed = true;

    string FontPath;

    string TextString = "Lorem Lpsum";

    int TextSpacing = 2;

    int LineSpacing = 3;

    int TextSize = 12;

    Vector2 Position, PastPosition;

    Vector2 Scale = new Vector2(1,1);

    FInt Rotation = (FInt)0;

    Font TFont = null;

    Color TextColor = Color.White;

    Text DObject = null;

    UIAdjustmentMode AdjustmentMode;

    public UIText2D(string fontPath, Vector2 position, UIAdjustmentMode adjMode)
    {
        FontPath = fontPath;

        Position = position;
        PastPosition = position;

        AdjustmentMode = adjMode;
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

            var currPos = Position.ToVectorF();

            var visualScale = args.windowView;

            if(AdjustmentMode == UIAdjustmentMode.Compact)
            {
                if(visualScale.X > visualScale.Y) visualScale.X = visualScale.Y;
                else visualScale.Y = visualScale.X;

                currPos = (new Vector2f(currPos.X * visualScale.X, currPos.Y * visualScale.Y)) / 100.0f;
            }
            else if (AdjustmentMode == UIAdjustmentMode.Extended)
            {
                currPos = (new Vector2f(currPos.X * visualScale.X, currPos.Y * visualScale.Y)) / 100.0f;
            }

            var finalPos = currPos;

            float vScale = args.windowView.X;

            if(vScale > args.windowView.Y) vScale = args.windowView.Y;

            DObject.Position = finalPos;

            DObject.Scale = ((Scale.ToVectorF() * vScale)/1000.0f);

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