using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Globalization;

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
            string child = this.ChildCombobox.SelectedItem.ToString();
            Node<Person> childNode = PersonTree.GetNodeByName(PersonTree.Tree, child);

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
            if(person.Name != string.Empty)
            {
                childList.Add(person.Name);
            }

            if(person.Partner != string.Empty)
            {
                childList.Add(person.Partner);
            }
        }
    }
}
