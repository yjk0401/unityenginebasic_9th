﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortAlgorithms
{

    internal static class ArraySorts
    {
        /// <summary>
        /// 거품 정렬
        /// O(N^2)
        /// Stable.
        /// </summary>
        /// <param name="arr"></param>
        public static void BubbleSort(int[] arr) 
        {
            int i, j;
            for (i = 0; i < arr.Length - 1; i++)
            {
                for (j = 0; j < arr.Length - 1 - i ; j++)
                {
                    if (arr[j] > arr[j + 1]) 
                    {
                        Swap(ref arr[j], ref arr[j + 1]);
                    }
                }
            }
        }

        /// <summary>
        /// 선택 정렬
        /// O(N^2)
        /// Unstable
        /// </summary>
        /// <param name="arr"></param>
        public static void SelectionSort(int[] arr) 
        {
            int i, j, minIdx;
            for (i = 0; i < arr.Length; i++)
            {
                minIdx = i;
                for (j = i + 1; j < arr.Length; j++)
                {
                    if (arr[j] < arr[minIdx])
                        minIdx = j;
                }

                Swap(ref arr[i], ref arr[minIdx]);
            }
        }

        /// <summary>
        /// 삽입정렬
        /// O(N^2)
        /// stable
        /// </summary>
        /// <param name="arr"></param>
        public static void InsertionSort(int[] arr) 
        {
            int i, j;
            int key;

            for (i = 1; i < arr.Length; i++)
            {
                key = arr[i];
                j= i - 1;
                while (j >= 0 && arr[j] > key)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }
                arr[j + 1] = key;
            }
        }

        public static void MergeSort(int[] arr) 
        {
            int lenght = arr.Length;

            for (int mergeSize = 1; mergeSize < lenght; mergeSize *= 2)
            {
                for (int start = 0; start < lenght; start += 2 * mergeSize)
                {
                    int mid = Math.Min(start + mergeSize - 1, lenght - 1);
                    int end = Math.Min(start + mergeSize * 2 - 1, lenght -1);

                    Merge(arr, start, mid, end);
                }
            }
        }

        private static void Merge(int[] arr, int start, int mid, int end) 
        {
            int part1 = start;
            int part2 = mid + 1;
            int length1 = mid - start + 1;
            int length2 = end - mid;

            int[] copy1 = new int[length1];
            int[] copy2 = new int[length2];

            int i = 0;
            int j = 0;
            for (i = 0; i < length1; i++)
                copy1[i] = arr[start + i];

            for (j = 0; j < length2; j++)
                copy2[j] = arr[mid + 1 + j];

            int index = start;
            i = 0;
            j = 0;

            while (i < length1 && j < length2) 
            {
                if (copy1[i] <= copy2[j])
                    arr[index++] = copy1[i++];
                else
                    arr[index++] = copy2[j++];
            }

            while (i < length1)
                arr[index++] = copy1[i++];
        }

        public static void RecursiveMergeSort(int[] arr)
        {
            RecursiveMergeSort(arr, 0, arr.Length -1);
        }

        private static void RecursiveMergeSort(int[] arr, int start, int end) 
        {
            if (start < end)
            {
                int mid = end + (start - end) / 2 - 1;
                RecursiveMergeSort(arr, start, mid);
                RecursiveMergeSort(arr, mid + 1, end);

                Merge(arr, start, mid, end);
            }
        }


        public static void QuickSort(int[] arr) 
        {
            Stack<int> partionStack = new Stack<int>();
            partionStack.Push(0);
            partionStack.Push(arr.Length - 1);

            while (partionStack.Count > 0)
            {
                int end = partionStack.Pop();
                int start = partionStack.Pop();
                int partition = Partition(arr, start, end);

                // left side
                if (partition - 1 > start) 
                {
                    partionStack.Push(start);
                    partionStack.Push(partition - 1);
                }

                // right side
                if (partition + 1 < end)
                {
                    partionStack.Push(partition + 1);
                    partionStack.Push(end);
                }
            }
        }

        public static void RecursiveQuickSort(int[] arr) 
        {
            RecursiveQuickSort(arr, 0, arr.Length - 1);
        }

        public static void RecursiveQuickSort(int[] arr, int start, int end) 
        {
            if (start < end) 
            {
                int partition = Partition(arr, start, end);
                RecursiveQuickSort(arr, start, partition - 1);
                RecursiveQuickSort(arr, partition + 1, end);
            }
        }

        private static int Partition(int[] arr, int start, int end)
        {
            int pivot = arr[end + (start - end) / 2];

            while (true)
            {
                while (arr[start] < pivot) start++;
                while (arr[end] > pivot) end--;

                if (start < end)
                {
                    if (arr[start] == pivot && arr[start] == arr[end])
                        end--;
                    else
                        Swap(ref arr[start], ref arr[end]);
                }
                else
                {
                    return end;
                }


            }
        }

        public static void HeapSort(int[] arr) 
        {
            //HeapifyTopDown(arr);
            HeapifyBottomUp(arr);

            InverseHeapify(arr);
        }

        public static void HeapifyTopDown(int[] arr) 
        {
            int end = 1;
            while (end < arr.Length)
            {
                SIFT_Up(arr, 0, end++);
            }
        }

        public static void HeapifyBottomUp(int[] arr) 
        {
            int end = arr.Length - 1;
            int current = end;

            while (current >= 0)
            {
                SIFT_Down(arr, end, current--);
            }
        }

        public static void InverseHeapify(int[] arr)
        {
            int end = arr.Length - 1;
            while (end > 0)
            {
                Swap(ref arr[0], ref arr[end]);
                end--;
                SIFT_Down(arr, end, 1);
            }
        }

        public static void SIFT_Up(int[] arr, int root, int current) 
        {
            int parrent = (current - 1) / 2;
            while (current > root)
            {
                if (arr[current] > arr[parrent]) 
                {
                    Swap(ref arr[current], ref arr[parrent]);
                    current = parrent;
                    parrent = (current - 1) / 2;
                }
                else
                {
                    return;
                }
            }
        }

        public static void SIFT_Down(int[] arr, int end, int currennt)
        {
            int parrent = (currennt - 1) / 2;
            while (currennt <= end) 
            {
                if (currennt + 1 <= end && 
                    arr[currennt] < arr[currennt + 1])
                {
                    currennt = currennt + 1;
                }
                if (arr[currennt] > arr[parrent])
                {
                    Swap(ref arr[currennt], ref arr[parrent]);
                    parrent = currennt;
                    currennt = parrent * 2 + 1;
                }
                else 
                {
                    return;
                }
            }
        }

        // ref : 인자를 변수의 참조로 받아야 할 때 사용하는 키워드
        public static void Swap(ref int a, ref int b) 
        {
            int tmp = b;
            b = a;
            a = tmp;
        }

    }
}
