using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


//Class that isn't a failure (surprisingly)
//Can be used for col detection easely, gg.

///<summary>
///Sorted dictionary with sqrt search and minimum
///memory allocation.
///Works with fast binary search method that loops
///sqrt(DictionaryAmountOfValue) times.
///Developed with hate by yours truly,
///Guliver Jham.
///</summary>
public class WTFDictionary<K, T>
{

    public T this[K key]
    {
        get
        {
            int trueKey = key.GetHashCode();

            var search = Find(trueKey);

            var valu = MasterList[search[0]];

            #if DEBUG
            if(valu.Key != trueKey) throw new IndexOutOfRangeException();
            #endif

            return valu.Value;
        }
        set
        {
            int trueKey = key.GetHashCode();

            var search = Find(trueKey);

            #if DEBUG
            var valu = MasterList[search[0]];
            if(valu.Key != trueKey) throw new IndexOutOfRangeException();
            #endif

            MasterList[search[0]] = new KeyValuePair<int, T>(trueKey, value);
        }
    }

    List<KeyValuePair<int, T>> MasterList = new List<KeyValuePair<int, T>>();

    public WTFDictionary()
    {
        MasterList = new List<KeyValuePair<int, T>>();
    }

    public WTFDictionary(int capacity)
    {
        MasterList = new List<KeyValuePair<int, T>>(capacity);
    }

    public void Add(K key, T value)
    {
        int keyTrue = key.GetHashCode();

        var addValu = new KeyValuePair<int, T>(keyTrue, value);

        if(MasterList.Count == 0)
        {
            MasterList.Add(addValu);

            return;
        }

        var searchResult = Find(keyTrue);

        int indexFirst = searchResult[0];

        int indexLast = searchResult[1];

        bool oneIndexScenario = indexFirst == indexLast;

        if (oneIndexScenario)
        {
            int indKey = MasterList[indexFirst].Key;

            //If is less.
            if(indKey > keyTrue)
            {
                //insert on the left
                MasterList.Insert(indexFirst, addValu);
            }
            else
            {
                int keyNode = MasterList[indexFirst].Key;
                //If is same.
                if(keyNode == keyTrue)
                {
                    throw new Exception("Same hash code.");
                }

                //Not same therefore is higher.

                //if is last.
                if(indexFirst == MasterList.Count - 1)
                {
                    MasterList.Add(addValu);
                }
                else MasterList.Insert(indexFirst + 1, addValu);
            }
        }
        else
        {
            //Left is lower or equal.

            int keyNode = MasterList[indexFirst].Key;
            
            //If its equal, EXCEPTION.
            if(keyNode == keyTrue)
            {
                throw new Exception("Same hash code.");
            }

            //It is lower then, insert it on the front.
            MasterList.Insert(indexLast, addValu);
        }
    }

    public void Remove (K key)
    {
        int keyTrue = key.GetHashCode();

        var searchResult = Find(keyTrue);

        int index = searchResult[0];

        var node = MasterList[index];

        if(node.Key != keyTrue) throw new Exception($"KEY VALUE '{keyTrue}' DOESN'T EXIST");

        MasterList.RemoveAt(index);
    }

    public int[] GetAllKeysHash()
    {
        int mc = MasterList.Count;

        int[] arr = new int[mc];

        for(int i = 0; i < mc; ++i)
        {
            arr[i] = MasterList[i].Key;
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

        bool leftHigher = MasterList[leftIndexLast].Key > key;
        bool rightHigher = MasterList[leftIndexLast+1].Key > key;


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
        if (leftHigher == rightHigher)
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
}