using System;

namespace GenealogyTree
{
    [Serializable]
    class PersonTree
    {
        
        public static Node<Person> Tree {get; set;}

        static PersonTree()
        {
            Tree = new Node<Person>(new Person(string.Empty, string.Empty), null);
            Tree.NewChildAdded += NewChildAdded;
        }

        public static Node<Person> GetNodeByName(Node<Person> node, string name)
        {
            Node<Person> nodeFound = null;
            if(node.Value.Name == name || node.Value.Partner == name)
            {
                nodeFound = node;
            }
            else
            {
                foreach(Node<Person> child in node.Children)
                {
                    nodeFound = GetNodeByName(child, name);
                    if(nodeFound != null)
                    {
                        break;
                    }
                }
            }
            return nodeFound;
        }

        public static void NewChildAdded(object sender, NewChildAddedEventArgs<Person> e)
        {
            NewChildAddedEvent?.Invoke(sender, e);
        }

        public static event NewChildAddedEventHandler<Person> NewChildAddedEvent;
    }
}
