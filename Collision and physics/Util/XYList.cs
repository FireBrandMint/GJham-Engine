using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//TODO: try to finish this tommorow.

namespace GJham.Physics.Util;

public class XYList<T> where T:XYBoolHolder
{
    long Multiple;

    int CapacityY;

    EqualityComparer<T> EqComparer = EqualityComparer<T>.Default;

    WTFDictionary<long, WTFDictionary<long, OneWayNode<T>>> InternalDict;

    WTFDictionary<long, OneWayNode<T>> ReserveDictionary;

    ///<summary>
    ///'capacityY' should always be a very small number like 10.
    ///</summary>
    public XYList(int gridSeparation, int capacityX, int capacityY)
    {
        Multiple = gridSeparation;

        InternalDict = new WTFDictionary<long, WTFDictionary<long, OneWayNode<T>>>(capacityX);

        ReserveDictionary = new WTFDictionary<long, OneWayNode<T>>(CapacityY);

        CapacityY = capacityY;
    }

    public void AddNode(T value, Vector2 topLeft, Vector2 bottomRight)
    {
        var ranges = GetRanges(topLeft, bottomRight);

        long x1 = ranges[0], x2 = ranges[1], y1 = ranges[2], y2 = ranges[3];

        while (x1 <= x2)
        {
            bool dictExisted;
            var dictY = InternalDict.AddIfNonexist(x1, ReserveDictionary, out dictExisted);

            if(!dictExisted) ReserveDictionary = new WTFDictionary<long, OneWayNode<T>>(CapacityY);

            long y1f = y1;

            while(y1f <= y2)
            {
                bool nodeExisted;

                var valueNode = new OneWayNode<T>(value);

                var node = dictY.AddIfNonexist(y1f, valueNode, out nodeExisted);

                if(nodeExisted)
                {
                    node.Add(valueNode);
                }

                y1f += Multiple;
            }

            x1 += Multiple;
        }
    }

    ///<summary>
    ///Gets x1,x2,y1,y2;
    ///</summary>
    private long[] GetRanges(Vector2 topLeft, Vector2 bottomRight)
    {
        long[] XYR = new long[4]
        {
            (long)topLeft.x,
            (long)bottomRight.x,
            (long)topLeft.y,
            (long)bottomRight.y
        };

        for(int i = 0; i < 4; ++i)
        {
            long mod = XYR[i] % Multiple;

            XYR[i] -= mod;
        }

        return XYR;
    }
}

class OneWayNode<T>
{
    public T Value;

    public OneWayNode<T> down;

    public OneWayNode(T value)
    {
        Value = value;

        down = null;
    }

    public void Add(T value)
    {
        var oneNode = this;

        while(oneNode.down != null)
        {
            oneNode = oneNode.down;
        }

        oneNode.down = new OneWayNode<T>(value);
    }

    public void Add(OneWayNode<T> value)
    {
        var oneNode = this;

        while(oneNode.down != null)
        {
            oneNode = oneNode.down;
        }

        oneNode.down = value;
    }
}