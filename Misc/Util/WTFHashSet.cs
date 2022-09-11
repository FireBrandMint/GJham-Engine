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
public class WTFHashSet
{
    //int LastGotHash = 0;
    //int LastGot = 0;

    List<int> MasterList;

    int Lenght = 0;

    public WTFHashSet()
    {
        MasterList = new List<int>();
    }

    public WTFHashSet(int capacity)
    {
        MasterList = new List<int>(capacity);
    }

    public void Add(int key)
    {
        int keyTrue = key;

        var addValu = key;

        if(MasterList.Count == 0 || MasterList[Lenght - 1] < keyTrue)
        {
            MasterList.Add(addValu);

            ++Lenght;

            return;
        }

        var searchResult = Find(keyTrue);

        int indexFirst = searchResult[0];

        int indexLast = searchResult[1];

        bool oneIndexScenario = indexFirst == indexLast;

        if (oneIndexScenario)
        {
            int indKey = MasterList[indexFirst];

            //If is less.
            if(indKey > keyTrue)
            {
                //insert on the left
                MasterList.Insert(indexFirst, addValu);
            }
            else
            {
                int keyNode = MasterList[indexFirst];
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

            int keyNode = MasterList[indexFirst];
            
            //If its equal, EXCEPTION.
            if(keyNode == keyTrue)
            {
                throw new Exception("Same hash code.");
            }

            //It is lower then, insert it on the front.
            MasterList.Insert(indexLast, addValu);
        }

        ++Lenght;
    }

    /// <summary>
    /// Ads if value didn't exist in hash table, returns wether the item existed.
    /// </summary>
    public bool AddIfNonexist(int key)
    {
        int keyTrue = key;

        var addValu = key;

        if(MasterList.Count == 0 || MasterList[Lenght - 1] < keyTrue)
        {
            MasterList.Add(addValu);

            ++Lenght;

            return false;
        }

        var searchResult = Find(keyTrue);

        int indexFirst = searchResult[0];

        int indexLast = searchResult[1];

        int indKey = MasterList[indexFirst];

        if(indKey == keyTrue) return true;

        bool oneIndexScenario = indexFirst == indexLast;

        if (oneIndexScenario)
        {

            //If is less.
            if(indKey > keyTrue)
            {
                //insert on the left
                MasterList.Insert(indexFirst, addValu);
            }
            else
            {
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

            //It is lower then, insert it on the front.
            MasterList.Insert(indexLast, addValu);
        }

        ++Lenght;

        return false;
    }

    public void Remove (int key)
    {
        int keyTrue = key;

        var searchResult = Find(keyTrue);

        int index = searchResult[0];

        var node = MasterList[index];

        if(node != keyTrue) throw new Exception($"KEY VALUE '{keyTrue}' DOESN'T EXIST");

        MasterList.RemoveAt(index);

        --Lenght;
    }

    public bool ContainsKey(int key)
    {
        if(Lenght == 0) return false;

        int keyTrue = key;

        var searchResult = Find(keyTrue);

        int indexFirst = searchResult[0];

        return keyTrue == MasterList[indexFirst];
    }
    public int[] GetKeys()
    {
        int mc = MasterList.Count;

        int[] arr = new int[mc];

        for(int i = 0; i < mc; ++i)
        {
            arr[i] = MasterList[i];
        }

        return arr;
    }

    public List<int> GetInternalList()
    {
        return MasterList;
    }

    private int[] Find (int key)
    {
        //Search algorithm intended to sqrt the amount
        //of searches.....

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

        bool leftHigher = MasterList[leftIndexLast] > key;
        bool rightHigher = MasterList[leftIndexLast+1] > key;


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

    public void Clear()
    {
        MasterList.Clear();
        Lenght = 0;
    }

    public int Count => Lenght;
}