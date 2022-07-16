using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections;

public class GDictionary<K, V> : IDictionary<K, V>
{
    Dictionary<int, IDObjectDictG> InternalDict = new Dictionary<int, IDObjectDictG>();

    public V this[K key]
    { 
        get
        {
            int hash = key.GetHashCode();

            IDObjectDictG value = InternalDict[key.GetHashCode()];

            if(KeyComparer.Equals(key, value.Key))
            {
                return value.Value;
            }
            else
            {
                while(true)
                {
                    ++hash;

                    value = InternalDict[hash];

                    if (KeyComparer.Equals(key, value.Key)) return value.Value;
                }
            }

            throw new Exception("Value out of bounds.");
        }
        set
        {
            int hash = key.GetHashCode();

            IDObjectDictG val = InternalDict[key.GetHashCode()];

            if(KeyComparer.Equals(key, val.Key))
            {
                InternalDict[hash] = new IDObjectDictG(val.Key, value);
            }
            else
            {
                while(true)
                {
                    ++hash;

                    val = InternalDict[hash];

                    if (KeyComparer.Equals(key, val.Key)) InternalDict[hash] = new IDObjectDictG(key, value);
                }
            }
        }
    }

    public ICollection<K> Keys => throw new NotImplementedException();

    public ICollection<V> Values => throw new NotImplementedException();

    public int Count => InternalDict.Count;

    public bool IsReadOnly => false;

    private EqualityComparer<K> KeyComparer;

    public GDictionary()
    {
        KeyComparer = EqualityComparer<K>.Default;
    }

    public void Add(K key, V value)
    {
        int hash = key.GetHashCode();

        if(InternalDict.ContainsKey(hash))
        {
            #if DEBUG

            if (KeyComparer.Equals(key ,InternalDict[hash].Key))
            throw new Exception($"Not possible to add more than one value with the same key. \"{key.ToString()}\" already exists.");

            #endif
            hash+=1;

            while (InternalDict.ContainsKey(hash))
            {
                if (KeyComparer.Equals(key ,InternalDict[hash].Key))
                throw new Exception($"Not possible to add more than one value with the same key. \"{key.ToString()}\" already exists.");
                ++hash;
            }

            InternalDict.Add(hash, new IDObjectDictG(key, value));
        }
        else
        {
            InternalDict.Add(hash, new IDObjectDictG(key, value));
        }
    }

    public void Add(KeyValuePair<K, V> item)
    {
        K key = item.Key;

        V value = item.Value;

        int hash = key.GetHashCode();

        if(InternalDict.ContainsKey(hash))
        {
            hash+=1;

            while (InternalDict.ContainsKey(hash))
            {
                ++hash;
            }

            InternalDict.Add(hash, new IDObjectDictG(key, value));
        }
        else
        {
            InternalDict.Add(hash, new IDObjectDictG(key, value));
        }
    }

    public void Clear()
    {
        InternalDict.Clear();
    }

    public bool Contains(KeyValuePair<K, V> item)
    {
        K key = item.Key;

        int hash = key.GetHashCode();

        IDObjectDictG val;

        if(InternalDict.TryGetValue(hash, out val))
        {
            if (KeyComparer.Equals(key, val.Key)) return true;

            ++hash;
            while(InternalDict.TryGetValue(hash, out val))
            {
                if (KeyComparer.Equals(key, val.Key)) return true;

                ++hash;
            }
        }

        return false;
    }

    public bool ContainsKey(K key)
    {
        int hash = key.GetHashCode();

        IDObjectDictG val;

        if(InternalDict.TryGetValue(hash, out val))
        {
            if (KeyComparer.Equals(key, val.Key)) return true;

            ++hash;
            while(InternalDict.TryGetValue(hash, out val))
            {
                if (KeyComparer.Equals(key, val.Key)) return true;

                ++hash;
            }
        }

        return false;
    }

    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public bool Remove(K key)
    {
        int hash = key.GetHashCode();

        IDObjectDictG val;

        while(InternalDict.TryGetValue(hash, out val))
        {
            if(KeyComparer.Equals(key, val.Key))
            {
                InternalDict.Remove(hash);
                ++hash;

                if(!InternalDict.ContainsKey(hash)) goto end;

                int lastHash = hash-1;

                while (InternalDict.TryGetValue(hash, out val))
                {
                    if(!KeyComparer.Equals(key, val.Key)) continue;

                    InternalDict[lastHash] = InternalDict[hash];

                    lastHash = hash;

                    ++hash;
                }

                InternalDict.Remove(lastHash);

                end:

                return true;
            }

            ++hash;
        }

        return false;
    }

    public bool Remove(KeyValuePair<K, V> item)
    {
        K key = item.Key;

        int hash = key.GetHashCode();

        IDObjectDictG val;

        while(InternalDict.TryGetValue(hash, out val))
        {
            if(KeyComparer.Equals(key, val.Key))
            {
                InternalDict.Remove(hash);
                ++hash;

                if(!InternalDict.ContainsKey(hash)) goto end;

                int lastHash = hash-1;

                while (InternalDict.TryGetValue(hash, out val))
                {
                    if(!KeyComparer.Equals(key, val.Key)) continue;

                    InternalDict[lastHash] = InternalDict[hash];

                    lastHash = hash;

                    ++hash;
                }

                InternalDict.Remove(lastHash);

                end:

                return true;
            }

            ++hash;
        }

        return false;
    }

    public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value)
    {
        int hash = key.GetHashCode();

        IDObjectDictG val;

        if(InternalDict.TryGetValue(hash, out val))
        {
            if (KeyComparer.Equals(key, val.Key))
            {
                value = val.Value;
                return true;
            }

            ++hash;
            while(InternalDict.TryGetValue(hash, out val))
            {
                if (KeyComparer.Equals(key, val.Key))
                {
                    value = val.Value;
                    return true;
                }

                ++hash;
            }
        }
            
        value = default(V);
        return false;
        
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    private readonly struct IDObjectDictG
    {
        public readonly K Key;
        public readonly V Value;

        public IDObjectDictG(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }
}