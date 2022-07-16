using SFML.Window;
using SFML.Graphics;
using SFML.System;

///<summary>
///This is the interface of the objects that are sent to the
///main window (of class Canvas) to be rendered.
///These objects render themselves using it's functions, you can read
///what they do from the comments i made.
///</summary>
public interface DrawableObject
{
    ///<summary>
    ///z is the layer the object will be drawn.
    ///</summary>
    int z {get;set;}

    ///<summary>
    ///This draws the current object into the window.
    ///</summary>
    void Draw(RenderArgs args);

    ///<summary>
    ///Determines if a drawable object is
    ///able to be batched with another or not.
    ///Used to determine how many of the next
    ///objects are gonna be batched and drawn with
    ///the DrawOptimizables function, if there is no
    ///batchable object next in the list, it just draws
    ///with this one with Draw.
    ///</summary>
    bool Optimizable (DrawableObject obj);

    ///<summary>
    ///The amount 'count' of objects starting at
    ///'index' are drawable by batching, therefore this is meant
    ///to draw all of those by doing just that.
    ///</summary>
    void DrawOptimizables(RenderArgs args, DrawableObject[] dObjects, uint index, uint count);
}