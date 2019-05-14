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
                Content = "👶"
            };
            AddChildButton.SetResourceReference(Button.ToolTipProperty, "AddChildButtonToolTip");
            AddChildButton.Click += AddChildButton_Click;
            DockPanel.SetDock(AddChildButton, Dock.Left);

            AddPartnerButton = new Button()
            {
                Content = "⚭"
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
            BasePanel.Children.Add(AddChildButton);
            BasePanel.Children.Add(AddPartnerButton);
            BasePanel.Children.Add(LanguageComboBox);
            DockPanel.SetDock(BasePanel, Dock.Top);
        }

        private void AddChildButton_Click(object sender, RoutedEventArgs e)
        {
            AddChildWindow addChildWindow = new AddChildWindow();
            addChildWindow.Show();
        }

        private void AddPartnerButton_Click(object sender, RoutedEventArgs e)
        {
            AddPartnerWindow addPartnerWindow = new AddPartnerWindow();
            addPartnerWindow.PartnerAdded += Partner_Added;
            if(addPartnerWindow.ChildList.Count > 0)
            {
                addPartnerWindow.Show();
            }
            else
            {
                MessageBox.Show((string)Application.Current.Resources["MenuEmptyChildListError"],
                    (string)Application.Current.Resources["MenuEmptyChildListErrorMessageBoxTile"]);
            }

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

        public void Partner_Added(object sender, PartnerAddedEventArgs e)
        {
            PartnerAdded?.Invoke(sender, e);
        }

        public event PartnerAddedEventHandler PartnerAdded;
    }
}
