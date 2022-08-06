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

    public long[] AddNode(T value, Vector2 topLeft, Vector2 bottomRight)
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

        return ranges;
    }

    public T[] GetValues (long[] ranges)
    {
        T[] container = new T[10];

        int cInd = 0, cSize = 10;

        long x1 = ranges[0], x2 = ranges[1], y1 = ranges[2], y2 = ranges[3];

        int ikX = InternalDict.GetInternalKey(x1);

        while (x1 <= x2)
        {
            var dictY = InternalDict.GetValueOfIK(ikX).Value;

            long y1f = y1;

            int ikY = InternalDict.GetInternalKey(y1f);

            while(y1f <= y2)
            {
                var node = dictY.GetValueOfIK(ikY).Value;

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

                y1f += Multiple;

                ikY += 1;
            }

            x1 += Multiple;

            ikX+=1;
        }

        Array.Resize(ref container, cInd + 1);

        for(int i = 0; i< container.Length; ++i)
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

        int ikX = InternalDict.GetInternalKey(x1);

        while (x1 <= x2)
        {
            var v1 = InternalDict.GetValueOfIK(ikX);

            var keyX = v1.Key;

            var dictY = v1.Value;

            long y1f = y1;

            int ikY = InternalDict.GetInternalKey(y1f);

            while(y1f <= y2)
            {
                var v2 = dictY.GetValueOfIK(ikY);

                var node = v2.Value;

                var keyY = v2.Key;

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
                    lastNode.down = node.down;

                    node.down = null;
                }

                y1f += Multiple;

                ikY += 1;
            }

            //if(cInd == cSize)
            //{
            //    cSize += 10;

            //    Array.Resize(ref container, cSize);
            //}
            //container[cInd] = valu;
            //++cInd;

            x1 += Multiple;

            ikX+=1;
        }

        for (int i = 0; i < cInd + 1; ++i)
        {
            var toRemove = container[i];

            var dictY = InternalDict[toRemove.keyX];

            dictY.Remove(toRemove.keyY);

            if(dictY.Count == 0)
            {
                //TODO: something with the empty dictionaries
                //that doesn't cause the computer pain,
                //because this does.

                InternalDict.Remove(toRemove.keyX);
            }
        }
    }

    public void HasValueOn(long x, long y)
    {

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