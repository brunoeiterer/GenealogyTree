using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenealogyTree
{
    public class NewGenerationAddedEventArgs : EventArgs
    {
        public Generation generation;
    }

    public delegate void NewGenerationAddedEventHandler(object sender, NewGenerationAddedEventArgs e);
}
