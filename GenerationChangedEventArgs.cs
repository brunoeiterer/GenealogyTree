using System;

namespace GenealogyTree
{
    public class GenerationChangedEventArgs : EventArgs
    {
        public string duplicatedName;
    }

    public delegate void GenerationChangedEventHandler(object sender, GenerationChangedEventArgs e);

}
