using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    internal class MyHashtableOfT<TKey, TValue>
    {
        private struct KeyValuePair : IComparable<KeyValuePair>
        {
            public TKey Key;
            public TValue Value;

            public int CompareTo(MyHashtableOfT<TKey, TValue>.KeyValuePair other) 
            {
                return Comparer<KeyValuePair>. Default.Compare(this, other);
            }
        }

        private MyDynamicArray<KeyValuePair>[] buckets;
        private int _capacity;

        public MyHashtableOfT(int capacity) 
        {
            _capacity= capacity;
            buckets = new MyDynamicArray<KeyValuePair>[capacity];
            for (int i = 0; i < capacity; i++)
            {
                buckets[i] = new MyDynamicArray<KeyValuePair>();
            }
        }

        public void Add(TKey key, TValue value) 
        {
            int index = Hash(key);
            for (int i = 0; i < buckets[index].Count; i++)
            {
                if (buckets[index][i].CompareTo(new KeyValuePair(key, value)))
            }
        }

        public int Hash(TKey key) 
        {
            string keyName = key.ToString();
            int hashValue = 0;
            for (int i = 0; i < keyName.Length; i++)
            {
                hashValue += keyName[i];
            }
            return hashValue %= _capacity;
        }
    }
}
