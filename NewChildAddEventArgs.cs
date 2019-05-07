using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenealogyTree
{
    public class NewChildAddedEventArgs<T> : EventArgs
    {
       public T child;
    }

    public delegate void NewChildAddedEventHandler<T>(object sender, NewChildAddedEventArgs<T> e);
}
