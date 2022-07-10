using SFML.Window;
using SFML.Graphics;
using SFML.System;


public interface DrawableObject
{
    int z {get;set;}

    void Draw(RenderArgs args);

    bool Optimizable (DrawableObject obj);

    void DrawOptimizables(RenderArgs args, DrawableObject[] dObjects, uint index, uint count);
}