using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;

namespace GenealogyTree
{
    public class Generation
    {
        public Grid BaseGrid { get; set; }
        public List<Grid> GenerationGridList { get; set; }
        private List<string> GenerationParentsList { get; set; }
        public List<Grid> ParentsGridList { get; set; }
        private List<TextBox> textboxlist;
        private List<Label> birthDateLabelList;
        private List<Label> deathDateLabelList;
        public Guid GenerationID { get; set; }

        public Generation(List<Grid> parentsGridList)
        {
            BaseGrid = new Grid()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            GenerationParentsList = new List<string>();
            GenerationGridList = new List<Grid>();

            if(parentsGridList != null)
            {
                for (int i = 0; i < parentsGridList.Count; i++)
                {
                    for(int j = 0; j < parentsGridList[i].Children.Count; j++)
                    {
                        if (parentsGridList[i].Children[j].GetType() == typeof(TextBox))
                        {
                            if (((string)parentsGridList[i].Children[j].GetValue(TextBox.NameProperty)).Substring(0, 5) == "child")
                            {
                                BaseGrid.ColumnDefinitions.Add(new ColumnDefinition());
                                BaseGrid.ColumnDefinitions[BaseGrid.ColumnDefinitions.Count - 1].Width = GridLength.Auto;
                                GenerationParentsList.Add(((string)parentsGridList[i].Children[j].GetValue(TextBox.NameProperty)).Substring(5));
                                GenerationGridList.Add(new Grid());
                                BaseGrid.Children.Add(GenerationGridList[GenerationGridList.Count - 1]);
                            }
                        }
                    }
                }
            }
            else
            {
                GenerationParentsList.Add(string.Empty);
                BaseGrid.ColumnDefinitions.Add(new ColumnDefinition());
                GenerationGridList.Add(new Grid());
                BaseGrid.Children.Add(GenerationGridList[GenerationGridList.Count - 1]);
            }

            textboxlist = new List<TextBox>();
            birthDateLabelList = new List<Label>();
            deathDateLabelList = new List<Label>();

            GenerationID = Guid.NewGuid();
            ParentsGridList = parentsGridList;

        }

