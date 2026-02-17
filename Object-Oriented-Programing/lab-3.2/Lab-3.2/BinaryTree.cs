using System;
using System.Collections;
using System.Collections.Generic;

namespace LabOOP3
{
    public class Node<T>
    {
        public T Data { get; set; }
        public Node<T>? Left { get; set; }
        public Node<T>? Right { get; set; }

        public Node(T data)
        {
            Data = data;
        }
    }

    public class BinaryTree<T> : IEnumerable<T> where T : class, IComparable<T>
    {
        public Node<T>? Root { get; private set; }

        public void Add(T data)
        {
            var newNode = new Node<T>(data);
            if (Root == null)
            {
                Root = newNode;
                return;
            }

            var current = Root;
            while (true)
            {
                int comparison = data.CompareTo(current.Data);
                if (comparison < 0)
                {
                    if (current.Left == null) { current.Left = newNode; break; }
                    current = current.Left;
                }
                else if (comparison > 0)
                {
                    if (current.Right == null) { current.Right = newNode; break; }
                    current = current.Right;
                }
                else
                {
                    // Елемент з таким ключем вже існує, нічого не робимо
                    break;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return PreOrderTraversal(Root).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private IEnumerable<T> PreOrderTraversal(Node<T>? node)
        {
            if (node != null)
            {
                yield return node.Data; // Повертаємо поточний вузол
                foreach (var data in PreOrderTraversal(node.Left))
                {
                    yield return data; // Рекурсивно обходимо ліве піддерево
                }
                foreach (var data in PreOrderTraversal(node.Right))
                {
                    yield return data; // Рекурсивно обходимо праве піддерево
                }
            }
        }
    }
}