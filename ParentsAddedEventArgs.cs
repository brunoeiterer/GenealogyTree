using System;

namespace GenealogyTree
{
    public class ParentsAddedEventArgs : EventArgs
    {
        public Node<Person> parent;
    }

    public delegate void ParentsAddedEventHandler(object sender, ParentsAddedEventArgs e);

}
