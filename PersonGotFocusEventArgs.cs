using System;

namespace GenealogyTree
{
    public class PersonGotFocusEventArgs : EventArgs
    {
        public string personName;
    }

    public delegate void PersonGotFocusEventHandler(object sender, PersonGotFocusEventArgs e);
}
