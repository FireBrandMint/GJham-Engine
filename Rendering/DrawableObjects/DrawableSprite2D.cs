using SFML.Graphics;
using SFML.System;
using System;

public class DrawableSprite2D : DrawableObject
{

    Sprite Image;

    Vector2f LastPos, CurrPos;

    public int z {get;set;}

    public DrawableSprite2D (Sprite _sprite, Vector2 lastPos, Vector2 currPos)
    {
        Image = _sprite;
        
        LastPos = lastPos.ToVectorF();
        CurrPos = currPos.ToVectorF();
    }

    public bool IsOptimizable (DrawableObject obj)
    {
        return false;
    }

    public int Optimize (DrawableObject[] objs, int currIndex, float lerp, RenderWindow w)
    {
        throw new NotImplementedException("Method not implemented!");
    }

    public void Draw(RenderWindow w, float lerp, Vector2f windowSize)
    {
        Image.Position = RenderMath.Lerp(LastPos, CurrPos, (float) lerp);

        FloatRect imgSize = (FloatRect)Image.TextureRect;

        Vector2f imgScale = RenderMath.Multiply(new Vector2f (imgSize.Width, imgSize.Height), Image.Scale);

        FloatRect imgRect = new FloatRect(Image.Position, imgScale);

        FloatRect canvasRect = new FloatRect(new Vector2f(), windowSize);

        if (canvasRect.Intersects(imgRect)) w.Draw(Image);
    }

    public bool Optimizable (DrawableObject obj)
    {
        return false;
    }

    public void DrawOptimizables(RenderWindow window, DrawableObject[] dObjects, uint index, uint count, float lerp)
    {

    }
}