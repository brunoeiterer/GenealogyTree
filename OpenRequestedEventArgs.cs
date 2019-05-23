namespace GenealogyTree
{
    public class OpenRequestedEventArgs
    {
        public string filename;
    }

    public delegate void OpenRequestedEventHandler(object sender, OpenRequestedEventArgs e);
}
