using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Globalization;

namespace GenealogyTree
{
    public partial class AddParentsWindow : Window
    {
        public ObservableCollection<string> ChildList { get; private set; }
        public AddParentsWindow()
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
            string errorMessage = string.Empty;
            if(this.FatherName.Text == string.Empty)
            {
                errorMessage += Application.Current.Resources["AddParentsWindowEmptyFatherErrorMessage"].ToString();
            }
            if(this.MotherName.Text == string.Empty)
            {
                errorMessage += "\n" + Application.Current.Resources["AddParentsWindowEmptyMotherErrorMessage"].ToString();
            }

            if (errorMessage != string.Empty)
            {
                MessageBox.Show(errorMessage, (string)FindResource("AddParentsWindowEmptyParentsMessageBoxTitle"));
            }
            else
            {
                if (this.FatherCheckBox.IsChecked == false && this.MotherCheckBox.IsChecked == false)
                {
                    MessageBox.Show((string)FindResource("AddParentsWindowNoPrimaryParentErrorMessage"),
                        (string)FindResource("AddParentsWindowNoPrimaryParentMessageBoxTitle"));
                }
                else
                {
                    DateTime birthDate;
                    DateTime deathDate;
                    Nullable<DateTime> fatherBirthDate = null;
                    Nullable<DateTime> fatherDeathDate = null;

                    try
                    {
                        birthDate = DateTime.ParseExact(this.FatherBirthDate.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None);
                        fatherBirthDate = (Nullable<DateTime>)birthDate;
                    }
                    catch (FormatException)
                    {
                        if (this.FatherBirthDate.Text.ToString() == string.Empty)
                        {
                            fatherBirthDate = null;
                        }
                        else
                        {
                            errorMessage += (string)FindResource("AddParentsWindowInvalidFatherBirthDateError");
                        }
                    }
                    try
                    {
                        deathDate = DateTime.ParseExact(this.FatherDeathDate.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None);
                        fatherDeathDate = (Nullable<DateTime>)deathDate;
                    }
                    catch (FormatException)
                    {
                        if (this.FatherDeathDate.Text.ToString() == string.Empty)
                        {
                            fatherDeathDate = null;
                        }
                        else
                        {
                            errorMessage += (string)FindResource("AddParentsWindowInvalidFatherDeathDateError");
                        }
                    }

                    Nullable<DateTime> motherBirthDate = null;
                    Nullable<DateTime> motherDeathDate = null;

                    try
                    {
                        birthDate = DateTime.ParseExact(this.MotherBirthDate.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None);
                        motherBirthDate = (Nullable<DateTime>)birthDate;
                    }
                    catch (FormatException)
                    {
                        if (this.MotherBirthDate.Text.ToString() == string.Empty)
                        {
                            motherBirthDate = null;
                        }
                        else
                        {
                            errorMessage += (string)FindResource("AddParentsWindowInvalidMotherBirthDateError");
                        }
                    }
                    try
                    {
                        deathDate = DateTime.ParseExact(this.MotherDeathDate.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None);
                        motherDeathDate = (Nullable<DateTime>)deathDate;
                    }
                    catch (FormatException)
                    {
                        if (this.MotherDeathDate.Text.ToString() == string.Empty)
                        {
                            motherDeathDate = null;
                        }
                        else
                        {
                            errorMessage += (string)FindResource("AddParentsWindowInvalidMotherDeathDateError");
                        }
                    }

                    if (errorMessage == string.Empty)
                    {
                        Person parent = new Person();
                        if (FatherCheckBox.IsChecked == true)
                        {
                            parent.Name = this.FatherName.Text;
                            parent.BirthDate = fatherBirthDate;
                            parent.DeathDate = fatherDeathDate;
                            parent.BirthPlace = this.FatherBirthPlace.Text;
                            parent.Partner = this.MotherName.Text;
                            parent.PartnerBirthDate = motherBirthDate;
                            parent.PartnerDeathDate = motherDeathDate;
                            parent.PartnerBirthPlace = this.MotherBirthPlace.Text;
                        }
                        else
                        {
                            parent.Name = this.MotherName.Text;
                            parent.BirthDate = motherBirthDate;
                            parent.DeathDate = motherDeathDate;
                            parent.BirthPlace = this.MotherBirthPlace.Text;
                            parent.Partner = this.FatherName.Text;
                            parent.PartnerBirthDate = fatherBirthDate;
                            parent.PartnerDeathDate = fatherDeathDate;
                            parent.PartnerBirthPlace = this.FatherBirthPlace.Text;
                        }

                        Node<Person> parentNode = new Node<Person>(parent, null);
                        parentNode.SubscribeToNewChildAdded(PersonTree.NewChildAdded);
                        Node<Person> child = PersonTree.GetNodeByName(PersonTree.Tree, this.ChildCombobox.SelectedItem.ToString());

                        child.Parent = parentNode;
                        parentNode.Children.Add(child);

                        PersonTree.Tree = parentNode;

                        ParentsAddedEventArgs eventArgs = new ParentsAddedEventArgs()
                        {
                            parent = parentNode
                        };

                        ParentsAdded?.Invoke(this, eventArgs);
                        this.Close();

                    }
                    else
                    {

                    }
                }
            }

            /*
            else
            {
                Node<Person> child = PersonTree.GetNodeByName(PersonTree.Tree , this.ChildCombobox.SelectedValue.ToString());
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
            */
        }

        private void AddToChildList(Person person, Node<Person> node)
        {
            if(node.Parent == null && node.Value.Name != string.Empty)
            {
                ChildList.Add(person.Name);
            }
        }

        public event ParentsAddedEventHandler ParentsAdded;

        private void PrimaryFamilyMember_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if((string)checkBox.Content == Application.Current.Resources["AddParentsWindowFatherCheckBox"].ToString())
            {
                MotherCheckBox.IsChecked = false;
            }
            else if((string)checkBox.Content == Application.Current.Resources["AddParentsWindowMotherCheckBox"].ToString())
            {
                FatherCheckBox.IsChecked = false;
            }
        }
    }
}
