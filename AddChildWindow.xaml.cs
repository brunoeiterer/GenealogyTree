using System;
using System.Linq;
using System.Windows;
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
                MessageBox.Show(Application.Current.Resources["AddChildWindowEmptyNewNameErrorMessage"].ToString(),
                    Application.Current.Resources["AddChildWindowEmptyNewNameErroMessageBoxTitle"].ToString());
            }
            else
            {
                Node<Person> parent;
                parent = PersonTree.GetNodeByName(PersonTree.Tree, this.ParentCombobox.SelectedItem.ToString());

                if(parent.Value.Partner != string.Empty)
                {
                    Person newPerson = new Person(this.NewPersonName.Text);
                    string errorMessage = string.Empty;

                    DateTime birthDate;
                    DateTime deathDate;

                    try
                    {
                        birthDate = DateTime.ParseExact(this.NewPersonBirthDate.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None);
                        newPerson.BirthDate = (Nullable<DateTime>)birthDate;
                    }
                    catch (FormatException)
                    {
                        if (this.NewPersonBirthDate.Text.ToString() == string.Empty)
                        {
                            newPerson.BirthDate = null;
                        }
                        else
                        {
                            errorMessage += (string)FindResource("AddChildWindowBirthDateFormatError");
                        }
                    }

                    try
                    {
                        deathDate = DateTime.ParseExact(this.NewPersonDeathDate.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None);
                        newPerson.DeathDate = (Nullable<DateTime>)deathDate;
                    }
                    catch (FormatException)
                    {
                        if (this.NewPersonDeathDate.Text.ToString() == string.Empty)
                        {
                            newPerson.BirthDate = null;
                        }
                        else
                        {
                            errorMessage += "\n" + (string)FindResource("AddChildWindowDeathDateFormatError");
                        }
                    }

                    if (this.NewPersonPartnerName.Text.ToString() != string.Empty)
                    {
                        newPerson.Partner = this.NewPersonPartnerName.Text;

                        DateTime partnerBirthDate;
                        DateTime partnerDeathDate;

                        try
                        {
                            partnerBirthDate = DateTime.ParseExact(this.NewPersonPartnerBirthDate.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None);
                            newPerson.PartnerBirthDate = (Nullable<DateTime>)partnerBirthDate;
                        }
                        catch (FormatException)
                        {
                            if (this.NewPersonPartnerBirthDate.Text.ToString() == string.Empty)
                            {
                                newPerson.PartnerBirthDate = null;
                            }
                            else
                            {
                                errorMessage += "\n" + (string)FindResource("AddChildWindowPartnerBirthDateFormatError");
                            }
                        }
                        try
                        {
                            partnerDeathDate = DateTime.ParseExact(this.NewPersonPartnerDeathDate.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None);
                            newPerson.PartnerDeathDate = (Nullable<DateTime>)partnerDeathDate;
                        }
                        catch (FormatException)
                        {
                            if (this.NewPersonPartnerDeathDate.Text.ToString() == string.Empty)
                            {
                                newPerson.PartnerDeathDate = null;
                            }
                            else
                            {
                                errorMessage += "\n" + (string)FindResource("AddChildWindowPartnerDeathDateFormatError");
                            }
                        }
                    }

                    if (errorMessage == string.Empty)
                    {
                        parent.AddChild(newPerson, parent);
                        parent.Children[parent.Children.Count - 1].SubscribeToNewChildAdded(PersonTree.NewChildAdded);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(errorMessage, (string)FindResource("AddChildWindowDateFormatErrorMessageBoxTitle"));
                    }
                }
                else
                {
                    MessageBox.Show((string)FindResource("AddChildWindowEmptyParentPartnerError"),
                        (string)FindResource("AddChildWindowEmptyParentPartnerErrorMessageBoxTitle"));
                }
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
