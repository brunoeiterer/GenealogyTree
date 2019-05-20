using System;

namespace GenealogyTree
{
    public class GenerationChangedEventArgs : EventArgs
    {

    }

    public delegate void GenerationChangedEventHandler(object sender, GenerationChangedEventArgs e);

}
