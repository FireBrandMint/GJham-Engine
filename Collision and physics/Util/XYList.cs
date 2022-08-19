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

    WTFDictionary<long, WTFDictionary<long, OneWayNode<T>>> DictX;

    WTFDictionary<long, OneWayNode<T>> ReserveDictionary;

    ///<summary>
    ///'capacityY' should always be a very small number like 10.
    ///</summary>
    public XYList(int gridSeparation, int capacityX, int capacityY)
    {
        Multiple = gridSeparation;

        DictX = new WTFDictionary<long, WTFDictionary<long, OneWayNode<T>>>(capacityX);

        ReserveDictionary = new WTFDictionary<long, OneWayNode<T>>(CapacityY);

        CapacityY = capacityY;
    }

    public long[] AddNode(T value, Vector2 topLeft, Vector2 bottomRight)
    {
        var ranges = GetRanges(topLeft, bottomRight);

        long x1 = ranges[0], x2 = ranges[1], y1 = ranges[2], y2 = ranges[3];

        while (x1 <= x2)
        {
            bool dictExisted;
            var dictY = DictX.AddIfNonexist(x1, ReserveDictionary, out dictExisted);

            if(!dictExisted) ReserveDictionary = new WTFDictionary<long, OneWayNode<T>>(CapacityY);

            long y1f = y1;

            while(y1f <= y2)
            {
                bool nodeExisted;

                var valueNode = new OneWayNode<T>(value, value.ID);

                var node = dictY.AddIfNonexist(y1f, valueNode, out nodeExisted);

                if(nodeExisted)
                {
                    if(valueNode.Priority < node.Priority)
                    {
                        valueNode.Append(node);
                        dictY[y1f] = valueNode;
                    }
                    else node.Add(valueNode);
                }

                y1f += 1;
            }

            x1 += 1;
        }

        return ranges;
    }

    public long[] AddNode(T value, long[] ranges)
    {
        long x1 = ranges[0], x2 = ranges[1], y1 = ranges[2], y2 = ranges[3];

        //while (x1 <= x2)
        for(; x1 <= x2; ++x1)
        {
            bool dictExisted;
            var dictY = DictX.AddIfNonexist(x1, ReserveDictionary, out dictExisted);

            if(!dictExisted) ReserveDictionary = new WTFDictionary<long, OneWayNode<T>>(CapacityY);


            for(long y1f = y1; y1f <= y2; ++y1f)
            {
                bool nodeExisted;

                var valueNode = new OneWayNode<T>(value, value.ID);

                var node = dictY.AddIfNonexist(y1f, valueNode, out nodeExisted);

                if(nodeExisted)
                {
                    if(valueNode.Priority < node.Priority)
                    {
                        valueNode.Append(node);
                        dictY[y1f] = valueNode;
                    }
                    else node.Add(valueNode);
                }
            }
        }

        if(x1 != x2 + 1) throw new Exception("Lol???");

        //Console.WriteLine($"Added x: {x1}-{x2}, y: {y1}-{y2}");

        return ranges;
    }

    public T[] GetValues (long[] ranges)
    {
        T[] container = new T[10];

        int cInd = 0, cSize = 10;

        long x1 = ranges[0], x2 = ranges[1], y1 = ranges[2], y2 = ranges[3];

        for(; x1 <= x2; ++x1)
        {

            var dictY = DictX[x1];

            long y1f = y1;

            while(y1f <= y2)
            {
                var node = dictY[y1f];

                while(node!= null)
                {
                    T valu = node.Value;

                    if(!valu.SelectedC)
                    {
                        valu.SelectedC = true;

                        if(cInd == cSize)
                        {
                            cSize += 10;

                            Array.Resize(ref container, cSize);
                        }

                        container[cInd] = valu;

                        ++cInd;
                    }

                    node = node.down;
                }

                y1f += 1;
            }
        }

        Array.Resize(ref container, cInd);

        for(int i = 0; i< cInd; ++i)
        {
            container[i].SelectedC = false;
        }

        return container;
    }

    public void RemoveValue (long[] ranges, T value)
    {
        (int keyX, int keyY)[] container = new (int keyX, int keyY)[10];

        int cInd = 0, cSize = 10;

        long x1 = ranges[0], x2 = ranges[1], y1 = ranges[2], y2 = ranges[3];

        //int ikX = DictX.GetInternalKey(x1);

        for(; x1 <= x2; x1 += 1)
        {
            //var v1 = DictX.GetValueOfIK(ikX);

            var keyX = (int)x1;

            var dictY = DictX[x1];

            //int ikY = dictY.GetInternalKey(y1);

            for(long y1f = y1; y1f <= y2; y1f += 1)
            {
                var node = dictY[y1f];

                var keyY = (int)y1f;

                bool first = node.down == null;

                OneWayNode<T> lastNode = null;

                while(node.down != null)
                {
                    if(EqComparer.Equals(value, node.Value)) break;

                    lastNode = node;
                    node = node.down;
                }

                if(first)
                {
                    if(cInd == cSize)
                    {
                        cSize += 10;

                        Array.Resize(ref container, cSize);
                    }

                    container[cInd] = (keyX, keyY);

                    ++cInd;
                }
                else
                {
                    if(lastNode == null)
                    {
                        //dictY.SetValueOfIK(ikY, node.down);
                        dictY[y1f] = node.down;
                        node.down = null;
                    }
                    else
                    {
                        lastNode.down = node.down;

                        node.down = null;
                    }
                }

                //ikY += 1;
            }

            //ikX+=1;
        }

        if(cInd > 0)for (int i = 0; i < cInd; ++i)
        {
            var toRemove = container[i];

            var dictY = DictX[toRemove.keyX];

            dictY.Remove(toRemove.keyY);

            if(dictY.Count == 0)
            {
                //TODO: something with the empty dictionaries
                //that doesn't cause the computer pain,
                //because this does.

                DictX.Remove(toRemove.keyX);
            }
        }
    }

    ///<summary>
    ///Gets x1,x2,y1,y2;
    ///</summary>
    public long[] GetRanges(Vector2 topLeft, Vector2 bottomRight)
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
            XYR[i] /= Multiple;
        }

        return XYR;
    }
}

class OneWayNode<T>
{
    public T Value;

    public int Priority;

    public OneWayNode<T> down;

    public OneWayNode(T value, int priority)
    {
        Value = value;

        Priority = priority;

        down = null;
    }

    public void Add(OneWayNode<T> value)
    {
        OneWayNode<T> lastNode = null;

        OneWayNode<T> oneNode = this;

        while(oneNode.down != null)
        {
            if(value.Priority < oneNode.Priority)
            {
                value.down = oneNode;
                lastNode.down = value;
            }
            lastNode = oneNode;
            oneNode = oneNode.down;
        }

        oneNode.down = value;
    }

    public void Append(OneWayNode<T> value)
    {
        down = value;
    }
}