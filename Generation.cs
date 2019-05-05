using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;

namespace GenealogyTree
{
    class Generation
    {
        public Grid GenerationGrid { get; set; }
        private List<TextBox> TextBoxList { get; set; }

        public Generation()
        {
            Binding panelWidthBinding = new Binding();
            panelWidthBinding.Source = Application.Current.MainWindow;
            panelWidthBinding.Path = new PropertyPath(Window.ActualWidthProperty);

            GenerationGrid = new Grid();
            GenerationGrid.Height = System.Windows.SystemFonts.MessageFontSize * 2;
            GenerationGrid.VerticalAlignment = VerticalAlignment.Top;
            GenerationGrid.HorizontalAlignment = HorizontalAlignment.Center;

            TextBoxList = new List<TextBox>();
        }

        public void AddPerson(Node<Person> person)
        {
            AddTextBox(person.Value.Name);
            GenerationGrid.ColumnDefinitions.Add(new ColumnDefinition());
            GenerationGrid.ColumnDefinitions[GenerationGrid.ColumnDefinitions.Count - 1].Width = GridLength.Auto;
            GenerationGrid.Children.Add(TextBoxList[TextBoxList.Count - 1]);
            Grid.SetColumn(TextBoxList[TextBoxList.Count - 1], GenerationGrid.ColumnDefinitions.Count - 1);

            if(person.Value.Partner != string.Empty)
            {
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.Visibility = Visibility.Visible;
                line.StrokeThickness = 1;
                line.X1 = 0;
                line.X2 = 25;
                line.Y1 = 0;
                line.Y2 = 0;
                line.Stretch = Stretch.Fill;
                GenerationGrid.ColumnDefinitions.Add(new ColumnDefinition());
                GenerationGrid.ColumnDefinitions[GenerationGrid.ColumnDefinitions.Count - 1].Width = GridLength.Auto;
                GenerationGrid.Children.Add(line);
                Grid.SetColumn(line, GenerationGrid.ColumnDefinitions.Count - 1);

                AddTextBox(person.Value.Partner);
                GenerationGrid.ColumnDefinitions.Add(new ColumnDefinition());
                GenerationGrid.ColumnDefinitions[GenerationGrid.ColumnDefinitions.Count - 1].Width = GridLength.Auto;
                GenerationGrid.Children.Add(TextBoxList[TextBoxList.Count - 1]);
                Grid.SetColumn(TextBoxList[TextBoxList.Count - 1], GenerationGrid.ColumnDefinitions.Count - 1);
            }
        }

        private void AddTextBox(string value)
        {
            TextBoxList.Add(new TextBox());

            TextBoxList[TextBoxList.Count - 1].Text = value;
            TextBoxList[TextBoxList.Count - 1].HorizontalAlignment = HorizontalAlignment.Center;
            TextBoxList[TextBoxList.Count - 1].VerticalAlignment = VerticalAlignment.Center;
            TextBoxList[TextBoxList.Count - 1].HorizontalContentAlignment = HorizontalAlignment.Center;
            TextBoxList[TextBoxList.Count - 1].Width = 250;
            TextBoxList[TextBoxList.Count - 1].GotFocus += GotFocus;
            TextBoxList[TextBoxList.Count - 1].LostFocus += LostFocus;
            TextBoxList[TextBoxList.Count - 1].Name = value;
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            PersonGotFocusEventArgs eventArgs = new PersonGotFocusEventArgs();
            eventArgs.personName = ((TextBox)sender).Name;
            PersonGotFocus?.Invoke(sender, eventArgs);
        }

        public event PersonGotFocusEventHandler PersonGotFocus;

        public void SubscribeToGotFocus(PersonGotFocusEventHandler handler)
        {
            PersonGotFocus += handler;
        }

        private void LostFocus(object sender, RoutedEventArgs e)
        {
            PersonLostFocusEventArgs eventArgs = new PersonLostFocusEventArgs();
            PersonLostFocus?.Invoke(sender, eventArgs);
        }

        public event PersonLostFocusEventHandler PersonLostFocus;

        public void SubscribeToLostFocus(PersonLostFocusEventHandler handler)
        {
            PersonLostFocus += handler;
        }
    }
}
