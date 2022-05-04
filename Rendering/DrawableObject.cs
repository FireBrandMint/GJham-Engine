using SFML.Window;
using SFML.Graphics;
using SFML.System;


public interface DrawableObject
{
    int z {get;set;}

    void Draw(RenderWindow window, float lerp, Vector2f windowSize);

    bool IsOptimizable(DrawableObject obj);

    int Optimize (DrawableObject[] objs, int currIndex, float lerp, RenderWindow w);
}