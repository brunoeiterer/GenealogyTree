using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenealogyTree
{
    public class SaveRequestedEventArgs
    {
        public string filename;
    }

    public delegate void SaveRequestedEventHandler(object sender, SaveRequestedEventArgs e);
}
