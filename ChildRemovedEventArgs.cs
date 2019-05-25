using System;

namespace GenealogyTree
{
    public class ChildRemovedEventArgs : EventArgs
    {
        public Node<Person> person;
        public string name;
    }

    public delegate void ChildRemovedEventHandler(object sender, ChildRemovedEventArgs e);
}
