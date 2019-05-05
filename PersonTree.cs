using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenealogyTree
{
    class PersonTree
    {
        public static Node<Person> Tree {get; private set;}

        public PersonTree()
        {
            Tree = new Node<Person>(new Person(string.Empty, string.Empty), null);
        }

        public static Node<Person> GetNodeByName(Node<Person> node, string name)
        {
            if(node.Value.Name == name || node.Value.Partner == name)
            {
                return node;
            }
            else
            {
                foreach(Node<Person> child in node.Children)
                {
                    GetNodeByName(child, name);
                }
            }
            return null;
        }
    }
}
