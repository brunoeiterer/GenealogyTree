using System;

namespace GenealogyTree
{
    public class NewGenerationAddedEventArgs : EventArgs
    {
        public Generation generation;
    }

    public delegate void NewGenerationAddedEventHandler(object sender, NewGenerationAddedEventArgs e);
}
