using SFML.Window;
using SFML.Graphics;
using SFML.System;


public interface DrawableObject
{
    int z {get;set;}

    void Draw(RenderWindow window, float lerp, Vector2f windowSize);

    bool Optimizable (DrawableObject obj);

    void DrawOptimizables(RenderWindow window, DrawableObject[] dObjects, uint index, uint count, float lerp);
}