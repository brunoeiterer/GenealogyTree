using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GenealogyTree
{
    class Menu
    {
        public DockPanel BasePanel { get; private set; }
        public Button AddChildButton { get; private set; }
        public Button AddPartnerButton { get; private set; }
        private readonly LanguagesAvailable languagesAvailable;
        private readonly ResourceDictionary languageResources;
        public ComboBox LanguageComboBox { get; private set; }
        public Menu()
        {

            languageResources = new ResourceDictionary()
            {
                Source = new Uri("StringResources.pt-BR.xaml", UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Add(languageResources);

            AddChildButton = new Button()
            {
                Content = "👶",
                Visibility = Visibility.Visible
            };
            AddChildButton.SetResourceReference(Button.ToolTipProperty, "AddChildButtonToolTip");
            AddChildButton.Click += AddChildButton_Click;
            DockPanel.SetDock(AddChildButton, Dock.Left);

            AddPartnerButton = new Button()
            {
                Content = "⚭",
                Visibility = Visibility.Visible
            };
            AddPartnerButton.SetResourceReference(Button.ToolTipProperty, "AddPartnerButtonToolTip");
            AddPartnerButton.Click += AddPartnerButton_Click;
            DockPanel.SetDock(AddPartnerButton, Dock.Left);

            languagesAvailable = new LanguagesAvailable();

            LanguageComboBox = new ComboBox()
            {
                ItemsSource = languagesAvailable,
                SelectedItem = languagesAvailable.First<string>()
            };
            LanguageComboBox.SelectionChanged += LanguageChanged;
            DockPanel.SetDock(LanguageComboBox, Dock.Left);


            Binding basePanelWidthBinding = new Binding()
            {
                Source = Application.Current.MainWindow,
                Path = new PropertyPath(MainWindow.ActualWidthProperty)
            };

            BasePanel = new DockPanel()
            {
                LastChildFill = false
            };
            BasePanel.SetBinding(StackPanel.WidthProperty, basePanelWidthBinding);
            BasePanel.Children.Add(LanguageComboBox);
        }

        private void AddChildButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddPartnerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public void PersonGotFocus(object sender, PersonGotFocusEventArgs e)
        {
            BasePanel.Children.Remove(LanguageComboBox);

            Node<Person> person;
            person = PersonTree.GetNodeByName(PersonTree.Tree, e.personName);
            if(person.Value.Partner == string.Empty)
            {
                BasePanel.Children.Add(AddPartnerButton);
            }

            BasePanel.Children.Add(AddChildButton);
            BasePanel.Children.Add(LanguageComboBox);
        }

        public void PersonLostFocus(object sender, PersonLostFocusEventArgs e)
        {
            BasePanel.Children.Remove(AddPartnerButton);
            BasePanel.Children.Remove(AddChildButton);
        }

        private void LanguageChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(e.AddedItems[0].ToString())
            {
                case "en-US":
                    languageResources.Source = new Uri("StringResources.en-US.xaml", UriKind.Relative);
                    break;
                case "pt-BR":
                    languageResources.Source = new Uri("StringResources.pt-BR.xaml", UriKind.Relative);
                    break;
                default:
                    break;
            }
        }
    }
}
