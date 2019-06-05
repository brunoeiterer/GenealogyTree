using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GenealogyTree
{
    public partial class AddPartnerWindow : Window
    {
        public ObservableCollection<string> ChildList { get; private set; }
        public AddPartnerWindow()
        {
            InitializeComponent();

            ChildList = new ObservableCollection<string>();

            Action<Person, Node<Person>> AddToChildListDelegate;
            AddToChildListDelegate = AddToChildList;
            PersonTree.Tree.Traverse(PersonTree.Tree, AddToChildListDelegate);

            this.ChildCombobox.ItemsSource = ChildList;
            if(ChildList.Count > 0)
            {
                this.ChildCombobox.SelectedItem = ChildList.First<string>();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.PartnerName.Text == string.Empty)
            {
                MessageBox.Show(Application.Current.Resources["AddPartnerWindowEmptyPartnerErrorMessage"].ToString(),
                    Application.Current.Resources["AddPartnerWindowEmptyPartnerErroMessageBoxTitle"].ToString());
            }
            else
            {
                Node<Person> child = null;
                if(Regex.Split(this.ChildCombobox.SelectedValue.ToString(), " & ").Length == 1)
                {
                    child = PersonTree.GetNodeByName(PersonTree.Tree, this.ChildCombobox.SelectedValue.ToString(), string.Empty);
                }
                else if(Regex.Split(this.ChildCombobox.SelectedValue.ToString(), " & ").Length == 2)
                {
                    child = PersonTree.GetNodeByName(PersonTree.Tree, Regex.Split(this.ChildCombobox.SelectedValue.ToString(), " & ")[0],
                        Regex.Split(this.ChildCombobox.SelectedValue.ToString(), " & ")[1]);
                }
                
                string errorMessage = string.Empty;

                DateTime birthDate;
                DateTime deathDate;
                Nullable<DateTime> partnerBirthDate = null;
                Nullable<DateTime> partnerDeathDate = null;

                try
                {
                    birthDate = DateTime.ParseExact(this.PartnerBirthDate.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None);
                    partnerBirthDate = (Nullable<DateTime>) birthDate;
                }
                catch(FormatException)
                {
                    if(this.PartnerBirthDate.Text.ToString() == string.Empty)
                    {
                        partnerBirthDate = null;
                    }
                    else
                    {
                        errorMessage += (string)FindResource("AddPartnerWindowPartnerBirthDateFormatError");
                    }
                }

                try
                {
                    deathDate = DateTime.ParseExact(this.PartnerDeathDate.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None);
                    partnerDeathDate = (Nullable<DateTime>)deathDate;
                }
                catch (FormatException)
                {
                    if (this.PartnerDeathDate.Text.ToString() == string.Empty)
                    {
                        partnerDeathDate = null;
                    }
                    else
                    {
                        errorMessage += "\n" + (string)FindResource("AddPartnerWindowPartnerDeathDateFormatError");
                    }
                }

                if (errorMessage == string.Empty)
                {
                    child.Value.Partner = this.PartnerName.Text;
                    child.Value.PartnerBirthDate = partnerBirthDate;
                    child.Value.PartnerDeathDate = partnerDeathDate;
                    child.Value.PartnerBirthPlace = this.PartnerBirthPlace.Text;

                    PartnerAddedEventArgs eventArgs = new PartnerAddedEventArgs();
                    eventArgs.childName = child.Value.Name;
                    eventArgs.partnerName = child.Value.Partner;
                    eventArgs.birthDate = child.Value.PartnerBirthDate;
                    eventArgs.deathDate = child.Value.PartnerDeathDate;
                    eventArgs.birthPlace = child.Value.PartnerBirthPlace;

                    PartnerAdded?.Invoke(this, eventArgs);
                    this.Close();
                }
                else
                {
                    MessageBox.Show(errorMessage, (string)FindResource("DateFormatErrorMessageBoxTitle"));
                }

            }
        }

        private void AddToChildList(Person person, Node<Person> node)
        {
            if(person.Partner == string.Empty)
            {
                ChildList.Add(person.Name);
            }
        }

        public event PartnerAddedEventHandler PartnerAdded;
    }
}
