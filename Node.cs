using System;
using System.Collections.Generic;

namespace GenealogyTree
{
    [Serializable]
    public class Node<T> where T : class
    {
        public T Value { get; set; }
        public List<Node<T>> Children { get; set; }
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
            for(int i = 0; i < Children.Count; i++)
            {
                if(Children[i].Value == node.Value)
                {
                    Children.Remove(Children[i]);
                }
            }
        }

        public void Traverse(Node<T> node, Action<T, Node<T>> visitor)
        {
            visitor(node.Value, node);
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
