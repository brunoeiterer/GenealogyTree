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
        private List<Node<T>> children;
        private Node<T> Parent { get; set; }

        public Node(T value, Node<T> parent)
        {
            Value = value;
            children = new List<Node<T>>();
            Parent = parent;
        }

        public void AddChild(T value, Node<T> parent)
        {
            children.Add(new Node<T>(value, parent));
        }

        public void InsertChild(Node<T> node, int index)
        {
            children.Insert(index, node);
        }

        public void Remove(Node<T> node)
        {
            children.Remove(node);
        }
    }
}
