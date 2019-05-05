using System;

namespace GenealogyTree
{
    public class PersonLostFocusEventArgs : EventArgs
    {

    }

    public delegate void PersonLostFocusEventHandler(object sender, PersonLostFocusEventArgs e);
}
