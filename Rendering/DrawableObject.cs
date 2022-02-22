using System.Drawing;


public interface DrawableObject
{
    PointF CurrPos {get;}

    PointF LastPos{get;}

    void Draw(Graphics g, double lerp);
}