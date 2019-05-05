using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GenealogyTree
{
    class Menu
    {
        public StackPanel Panel { get; private set; }
        public Button AddChildButton { get; private set; }
        public Button AddPartnerButton { get; private set; }
        public Menu()
        {
            AddChildButton = new Button()
            {
                Content = "👶",
                ToolTip = "Add child",
                Visibility = Visibility.Hidden
            };
            AddChildButton.Click += AddChildButton_Click;

            AddPartnerButton = new Button()
            {
                Content = "⚭",
                ToolTip = "Add partner",
                Visibility = Visibility.Hidden
            };
            AddPartnerButton.Click += AddPartnerButton_Click;

            Binding panelWidthBinding = new Binding()
            {
                Source = Application.Current.MainWindow,
                Path = new PropertyPath(MainWindow.ActualWidthProperty)
            };

            Panel = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            Panel.SetBinding(StackPanel.WidthProperty, panelWidthBinding);
            Panel.Children.Add(AddChildButton);
            Panel.Children.Add(AddPartnerButton);
        }

        private void AddChildButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddPartnerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public void PersonGotFocus(object sender, PersonGotFocusEventArgs e)
        {
            Node<Person> person;
            person = PersonTree.GetNodeByName(PersonTree.Tree, e.personName);
            if(person.Value.Partner == string.Empty)
            {
                AddPartnerButton.Visibility = Visibility.Visible;
            }

            AddChildButton.Visibility = Visibility.Visible;
        }

        public void PersonLostFocus(object sender, PersonLostFocusEventArgs e)
        {
            AddChildButton.Visibility = Visibility.Hidden;
            AddPartnerButton.Visibility = Visibility.Hidden;
        }
    }
}
