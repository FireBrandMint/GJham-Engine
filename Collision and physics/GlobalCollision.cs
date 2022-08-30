using System;
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

        CollisionResult result = new CollisionResult();

        for(int i1 = 0; i1 < colObjs.Length; ++i1)
        {
            var currShape = colObjs[i1];

            if(!currShape.IsActive()) continue;

            var closeShapes = Shape.GetShapesInGrid(currShape);

            if(currShape.Solid)
            {
                for (int i2 = 0; i2 < closeShapes.Length; ++i2)
                {
                    var secondShape = closeShapes[i2];

                    if(currShape == secondShape || !secondShape.IsActive()) continue;

                    if(secondShape.Solid)
                    {
                        currShape.IntersectsInfo(secondShape, ref result);

                        if(!result.Intersects) continue;

                        int repetition = currShape.CollisionRepetition;
                        if(repetition < secondShape.CollisionRepetition) repetition = secondShape.CollisionRepetition;

                        if(secondShape.ObjectUsingIt == null) throw new Exception("No object using the shape.");

                        var currUsingIt = currShape.ObjectUsingIt;
                        var secondUsingIt = secondShape.ObjectUsingIt;

                        currUsingIt.ResolveOverlap(secondUsingIt, ref result);

                        result.Separation *= -1;
                        result.SeparationDirection *= -1;

                        secondUsingIt.ResolveOverlap(currUsingIt, ref result);

                        repetition -=1;

                        for(int r = 0; r< repetition; ++r)
                        {
                            CollisionResult resultR = new CollisionResult();

                            currShape.IntersectsInfo(secondShape, ref resultR);

                            if (!resultR.Intersects) continue;

                            currUsingIt.ResolveOverlap(secondUsingIt, ref resultR);

                            resultR.Separation *= -1;
                            resultR.SeparationDirection *= -1;

                            secondUsingIt.ResolveOverlap(currUsingIt, ref resultR);

                            result.Separation += resultR.Separation;
                        }

                        result.Separation *= -1;
                        result.SeparationDirection *= -1;

                        currUsingIt.OnCollision(secondUsingIt, ref result);

                        result.Separation *= -1;
                        result.SeparationDirection *= -1;

                        secondUsingIt.OnCollision(currUsingIt, ref result);
                    }
                    else
                    {
                        bool intersects = currShape.Intersect(secondShape);
                        if(!intersects) continue;
                        result.Intersects = intersects;

                        currShape.ObjectUsingIt.OnCollision(secondShape.ObjectUsingIt, ref result);
                    }
                }

            }
            else
            {
                for (int i2 = 0; i2 < closeShapes.Length; ++i2)
                {
                    var secondShape = closeShapes[i2];

                    bool intersects = currShape.Intersect(secondShape);
                    if(!intersects) continue;
                    result.Intersects = intersects;

                    currShape.ObjectUsingIt.OnCollision(secondShape.ObjectUsingIt, ref result);
                }
            }
        }
    }

    public static void AddShape(int id, Shape shape)
    {
        ShapesDetecting.Add(id, shape);
    }

    public static void RemoveShape(int id)
    {
        ShapesDetecting.Remove(id);
    }
}