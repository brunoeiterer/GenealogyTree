﻿using System;
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

            Action<Person> AddToChildListDelegate;
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

                    PartnerAddedEventArgs eventArgs = new PartnerAddedEventArgs();
                    eventArgs.childName = child.Value.Name;
                    eventArgs.partnerName = child.Value.Partner;
                    eventArgs.birthDate = child.Value.PartnerBirthDate;
                    eventArgs.deathDate = child.Value.PartnerDeathDate;

                    PartnerAdded?.Invoke(this, eventArgs);
                    this.Close();
                }
                else
                {
                    MessageBox.Show(errorMessage, (string)FindResource("DateFormatErrorMessageBoxTitle"));
                }

            }
        }

        private void AddToChildList(Person person)
        {
            if(person.Partner == string.Empty)
            {
                ChildList.Add(person.Name);
            }
        }

        public event PartnerAddedEventHandler PartnerAdded;
    }
}
