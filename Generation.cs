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
        private List<TextBox> textboxlist;
        private List<Label> birthDateLabelList;
        private List<Label> deathDateLabelList;

        public Generation()
        {
            Binding panelWidthBinding = new Binding();
            panelWidthBinding.Source = Application.Current.MainWindow;
            panelWidthBinding.Path = new PropertyPath(Window.ActualWidthProperty);

            GenerationGrid = new Grid()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            textboxlist = new List<TextBox>();
            birthDateLabelList = new List<Label>();
            deathDateLabelList = new List<Label>();
        }

        public void AddPerson(Node<Person> person)
        {
            AddTextBox(person.Value.Name);
            GenerationGrid.ColumnDefinitions.Add(new ColumnDefinition());
            GenerationGrid.ColumnDefinitions[GenerationGrid.ColumnDefinitions.Count - 1].Width = GridLength.Auto;
            GenerationGrid.Children.Add(textboxlist[textboxlist.Count - 1]);
            Grid.SetColumn(textboxlist[textboxlist.Count - 1], GenerationGrid.ColumnDefinitions.Count - 1);
            Grid.SetRow(textboxlist[textboxlist.Count - 1], GenerationGrid.RowDefinitions.Count);

            if (person.Value.BirthDate.HasValue)
            {
                GenerationGrid.RowDefinitions.Add(new RowDefinition());
                GenerationGrid.RowDefinitions[GenerationGrid.RowDefinitions.Count - 1].Height = new GridLength(25);

                AddBirthDateLabel(person.Value.BirthDate);
                GenerationGrid.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(birthDateLabelList[birthDateLabelList.Count - 1], GenerationGrid.RowDefinitions.Count);
                GenerationGrid.Children.Add(birthDateLabelList[birthDateLabelList.Count - 1]);
            }

            if(person.Value.DeathDate.HasValue)
            {
                GenerationGrid.RowDefinitions.Add(new RowDefinition());
                GenerationGrid.RowDefinitions[GenerationGrid.RowDefinitions.Count - 1].Height = new GridLength(23);

                AddDeathDateLabel(person.Value.DeathDate);
                GenerationGrid.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(deathDateLabelList[deathDateLabelList.Count - 1], GenerationGrid.RowDefinitions.Count);
                GenerationGrid.Children.Add(deathDateLabelList[deathDateLabelList.Count - 1]);
            }


            if (person.Value.Partner != string.Empty)
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
                GenerationGrid.Children.Add(textboxlist[textboxlist.Count - 1]);
                Grid.SetColumn(textboxlist[textboxlist.Count - 1], GenerationGrid.ColumnDefinitions.Count - 1);
            }
        }

        private void AddTextBox(string value)
        {
            textboxlist.Add(new TextBox());

            textboxlist[textboxlist.Count - 1].Text = value;
            textboxlist[textboxlist.Count - 1].HorizontalAlignment = HorizontalAlignment.Center;
            textboxlist[textboxlist.Count - 1].VerticalAlignment = VerticalAlignment.Center;
            textboxlist[textboxlist.Count - 1].HorizontalContentAlignment = HorizontalAlignment.Center;
            textboxlist[textboxlist.Count - 1].Width = 250;
            textboxlist[textboxlist.Count - 1].Name = value;
        }

        private void AddBirthDateLabel(Nullable<DateTime> date)
        {
            Label newLabel = new Label()
            {
                Content = "☆" + date.Value.ToShortDateString(),
                BorderBrush = Brushes.Transparent
            };

            birthDateLabelList.Add(newLabel);
        }

        private void AddDeathDateLabel(Nullable<DateTime> date)
        {
            Label newLabel = new Label()
            {
                Content = "✞" + date.Value.ToShortDateString(),
                BorderBrush = Brushes.Transparent
            };

            deathDateLabelList.Add(newLabel);
        }
    }
}
