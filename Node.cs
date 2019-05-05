using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenealogyTree
{
    class Node<T>
    {
        public T Value { get; set; }
        public List<Node<T>> Children { get; private set; }
        private Node<T> Parent { get; set; }

        public Node(T value, Node<T> parent)
        {
            Value = value;
            Children = new List<Node<T>>();
            Parent = parent;
        }

        public void AddChild(T value, Node<T> parent)
        {
            Children.Add(new Node<T>(value, parent));
        }

        public void InsertChild(Node<T> node, int index)
        {
            Children.Insert(index, node);
        }

        public void Remove(Node<T> node)
        {
            Children.Remove(node);
        }
    }
}
