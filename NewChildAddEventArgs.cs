using System;

namespace GenealogyTree
{
    public class NewChildAddedEventArgs<T> : EventArgs
    {
       public T child;
    }

    public delegate void NewChildAddedEventHandler<T>(object sender, NewChildAddedEventArgs<T> e);
}
