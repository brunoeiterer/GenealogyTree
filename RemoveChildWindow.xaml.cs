using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace GenealogyTree
{
    public partial class RemoveChildWindow : Window
    {
        ObservableCollection<string> childList;
        public RemoveChildWindow()
        {
            InitializeComponent();

            childList = new ObservableCollection<string>();

            Action<Person, Node<Person>> AddToParentListDelegate;
            AddToParentListDelegate = AddToChildList;
            PersonTree.Tree.Traverse(PersonTree.Tree, AddToParentListDelegate);

            this.ChildCombobox.ItemsSource = childList;
            if(childList.Count > 0)
            {
                this.ChildCombobox.SelectedItem = childList.First<string>();
            }
        }

        private void RemoveChildButton_Click(object sender, RoutedEventArgs e)
        {
            string child = string.Empty;
            string partner = string.Empty;

            if (Regex.Split(this.ChildCombobox.SelectedItem.ToString(), " & ").Length == 1)
            {
                child = this.ChildCombobox.SelectedItem.ToString();
            }
            else if(Regex.Split(this.ChildCombobox.SelectedItem.ToString(), " & ").Length == 2)
            {
                child = Regex.Split(this.ChildCombobox.SelectedItem.ToString(), " & ")[0];
                partner = Regex.Split(this.ChildCombobox.SelectedItem.ToString(), " & ")[1];
            }
            Node<Person> childNode = PersonTree.GetNodeByName(PersonTree.Tree, child, partner);

            ChildRemovedEventArgs eventArgs = new ChildRemovedEventArgs()
            {
                person = childNode,
                name = child
            };

            ChildRemoved?.Invoke(this, eventArgs);

            this.Close();
        }

        public event ChildRemovedEventHandler ChildRemoved;

        private void AddToChildList(Person person, Node<Person> node)
        {
            if(person.Name != string.Empty && person.Partner != string.Empty)
            {
                childList.Add(person.Name + " & " + person.Partner);
            }
            else if(person.Name != string.Empty)
            {
                childList.Add(person.Name);
            }
        }
    }
}
