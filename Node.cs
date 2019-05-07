using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenealogyTree
{
    public class Node<T>
    {
        public T Value { get; set; }
        public List<Node<T>> Children { get; private set; }
        public Node<T> Parent { get; set; }

        public Node(T value, Node<T> parent)
        {
            Value = value;
            Children = new List<Node<T>>();
            Parent = parent;
        }

        public void AddChild(T value, Node<T> parent)
        {
            Children.Add(new Node<T>(value, parent));
            NewChildAddedEventArgs<T> eventArgs = new NewChildAddedEventArgs<T>()
            {
                child = value
            };
            NewChildAdded?.Invoke(this, eventArgs);
        }

        public event NewChildAddedEventHandler<T> NewChildAdded;

        public void InsertChild(Node<T> node, int index)
        {
            Children.Insert(index, node);
        }

        public void Remove(Node<T> node)
        {
            Children.Remove(node);
        }

        public void Traverse(Node<T> node, Action<T> visitor)
        {
            visitor(node.Value);
            foreach(Node<T> child in node.Children)
            {
                Traverse(child, visitor);
            }
        }

        public void SubscribeToNewChildAdded(NewChildAddedEventHandler<T> handler)
        {
            NewChildAdded += handler;
        }
    }
}
