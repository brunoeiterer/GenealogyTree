using System;

namespace GenealogyTree
{
    public class OpenRequestedEventArgs : EventArgs
    {
        public string filename;
    }

    public delegate void OpenRequestedEventHandler(object sender, OpenRequestedEventArgs e);
}
