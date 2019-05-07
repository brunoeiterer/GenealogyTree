using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;

namespace GenealogyTree
{
    public class Generation
    {
        public Grid GenerationGrid { get; set; }
        private List<TextBox> textboxlist;
        private List<Label> birthDateLabelList;
        private List<Label> deathDateLabelList;
        public Guid GenerationID { get; set; }

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

            GenerationID = Guid.NewGuid();
        }

        public void AddPerson(Node<Person> person, bool firstGeneration)
        {
            AddTextBox(person.Value.Name);
            GenerationGrid.ColumnDefinitions.Add(new ColumnDefinition());
            GenerationGrid.ColumnDefinitions[GenerationGrid.ColumnDefinitions.Count - 1].Width = new GridLength(250);
            GenerationGrid.Children.Add(textboxlist[textboxlist.Count - 1]);
            Grid.SetColumn(textboxlist[textboxlist.Count - 1], GenerationGrid.ColumnDefinitions.Count - 1);
            Grid.SetRow(textboxlist[textboxlist.Count - 1], GenerationGrid.RowDefinitions.Count);

            GenerationGrid.RowDefinitions.Add(new RowDefinition());
            GenerationGrid.RowDefinitions[GenerationGrid.RowDefinitions.Count - 1].Height = new GridLength(25);

            AddBirthDateLabel(person.Value.BirthDate);
            GenerationGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(birthDateLabelList[birthDateLabelList.Count - 1], GenerationGrid.RowDefinitions.Count);
            GenerationGrid.Children.Add(birthDateLabelList[birthDateLabelList.Count - 1]);

            GenerationGrid.RowDefinitions.Add(new RowDefinition());
            GenerationGrid.RowDefinitions[GenerationGrid.RowDefinitions.Count - 1].Height = new GridLength(23);

            AddDeathDateLabel(person.Value.DeathDate);
            GenerationGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(deathDateLabelList[deathDateLabelList.Count - 1], GenerationGrid.RowDefinitions.Count);
            GenerationGrid.Children.Add(deathDateLabelList[deathDateLabelList.Count - 1]);

            if (person.Value.Partner != string.Empty)
            {
                Point textboxPoint = textboxlist[textboxlist.Count - 1].TransformToAncestor(GenerationGrid).
                    Transform(new Point(0, 0));
                Line horizontalLine1 = new Line()
                {
                    Stroke = Brushes.Black,
                    Visibility = Visibility.Visible,
                    StrokeThickness = 1,
                    X1 = 0,
                    X2 = 25,
                    Y1 = SystemFonts.MessageFontSize,
                    Y2 = SystemFonts.MessageFontSize,
                    Stretch = Stretch.None
                };

                GenerationGrid.ColumnDefinitions.Add(new ColumnDefinition());
                GenerationGrid.ColumnDefinitions[GenerationGrid.ColumnDefinitions.Count - 1].Width = new GridLength(25);
                GenerationGrid.Children.Add(horizontalLine1);
                Grid.SetColumn(horizontalLine1, GenerationGrid.ColumnDefinitions.Count - 1);

                AddTextBox(person.Value.Partner);
                GenerationGrid.ColumnDefinitions.Add(new ColumnDefinition());
                GenerationGrid.ColumnDefinitions[GenerationGrid.ColumnDefinitions.Count - 1].Width = GridLength.Auto;
                GenerationGrid.Children.Add(textboxlist[textboxlist.Count - 1]);
                Grid.SetColumn(textboxlist[textboxlist.Count - 1], GenerationGrid.ColumnDefinitions.Count - 1);


                if (!firstGeneration)
                {
                    Line verticalLine1 = new Line()
                    {
                        Stroke = Brushes.Black,
                        Visibility = Visibility.Visible,
                        StrokeThickness = 1,
                        X1 = 12.5,
                        X2 = 12.5,
                        Y1 = -87,
                        Y2 = -20,
                        Stretch = Stretch.None
                    };
                    GenerationGrid.Children.Add(verticalLine1);
                    Grid.SetRow(verticalLine1, 0);
                    Grid.SetColumn(verticalLine1, GenerationGrid.ColumnDefinitions.Count - 2);

                    Line verticalLine2 = new Line()
                    {
                        Stroke = Brushes.Black,
                        Visibility = Visibility.Visible,
                        StrokeThickness = 1,
                        X1 = 125,
                        X2 = 125,
                        Y1 = 0,
                        Y2 = -20,
                        Stretch = Stretch.None
                    };
                    GenerationGrid.Children.Add(verticalLine2);
                    Grid.SetRow(verticalLine2, 0);
                    Grid.SetColumn(verticalLine2, GenerationGrid.ColumnDefinitions.Count - 3);

                    Line horizontalLine2 = new Line()
                    {
                        Stroke = Brushes.Black,
                        Visibility = Visibility.Visible,
                        StrokeThickness = 1,
                        X1 = 125,
                        X2 = 250,
                        Y1 = -20,
                        Y2 = -20,
                        Stretch = Stretch.None
                    };
                    GenerationGrid.Children.Add(horizontalLine2);
                    Grid.SetRow(horizontalLine2, 0);

                    Line horizontalLine3 = new Line()
                    {
                        Stroke = Brushes.Black,
                        Visibility = Visibility.Visible,
                        StrokeThickness = 1,
                        X1 = 0,
                        X2 = 12.5,
                        Y1 = -20,
                        Y2 = -20,
                        Stretch = Stretch.None
                    };
                    GenerationGrid.Children.Add(horizontalLine3);
                    Grid.SetRow(horizontalLine3, 0);
                    Grid.SetColumn(horizontalLine3, GenerationGrid.ColumnDefinitions.Count - 2);
                }
            }
            else
            {
                Point textboxPoint = textboxlist[textboxlist.Count - 1].TransformToAncestor(Application.Current.MainWindow).
                    Transform(new Point(0, 0));

                Line verticalLine1 = new Line()
                {
                    Stroke = Brushes.Black,
                    Visibility = Visibility.Visible,
                    StrokeThickness = 1,
                    X1 = 125,
                    X2 = 125,
                    Y1 = -87,
                    Y2 = -20,
                    Stretch = Stretch.None
                };
                GenerationGrid.Children.Add(verticalLine1);

                Line verticalLine2 = new Line()
                {
                    Stroke = Brushes.Black,
                    Visibility = Visibility.Visible,
                    StrokeThickness = 1,
                    X1 = 125,
                    X2 = 125,
                    Y1 = 0,
                    Y2 = -20,
                    Stretch = Stretch.None
                };
                GenerationGrid.Children.Add(verticalLine2);
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
            textboxlist[textboxlist.Count - 1].Margin = new Thickness(0, 0, 0, 0);
        }

        private void AddBirthDateLabel(Nullable<DateTime> date)
        {
            Label newLabel;
            if(date != null)
            {
                newLabel = new Label()
                {
                    Content = "☆" + date.Value.ToShortDateString(),
                    BorderBrush = Brushes.Transparent
                };
            }
            else
            {
                newLabel = new Label()
                {
                    Content = "☆",
                    BorderBrush = Brushes.Transparent
                };
            }


            birthDateLabelList.Add(newLabel);
        }

        private void AddDeathDateLabel(Nullable<DateTime> date)
        {
            Label newLabel;
            if(date != null)
            {
                newLabel = new Label()
                {
                    Content = "✞" + date.Value.ToShortDateString(),
                    BorderBrush = Brushes.Transparent,
                    Margin = new Thickness(0, 0, 0, 25)
                };
            }
            else
            {
                newLabel = new Label()
                {
                    Content = "✞",
                    BorderBrush = Brushes.Transparent,
                    Margin = new Thickness(0, 0, 0, 25)
                };
            }


            deathDateLabelList.Add(newLabel);
        }
    }
}
