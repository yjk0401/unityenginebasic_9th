using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    internal class MyDynamicArray<T> : IEnumerable<T>
        where T : IComparable<T>
    {
        private static int DEFAULT_SIZE = 1;
        public int Count => _Count;
        public int Capacity => _items.Length;
        public T this[int index] 
        {
            get 
            {
                if ((uint)index >= _Count) 
                {
                    throw new IndexOutOfRangeException();
                }
                return _items[index];
            }
            set
            {
                if ((uint)index >= _Count)
                {
                    throw new IndexOutOfRangeException();
                }
                _items[index] = value;
            }
        }
        private T[] _items = new T[DEFAULT_SIZE];
        private int _Count;

        /// <summary>
        ///  삽입 알고리즘
        ///  일반적인 경우 0(1), 공간이 모자란 경우에는 0(N)
        /// </summary>
        /// <param name="item"></param>

        public void Add(T item) 
        {
            // 공간이 모자라다면 더 큰배열을 만들고 아이템 복제
            if (_Count == _items.Length) 
            {
                T[]tmpItems = new T[_Count * 2];
                Array.Copy(_items, 0, tmpItems, 0, _Count);
                _items = tmpItems;
            }

            _items[_Count++] = item;
        }

        public T Find(Predicate<T> match) 
        {
            for (int i = 0; i < _Count; i++)
            {
                if (match.Invoke(_items[i]))
                    return _items[i];
            }
            return default(T);
        }

        public MyDynamicArray<T> FindAll(Predicate<T> match)
        {
            MyDynamicArray<T> result = new MyDynamicArray<T>();
            for (int i = 0; i < _Count; i++)
            {
                if (match.Invoke(_items[i]))
                    result.Add(_items[i]);
            }
            return result;
        }
        /// <summary>
        /// 탑색 알고리즘
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item) 
        {
            for (int i = 0; i < _Count; i++)
            {
                if (Comparer<T>.Default.Compare(_items[i], item) == 0)
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// 삭제 알고리즘
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0)
                return false;

            for (int i = index; i < _Count - 1; i++)
            {
                _items[i] = _items[i + 1];
            }
            _Count--;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
           for (int i = 0; i < _Count; i++)
           {
               yield return _items[i];
           }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<T>
        {
            public T Current => _data._items[_index];

            object IEnumerator.Current => _data._items[_index];

            private MyDynamicArray<T> _data;
            private int _index;

            public Enumerator(MyDynamicArray<T> data) 
            {
                _data = data;
                _index = -1;
            }

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                _index++;
                return _index < _data._Count;
            }

            public void Reset()
            {
                _index = -1;
            }
        }
    }
}
