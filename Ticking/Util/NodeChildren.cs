using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class NodeChildren<T>
{
    public T this[int index]
    {
        get => Children[index];
        set => Children[index] = value;
    }

    T[] Children;

    int ActualCount = 0;

    public int Count => ActualCount;

    EqualityComparer<T> Comparer;

    public NodeChildren (int OriginalSize = 8)
    {
        #if DEBUG
        if(OriginalSize < 1) throw new Exception("Tried to start NodeChildren with a size lower than 1.");
        #endif
        Children = new T[OriginalSize];

        Comparer = EqualityComparer<T>.Default;
    }

    public NodeChildren (T[] array)
    {
        #if DEBUG
        if(array.Length < 1) throw new Exception("Tried to start NodeChildren with a array with no elements.");
        #endif
        Children = array;

        ActualCount = array.Length;

        Comparer = EqualityComparer<T>.Default;
    }

    public NodeChildren (T[] array, int count)
    {
        #if DEBUG
        if(array.Length < 1) throw new Exception("Tried to start NodeChildren with a array with no elements.");
        #endif
        Children = array;

        ActualCount = count;

        Comparer = EqualityComparer<T>.Default;
    }

    public void Add(T value)
    {
        Children[ActualCount] = value;
        ++ActualCount;

        if(ActualCount >= Children.Length - 1) Array.Resize(ref Children, Children.Length + 8);
    }

    public void Remove(T value)
    {
        int currCount = ActualCount;
        for(int i1 = 0; i1 < currCount; ++i1)
        {
            if(Comparer.Equals(Children[i1], value))
            {
                --ActualCount;

                if(i1 == currCount - 1) Children[i1] = default(T);
                else
                {
                    for (int i2 = i1; i2 < currCount - 1; ++i2)
                    {
                        Children[i2] = Children[i2 + 1];
                    }
                }
            }
        }
    }

    public void Clear ()
    {
        for(int i = 0; i < ActualCount; ++i)
        {
            Children[i] = default(T);
        }

        ActualCount = 0;
    }
}