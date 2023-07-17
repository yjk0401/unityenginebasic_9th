using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    internal class AVL_Tree<T>
        where T : IComparable<T>
    {
        private class Node 
        {
            public T Value;
            public Node Left;
            public Node Right;
            public int Height;
        }

        private Node _root;

        public void Add(T value) 
        {
            _root = Add(_root, value);
        }

        private Node Add(Node node, T value) 
        {
            if (_root == null) 
            {
                return new Node { Value = value, Height = 1 };
            }

            int compare =  value.CompareTo(node.Value);
            if (compare < 0)
            {
                node.Left =  Add(node.Left, value);
            }

            else if (compare > 0)
            {
                node.Right = Add(node.Right, value);
            }
            // ?? Null 병합연산자
            // null 이 아닌경우 왼쪽 값 변환, null 이면 오른쪽 값 변환
            node.Height = 1 + Math.Max(node?.Left.Height ?? 0, node?.Right.Height ?? 0);
            int balance = Balance(node);
            //왼쪽으로 치우처져있으면
            if (balance > 1) 
            {
            
            }
            //오른쪽으로 치우처져있으면
            else if (balance < -1)
            {

            }

            return node;
        }

        /// <summary>
        /// 기준노드 중심으로 어느쪽으로 자식노드들이 치우쳐저있는지 판단
        /// </summary>
        /// <param name="node"></param>
        /// <returns> 왼쪽 : > 1, 오른쪽 : < - 1 </returns>
        private int Balance(Node node) 
        {
            return node != null ?(node?.Left.Height ?? 0 - node?.Right.Height ?? 0) : 0;
        }


    }
}
