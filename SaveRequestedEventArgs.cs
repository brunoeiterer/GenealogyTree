namespace GenealogyTree
{
    public class SaveRequestedEventArgs
    {
        public string filename;
    }

    public delegate void SaveRequestedEventHandler(object sender, SaveRequestedEventArgs e);
}
