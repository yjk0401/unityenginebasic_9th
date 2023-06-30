using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    internal class MyDynamicArray<T> : IEnumerable<T>
    {
        private static int DEFAULT_SIZE = 1;
        public int Length => _Length;
        public int Capacity => _items.Length;
        private object[] _items = new object[DEFAULT_SIZE];
        private int _Length;

        /// <summary>
        ///  삽입 알고리즘
        /// </summary>
        /// <param name="item"></param>

        public void Add(object item) 
        {
            // 공간이 모자라다면 더 큰배열을 만들고 아이템 복제
            if (_Length == _items.Length) 
            {
                object[]tmpItems = new object[_Length * 2];
                Array.Copy(_items, 0, tmpItems, 0, _Length);
                _items = tmpItems;
            }

            _items[_Length++] = item;
        }
        /// <summary>
        /// 탑색 알고리즘
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(object item) 
        {
            Comparer comparer = Comparer.Default;
            // Comparer.Default : 해당 타입의 default 비교 연산자를 가지고 비교해서 결과를 반환하는 기능을 가진 객체
            for (int i = 0; i < _Length; i++)
            {
                if (comparer.Compare(_items[i], item) == 0)
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// 삭제 알고리즘
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(object item)
        {
            int index = IndexOf(item);
            if (index < 0)
                return false;

            for (int i = index; i < _Length - 1; i++)
            {
                _items[i] = _items[i + 1];
            }
            _Length--;
            return true;
        }

        public IEnumerator GetEnumerator()
        {
           for (int i = 0; i < _Length; i++)
           {
               yield return _items[i];
           }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
