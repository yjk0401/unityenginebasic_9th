﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
//  using 의 사용처
// 1. namespace 를 가져다 쓸때
// 2. namespace 간 멤버 호출이 모호할때 (다른 namespace 에 동일한 이름의 타입이 정의되어있을 때)
// 3. IDisposable 객체의 Dispose 함수 호출을 보장할 때

namespace Collections
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Queue
            Queue<int> queue1 = new Queue<int>();
            queue1.Enqueue(3);
            queue1.Dequeue();

            while (queue1.Count > 0)
            {
                if (queue1.Peek() > 1)
                    Console.WriteLine(queue1.Dequeue());
            }

            Stack<float> stack1 = new Stack<float>();
            stack1.Push(3);
            stack1.Push(2);
            stack1.Push(5);
            #endregion

            #region Stack
            while (stack1.Count > 0)
            {
                if (stack1.Peek() > 1)
                    Console.WriteLine(stack1.Pop());
            }
            #endregion

            #region HashSet
            HashSet<int> visited = new HashSet<int>();
            if (visited.Add(3))
            {
                Console.WriteLine("Added 3 in hashset");
            }
            if (visited.Remove(2))
            {
                Console.WriteLine("Remove 2 in hashset");
            }
            #endregion

            #region Dynamic Array
            MyDynamicArray myDynamicArray = new MyDynamicArray();
            myDynamicArray.Add(3);
            myDynamicArray.Add(2);
            myDynamicArray.Add(5);
            myDynamicArray.Add(7);

            IEnumerator e1 = myDynamicArray.GetEnumerator();
            while (e1.MoveNext())
            {
                Console.WriteLine(e1.Current);
            }

            MyDynamicArray<int> intDA = new MyDynamicArray<int>();
            intDA.Add(3);
            intDA.Add(2);
            intDA.Add(5);
            intDA.Add(7);
            Console.WriteLine(intDA[0]);

            IEnumerator<int> intDAEnum = intDA.GetEnumerator();
            while (intDAEnum.MoveNext())
            {
                Console.WriteLine(intDAEnum.Current);
            }
            intDAEnum.Reset();
            intDAEnum.Dispose();

            // IDisposable 객체의 Dispose() 함수의 호출을 보장하는 구문.
            using (IEnumerator<int> intDAEnum2 = intDA.GetEnumerator())
            {
                while (intDAEnum.MoveNext())
                {
                    Console.WriteLine(intDAEnum.Current);
                }
                intDAEnum.Reset();
            }

            // foreach 문
            // IEnumerable 을 순회하는 구문
            foreach (int item in intDA)
            {
                Console.WriteLine(item);
            }

            ArrayList arrayList = new ArrayList();
            arrayList.Add(3);
            arrayList.Add("Carl");
            arrayList.Contains(3); // 결과는 false. 
                                   // WHY? add(3) 호출시 Boxing 으로 인해 만들어진 객체와 
                                   // Contains(3) 호출시 Boxing 으로 인해 만들어진 객체는 다르기 때문.
            
            List<int> List = new List<int>();
            List.Add(3);
            List.Remove(3);
            List.IndexOf(3);
            List.Contains(3);
            List.Find(x => x > 1);
            List.Insert(0, 2);
            List.FindAll(x => x > 1);
            Console.WriteLine(List[0]);
            #endregion

            #region Hashtable

            Hashtable ht = new Hashtable();
            ht.Add(1, "점수");

            Dictionary<string, int> dictionary1 = new Dictionary<string, int>();
            if (dictionary1.TryGetValue("점수", out int grade)) 
            {
                Console.WriteLine("철수 점수 : " + grade);
            }

            MyHashtableOfT<string, int> dictionary2 = new MyHashtableOfT<string, int>(1000);
            dictionary2.Add("철수", 40);
            if (dictionary2.TryGetValue("철수",out grade))
            {
                Console.WriteLine("철수 점수 :" + grade);
            }

            #endregion
        }
    }
}