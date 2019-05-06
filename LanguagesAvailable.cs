using System.Collections.ObjectModel;

namespace GenealogyTree
{
    class LanguagesAvailable : ObservableCollection<string>
    {
        public LanguagesAvailable()
        {
            Add("pt-BR");
            Add("en-US");
        }
    }
}
