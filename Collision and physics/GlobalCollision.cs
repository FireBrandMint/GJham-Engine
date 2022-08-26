using System.Collections.Generic;

public class GlobalCollision
{
    //Ordered dictionary of the list of shapes testing for collision
    //Key is the ID of the object that registered it to make it deterministic
    //Value is the shape in question.
    public static WTFDictionary<int,Shape> ShapesDetecting = new WTFDictionary<int,Shape>(100);

    public static void Tick()
    {
        var colObjs = ShapesDetecting.GetValues();
        for(int i = 0; i < colObjs.Length; ++i)
        {
            var currShape = colObjs[i];

            if(!currShape.IsActive()) continue;

            var closeShapes = Shape.GetShapesInGrid(currShape);

            for(int o = 0; o < currShape.CollisionRepetition; ++o)
            {
                //TODO: detect collisions on closeShapes
                //if they aren't currShape and their 'IsActive()' is true.
                //If one of them collides, execute, if not null,
                //ObjectUsingIt.OnCollision().
            }
        }
    }

    public void AddShape(int id, Shape shape)
    {
        ShapesDetecting.Add(id, shape);
    }

    public void RemoveShape(int id)
    {
        ShapesDetecting.Remove(id);
    }
}