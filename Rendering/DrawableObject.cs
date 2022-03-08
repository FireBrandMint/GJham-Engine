using System.Drawing;


public interface DrawableObject
{
    int z {get;set;}

    void Draw(Graphics g, double lerp);
}