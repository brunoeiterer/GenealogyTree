﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;

namespace GenealogyTree
{
    class Menu
    {
        public DockPanel BasePanel { get; private set; }
        public Button AddChildButton { get; private set; }
        public Button AddPartnerButton { get; private set; }
        public Button AddParentsButton { get; private set; }
        public Button SaveButton { get; private set; }
        public Button SaveAsButton { get; private set; }
        private string FileName { get; set; }
        public Button OpenButton { get; private set; }
        public Button RemoveChildButton { get; private set; }
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

            AddParentsButton = new Button()
            {
                Content = "👫"
            };
            AddParentsButton.SetResourceReference(Button.ToolTipProperty, "AddParentsButtonToolTip");
            AddParentsButton.Click += AddParentsButton_Click;
            DockPanel.SetDock(AddParentsButton, Dock.Left);

            SaveButton = new Button()
            {
                Content = "💾"
            };
            SaveButton.SetResourceReference(Button.ToolTipProperty, "SaveButtonToolTip");
            SaveButton.Click += SaveButton_Click;
            DockPanel.SetDock(SaveButton, Dock.Left);

            SaveAsButton = new Button()
            {
                Content = "✐💾"
            };
            SaveAsButton.SetResourceReference(Button.ToolTipProperty, "SaveAsButtonToolTip");
            SaveAsButton.Click += SaveAsButton_Click;
            DockPanel.SetDock(SaveAsButton, Dock.Left);

            OpenButton = new Button()
            {
                Content = "📂"
            };
            OpenButton.SetResourceReference(Button.ToolTipProperty, "OpenButtonToolTip");
            OpenButton.Click += OpenButton_Click;
            DockPanel.SetDock(OpenButton, Dock.Left);

            RemoveChildButton = new Button()
            {
                Content = "🗑"
            };
            RemoveChildButton.SetResourceReference(Button.ToolTipProperty, "RemoveChildButtonToolTip");
            RemoveChildButton.Click += RemoveChildButton_Click;
            DockPanel.SetDock(RemoveChildButton, Dock.Left);

            languagesAvailable = new LanguagesAvailable();

            LanguageComboBox = new ComboBox()
            {
                ItemsSource = languagesAvailable,
                SelectedItem = languagesAvailable.First<string>()
            };
            LanguageComboBox.SelectionChanged += LanguageChanged;
            DockPanel.SetDock(LanguageComboBox, Dock.Left);

            FileName = string.Empty;


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
            BasePanel.Children.Add(RemoveChildButton);
            BasePanel.Children.Add(AddParentsButton);
            BasePanel.Children.Add(SaveButton);
            BasePanel.Children.Add(SaveAsButton);
            BasePanel.Children.Add(OpenButton);
            BasePanel.Children.Add(LanguageComboBox);
            DockPanel.SetDock(BasePanel, Dock.Top);
        }

        private void AddChildButton_Click(object sender, RoutedEventArgs e)
        {
            AddChildWindow addChildWindow = new AddChildWindow();
            addChildWindow.FirstChildAddedEvent += FirstChildAdded;
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

        private void AddParentsButton_Click(object sender, RoutedEventArgs e)
        {
            AddParentsWindow addParentsWindow = new AddParentsWindow();
            addParentsWindow.ParentsAdded += ParentsAdded;
            if (addParentsWindow.ChildList.Count > 0)
            {
                addParentsWindow.Show();
            }
            else
            {
                MessageBox.Show((string)Application.Current.Resources["MenuEmptyAddParentsChildListError"],
                    (string)Application.Current.Resources["MenuEmptyAddParentsChildListErrorMessageBoxTile"]);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if(FileName != string.Empty)
            {
                SaveRequestedEventArgs eventArgs = new SaveRequestedEventArgs()
                {
                    filename = FileName
                };
                SaveRequested?.Invoke(this, eventArgs);
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = ".bin";
                saveFileDialog.Filter = "Genealogy tree binary files (.bin)|*.bin";
                Nullable<bool> result = saveFileDialog.ShowDialog();

                if (result == true)
                {
                    FileName = saveFileDialog.FileName;
                    SaveRequestedEventArgs eventArgs = new SaveRequestedEventArgs();
                    eventArgs.filename = FileName;
                    SaveRequested?.Invoke(this, eventArgs);
                }
            }
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".bin";
            saveFileDialog.Filter = "Genealogy tree binary files (.bin)|*.bin";
            Nullable<bool> result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                FileName = saveFileDialog.FileName;
                SaveRequestedEventArgs eventArgs = new SaveRequestedEventArgs();
                eventArgs.filename = FileName;
                SaveRequested?.Invoke(this, eventArgs);
            }
        }

        public event SaveRequestedEventHandler SaveRequested;

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".bin";
            openFileDialog.Filter = "Genealogy tree binary files (.bin)|*.bin";
            Nullable<bool> result = openFileDialog.ShowDialog();

            if(result == true)
            {
                FileName = openFileDialog.FileName;
                OpenRequestedEventArgs eventArgs = new OpenRequestedEventArgs();
                eventArgs.filename = FileName;
                OpenRequested?.Invoke(this, eventArgs);
            }
        }

        public event OpenRequestedEventHandler OpenRequested;

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

        private void FirstChildAdded(object sender, NewChildAddedEventArgs<Person> e)
        {
            NewChildAddedEventArgs<Person> eventArgs = new NewChildAddedEventArgs<Person>();
            eventArgs.child = e.child;
            FirstChildAddedEvent?.Invoke(this, eventArgs);
        }

        public event NewChildAddedEventHandler<Person> FirstChildAddedEvent;

        private void RemoveChildButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveChildWindow removeChildWindow = new RemoveChildWindow();
            removeChildWindow.ChildRemoved += ChildRemoved;

            if(removeChildWindow.ChildCombobox.Items.Count > 0)
            {
                removeChildWindow.Show();
            }
            else
            {
                MessageBox.Show((string)Application.Current.Resources["MenuEmptyRemoveChildListError"],
                    (string)Application.Current.Resources["MenuEmptyRemoveChildListErrorMessageBoxTitle"]);
            }
        }

        private void ChildRemoved(object sender, ChildRemovedEventArgs e)
        {
            ChildRemovedEvent?.Invoke(sender, e);
        }

        public event ChildRemovedEventHandler ChildRemovedEvent;

        private void ParentsAdded(object sender, ParentsAddedEventArgs e)
        {
            ParentsAddedEvent?.Invoke(sender, e);
        }

        public event ParentsAddedEventHandler ParentsAddedEvent;
    }
}
