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

    public int Optimize (DrawableObject[] objs, int currIndex, double lerp, RenderWindow w)
    {
        throw new NotImplementedException("Method not implemented!");
    }

    public void Draw(RenderWindow w, double lerp)
    {
        Image.Position = RenderMath.Lerp(LastPos, CurrPos, (float) lerp);

        w.Draw(Image);
    }
}