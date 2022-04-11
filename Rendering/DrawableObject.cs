using SFML.Window;
using SFML.Graphics;


public interface DrawableObject
{
    int z {get;set;}

    void Draw(RenderWindow window, double lerp);

    bool IsOptimizable(DrawableObject obj);

    int Optimize (DrawableObject[] objs, int currIndex, double lerp, RenderWindow w);
}