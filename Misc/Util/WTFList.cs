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

    List<WTFValue> MasterList = new List<WTFValue>();

    public WTFList()
    {
        MasterList = new List<WTFValue>();
    }

    public WTFList(int capacity)
    {
        MasterList = new List<WTFValue>(capacity);
    }

    public void Add(int key, T value)
    {
        WTFValue addValu = new WTFValue(value, key);

        if(MasterList.Count == 0) MasterList.Add(addValu);

        //Search algorithm intended to sqrt the amount
        //of searches.....
        //WAIT WAIT WAIT WTF

        //DISCOVERY for me: sqrt is literally dividing by 2 until you
        //reach the value '1'

        int indexFirst = 0;

        int indexLast = MasterList.Count - 1;

        int leftIndexLast;

        bool oneIndexScenario = false;

        loopStart:

        //If index first and index last are the same
        //then we have a 'one index', a scenario
        //where the value belongs either to the right
        //or to the left of the left of the index.
        if(indexFirst == indexLast)
        {
            oneIndexScenario = true;

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
        //CASE: left is lower and right is higher
        //aka end of operation.

        indexFirst = leftIndexLast;
        indexLast = leftIndexLast + 1;

        endLoop:

        if (oneIndexScenario)
        {
            int indKey = MasterList[indexFirst].key;

            if(indKey > key)
            {
                //insert on the left
                MasterList.Insert(indexFirst, new WTFValue(value, key));
            }
            else
            {
                //insert to the right
                if(indexFirst == MasterList.Count - 1) MasterList.Add(addValu);
                else MasterList.Insert(indexFirst + 1, addValu);
            }
        }
        else
        {
            MasterList.Insert(indexLast, addValu);
        }
    }

    private class WTFValue
    {
        public WTFValue Down = null;

        public int key;

        public T Value;

        public WTFValue (T valu, int k)
        {
            Value = valu;

            key = k;
        }
    }
}