using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenealogyTree
{
    public class OpenRequestedEventArgs
    {
        public string filename;
    }

    public delegate void OpenRequestedEventHandler(object sender, OpenRequestedEventArgs e);
}
