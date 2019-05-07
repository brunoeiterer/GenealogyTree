using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Globalization;

namespace GenealogyTree
{
    public partial class AddChildWindow : Window
    {
        ObservableCollection<string> parentList;
        public AddChildWindow()
        {
            InitializeComponent();

            parentList = new ObservableCollection<string>();

            Action<Person> AddToParentListDelegate;
            AddToParentListDelegate = AddToParentList;
            PersonTree.Tree.Traverse(PersonTree.Tree, AddToParentListDelegate);


            this.ParentCombobox.ItemsSource = parentList;
            this.ParentCombobox.SelectedItem = parentList.First<string>();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.NewPersonName.Text == string.Empty)
            {
                MessageBox.Show(Application.Current.Resources["EmptyNewNameErrorMessage"].ToString(),
                    Application.Current.Resources["EmptyNewNameErroMessageBoxTitle"].ToString());
            }
            else
            {
                Node<Person> parent;
                parent = PersonTree.GetNodeByName(PersonTree.Tree, this.ParentCombobox.SelectedItem.ToString());

                Person newPerson;
                if (this.NewPersonPartnerName.Text.ToString() != string.Empty)
                {
                    newPerson = new Person(this.NewPersonName.Text.ToString(), this.NewPersonPartnerName.Text.ToString());
                }
                else
                {
                    newPerson = new Person(this.NewPersonName.Text.ToString());
                }

                DateTime birthDate;
                DateTime deathDate;

                if(DateTime.TryParseExact(this.NewPersonBirthDate.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None, out birthDate))
                {
                    newPerson.BirthDate = (Nullable<DateTime>)birthDate;
                }
                else
                {
                    newPerson.BirthDate = null;
                }

                if(DateTime.TryParseExact(this.NewPersonDeathDate.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None, out deathDate))
                {
                    newPerson.DeathDate = (Nullable<DateTime>)deathDate;
                }
                else
                {
                    newPerson.DeathDate = null;
                }

                parent.AddChild(newPerson, parent);
                parent.Children[parent.Children.Count - 1].SubscribeToNewChildAdded(PersonTree.NewChildAdded);
                this.Close();
            }
        }

        private void AddToParentList(Person person)
        {
            parentList.Add(person.Name);

            if(person.Partner != string.Empty)
            {
                parentList.Add(person.Partner);
            }
        }
    }
}
