using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//A list that will certainly be written in a way
//there's no way you can understand wtf
//it does, fuck you, let's get writing
public class WTFList<T>
{
    //what the hell is this, and what am i doing
    //isn't this just a dictionary?

    int IDNext = 0;

    List<WTFValue<T>> MasterList = new List<WTFValue<T>>();

    public WTFList()
    {
        MasterList = new List<WTFValue<T>>();
    }

    public WTFList(int capacity)
    {
        MasterList = new List<WTFValue<T>>(capacity);
    }

    public WTFIdentifier Add(int key, T value)
    {
        int id = GetID();

        WTFIdentifier identifier = new WTFIdentifier(key, id);

        WTFValue<T> addValu = new WTFValue<T>(value, key, id);

        if(MasterList.Count == 0)
        {
            MasterList.Add(addValu);

            return identifier;
        }

        var searchResult = Find(key);

        int indexFirst = searchResult[0];

        int indexLast = searchResult[1];

        bool oneIndexScenario = indexFirst == indexLast;

        if (oneIndexScenario)
        {
            int indKey = MasterList[indexFirst].key;

            if(indKey > key)
            {
                //insert on the left
                MasterList.Insert(indexFirst, addValu);
            }
            else
            {
                WTFValue<T> curr = MasterList[indexFirst];
                //If was found
                if(curr.key == key)
                {
                    while(curr.Down != null)
                    {
                        curr = curr.Down;
                    }

                    curr.Down = addValu;
                    return identifier;
                }

                //if is last
                if(indexFirst == MasterList.Count - 1)
                {
                    MasterList.Add(addValu);
                }
                else MasterList.Insert(indexFirst + 1, addValu);
            }
        }
        else
        {
            //If its the same as the value, add it.
            WTFValue<T> curr = MasterList[indexFirst];
            
            if(curr.key == key)
            {
                while(curr.Down != null)
                {
                    curr = curr.Down;
                }

                curr.Down = addValu;
                return identifier;
            }

            MasterList.Insert(indexLast, addValu);
        }

        return identifier;
    }

    public void Remove (WTFIdentifier indentifier)
    {
        var searchResult = Find(indentifier.Key);

        int index = searchResult[0];

        var node = MasterList[index];

        if(node.key != indentifier.Key) throw new Exception($"KEY VALUE '{indentifier.Key}' DOESN'T EXIST");

        int ID = indentifier.ID;

        bool passedOne = false;

        var lastNode = node;

        while(node != null)
        {
            if(node.ID == ID)
            {
                if(passedOne)
                {
                    if(node.Down == null)
                    {
                        lastNode.Down = null;
                    }
                    else
                    {
                        lastNode.Down = node.Down;
                        node.Down = null;
                    }
                }
                else
                {
                    if(node.Down == null)
                    {
                        MasterList.RemoveAt(index);
                    }
                    else
                    {
                        MasterList[index] = node.Down;
                        node.Down = null;
                    }
                }

                return;
            }

            passedOne = true;
            lastNode = node;
            node = node.Down;
        }

        throw new Exception($"ID VALUE '{ID}' DOESN'T EXIST");
    }

    public WTFNodeSlot<T> GetValuesOnKey(int key)
    {
        var searchResult = Find(key);

        int index = searchResult[0];

        var node = MasterList[index];

        if(node.key != key) return null;

        return new WTFNodeSlot<T>(node);
    }

    public int[] GetAllKeys()
    {
        int mc = MasterList.Count;

        int[] arr = new int[mc];

        for(int i = 0; i < mc; ++i)
        {
            arr[i] = MasterList[i].key;
        }

        return arr;
    }

    private int[] Find (int key)
    {
        //Search algorithm intended to sqrt the amount
        //of searches.....
        //WAIT WAIT WAIT WTF

        //DISCOVERY for me: sqrt is literally dividing by 2 until you
        //reach the value '1'

        int indexFirst = 0;

        int indexLast = MasterList.Count - 1;

        int leftIndexLast;

        loopStart:

        //If index first and index last are the same
        //then we have a 'one index', a scenario
        //where the value belongs either to the right
        //or to the left of the left of the index.
        if(indexFirst == indexLast)
        {
            goto endLoop;
        }

        leftIndexLast = (indexLast - indexFirst) >> 1;
        leftIndexLast += indexFirst;

        bool leftHigher = MasterList[leftIndexLast].key > key;
        bool rightHigher = MasterList[leftIndexLast+1].key > key;


        //If both are higher then the result is more to the left,
        //since the results that are not garanteed to be even higher than
        //the subject are there. That makes it become closer to the result.
        //If both are lower, then the result is more to the right,
        //since the results that are not garanteed to be even lower than
        //the subjects there. That makes it become closer to the result.
        //If the left is lower and the higher, then the result belongs in
        //the middle.
        if(leftHigher && rightHigher)
        {
            //CASE: both are higher
            indexLast = leftIndexLast;

            goto loopStart;
        }
        else if (!(leftHigher || rightHigher))
        {
            //CASE: both are lower
            indexFirst = leftIndexLast+1;

            goto loopStart;
        }
        //CASE: left is lower or equal and right is higher
        //aka end of operation.

        indexFirst = leftIndexLast;
        indexLast = leftIndexLast + 1;

        endLoop:

        return new int[2]
        {
            indexFirst, indexLast
        };
    }

    private int GetID()
    {
        int nowID = IDNext;

        ++IDNext;

        return nowID;
    }
}

public class WTFValue<T>
{
    public WTFValue<T> Down = null;

    public int key;

    public T Value;

    public int ID;

    public WTFValue (T valu, int k, int id)
    {
        Value = valu;

        key = k;

        ID = id;
    }
}

public struct WTFIdentifier
{
    public int Key;

    public int ID;

    public WTFIdentifier(int key, int id)
    {
        Key = key;

        ID = id;
    }
}

public class WTFNodeSlot<T>
{
    WTFValue<T> Node;

    public WTFNodeSlot(WTFValue<T> node)
    {
        Node = node;
    }

    public T GetValue()
    {
        var currNode = Node;
        Node = Node.Down;

        return currNode.Value;
    }
}