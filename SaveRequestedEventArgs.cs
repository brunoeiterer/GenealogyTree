using System;

namespace GenealogyTree
{
    public class SaveRequestedEventArgs : EventArgs
    {
        public string filename;
    }

    public delegate void SaveRequestedEventHandler(object sender, SaveRequestedEventArgs e);
}
