using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//TODO: try to finish this tommorow.

public class XYList<T>
{
    long Multiple;

    public XYList(int gridSeparation)
    {
        Multiple = gridSeparation;
    }

    public void AddNode(T value, Vector2 topLeft, Vector2 bottomRight)
    {
        //WHATTODO: finished the node integer calc
        //everything else to go.
        long[] firstXY = new long[2]
        {
            (long)topLeft.x,
            (long)topLeft.y
        };

        for(int i = 0; i < 2; ++i)
        {
            long first = firstXY[i];
            long modF = first % Multiple;

            if(first < 0)
            firstXY[i] += modF;
            else
            firstXY[i] -= modF;
        }

        long[] lastXY = new long[2]
        {
            (long)bottomRight.x,
            (long)bottomRight.y
        };

        for(int i = 0; i < 2; ++i)
        {
            long last = lastXY[i];
            if (last < bottomRight[i]) last += 1;

            long mod = last % Multiple;

            long final = mod - Multiple;

            if(last < 0)
            lastXY[i] += final;
            else
            lastXY[i] -= final;
        }
    }
}