        public void AddPerson(Node<Person> person)
        {
            int generationGridIndex = 0;
            bool found = false;
            for(int i = 0; i < GenerationGridList.Count && !found; i++)
            {
                if(person.Value.Name == GenerationParentsList[i])
                {
                    generationGridIndex = i;
                    found = true;
                }
            }

            for(int i = 0; i < GenerationGridList[generationGridIndex].Children.Count; i++)
            {
                if(GenerationGridList[generationGridIndex].Children[i].GetType() == typeof(Line))
                {
                    GenerationGridList[generationGridIndex].Children.Remove(GenerationGridList[generationGridIndex].Children[i]);
                    i--;
                }
            }

            AddTextBox(person.Value.Name, "child");
            GenerationGridList[generationGridIndex].ColumnDefinitions.Add(new ColumnDefinition());
            GenerationGridList[generationGridIndex].ColumnDefinitions[GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1].Width = new GridLength(250);
            GenerationGridList[generationGridIndex].Children.Add(textboxlist[textboxlist.Count - 1]);
            Grid.SetColumn(textboxlist[textboxlist.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);
            //Grid.SetRow(textboxlist[textboxlist.Count - 1], GenerationGrid.RowDefinitions.Count);

            if(GenerationGridList[generationGridIndex].RowDefinitions.Count < 3)
            {
                GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
                GenerationGridList[generationGridIndex].RowDefinitions[GenerationGridList[generationGridIndex].RowDefinitions.Count - 1].Height = new GridLength(25);
            }

            AddBirthDateLabel(person.Value.BirthDate);

            if (GenerationGridList[generationGridIndex].RowDefinitions.Count < 3)
            {
                GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
            }
            Grid.SetRow(birthDateLabelList[birthDateLabelList.Count - 1], 1);
            Grid.SetColumn(birthDateLabelList[birthDateLabelList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

            GenerationGridList[generationGridIndex].Children.Add(birthDateLabelList[birthDateLabelList.Count - 1]);

            AddDeathDateLabel(person.Value.DeathDate);

            if (GenerationGridList[generationGridIndex].RowDefinitions.Count < 3)
            {
                GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
            }
            Grid.SetRow(deathDateLabelList[deathDateLabelList.Count - 1], 3);
            Grid.SetColumn(deathDateLabelList[deathDateLabelList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

            GenerationGridList[generationGridIndex].Children.Add(deathDateLabelList[deathDateLabelList.Count - 1]);

            if (person.Value.Partner != string.Empty)
            {
                Point textboxPoint = textboxlist[textboxlist.Count - 1].TransformToAncestor(GenerationGridList[generationGridIndex]).
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

                GenerationGridList[generationGridIndex].ColumnDefinitions.Add(new ColumnDefinition());
                GenerationGridList[generationGridIndex].ColumnDefinitions[GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1].Width = new GridLength(25);
                GenerationGridList[generationGridIndex].Children.Add(horizontalLine1);
                Grid.SetColumn(horizontalLine1, GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

                AddTextBox(person.Value.Partner, "partner");
                GenerationGridList[generationGridIndex].ColumnDefinitions.Add(new ColumnDefinition());
                GenerationGridList[generationGridIndex].ColumnDefinitions[GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1].Width = GridLength.Auto;
                GenerationGridList[generationGridIndex].Children.Add(textboxlist[textboxlist.Count - 1]);
                Grid.SetColumn(textboxlist[textboxlist.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

                if (ParentsGridList != null)
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
                    //GenerationGridList[generationGridIndex].Children.Add(verticalLine1);
                    //Grid.SetRow(verticalLine1, 0);
                    //Grid.SetColumn(verticalLine1, GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 2);

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
                    //GenerationGridList[generationGridIndex].Children.Add(verticalLine2);
                    //Grid.SetRow(verticalLine2, 0);
                    //Grid.SetColumn(verticalLine2, GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 3);

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
                    //GenerationGridList[generationGridIndex].Children.Add(horizontalLine2);
                    //Grid.SetRow(horizontalLine2, 0);

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
                    //GenerationGridList[generationGridIndex].Children.Add(horizontalLine3);
                    //Grid.SetRow(horizontalLine3, 0);
                    //Grid.SetColumn(horizontalLine3, GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 2);
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
                //GenerationGridList[generationGridIndex].Children.Add(verticalLine1);

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
                //GenerationGridList[generationGridIndex].Children.Add(verticalLine2);
            }

            GenerationGridList[generationGridIndex].ColumnDefinitions.Add(new ColumnDefinition());
            GenerationGridList[generationGridIndex].ColumnDefinitions[GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1].Width = new GridLength(25);
        }

        private void AddTextBox(string value, string type)
        {
            textboxlist.Add(new TextBox());

            textboxlist[textboxlist.Count - 1].Text = value;
            textboxlist[textboxlist.Count - 1].HorizontalAlignment = HorizontalAlignment.Center;
            textboxlist[textboxlist.Count - 1].VerticalAlignment = VerticalAlignment.Center;
            textboxlist[textboxlist.Count - 1].HorizontalContentAlignment = HorizontalAlignment.Center;
            textboxlist[textboxlist.Count - 1].Width = 250;
            textboxlist[textboxlist.Count - 1].Name = type + value;
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

        private void ConnectChildrenToParents(Node<Person> person)
        {
            int parentColumnIndex;
            bool found = false;
            for(int i = 0; i < ParentsGridList.Count; i++)
            {
                for (int j = 0; j < ParentsGridList[i].Children.Count && !found; j++)
                {
                    if (ParentsGridList[i].Children[j].GetType() == typeof(TextBox))
                    {
                        if ((string)ParentsGridList[i].Children[j].GetValue(TextBox.TextProperty) == person.Parent.Value.Name)
                        {
                            parentColumnIndex = Grid.GetColumn(ParentsGridList[i].Children[j]) + 1;
                            found = true;
                        }
                    }
                }
            }


            //int middleColumn = (GenerationGrid.ColumnDefinitions.Count + 1) / 2;
        }
    }
}
