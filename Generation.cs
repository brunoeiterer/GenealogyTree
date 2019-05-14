using System;
using System.Windows;
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
        private List<TextBlock> textBlockList;
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

            if (parentsGridList != null)
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
            textBlockList = new List<TextBlock>();

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

            for (int i = 0; i < GenerationGridList[generationGridIndex].Children.Count; i++)
            {
                if (GenerationGridList[generationGridIndex].Children[i].GetType() == typeof(Line))
                {
                    GenerationGridList[generationGridIndex].Children.Remove(GenerationGridList[generationGridIndex].Children[i]);
                    i--;
                }
            }

            if(GenerationGridList[generationGridIndex].Children.Count > 0)
            {
                textBlockList.Add(new TextBlock());
                textBlockList[textBlockList.Count - 1].Width = 25;
                GenerationGridList[generationGridIndex].ColumnDefinitions.Add(new ColumnDefinition());
                GenerationGridList[generationGridIndex].ColumnDefinitions[GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1].Width = new GridLength(25);
                GenerationGridList[generationGridIndex].Children.Add(textBlockList[textBlockList.Count - 1]);
                Grid.SetColumn(textBlockList[textBlockList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);
            }

            AddTextBox(person.Value.Name, "child");
            GenerationGridList[generationGridIndex].ColumnDefinitions.Add(new ColumnDefinition());
            GenerationGridList[generationGridIndex].ColumnDefinitions[GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1].Width = new GridLength(250);
            GenerationGridList[generationGridIndex].Children.Add(textboxlist[textboxlist.Count - 1]);
            Grid.SetColumn(textboxlist[textboxlist.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

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
                GenerationGridList[generationGridIndex].ColumnDefinitions[GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1].Width = new GridLength(250);
                GenerationGridList[generationGridIndex].Children.Add(textboxlist[textboxlist.Count - 1]);
                Grid.SetColumn(textboxlist[textboxlist.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

                if (GenerationGridList[generationGridIndex].RowDefinitions.Count < 3)
                {
                    GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
                    GenerationGridList[generationGridIndex].RowDefinitions[GenerationGridList[generationGridIndex].RowDefinitions.Count - 1].Height = new GridLength(25);
                }

                AddBirthDateLabel(person.Value.PartnerBirthDate);

                if (GenerationGridList[generationGridIndex].RowDefinitions.Count < 3)
                {
                    GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
                }
                Grid.SetRow(birthDateLabelList[birthDateLabelList.Count - 1], 1);
                Grid.SetColumn(birthDateLabelList[birthDateLabelList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

                GenerationGridList[generationGridIndex].Children.Add(birthDateLabelList[birthDateLabelList.Count - 1]);

                AddDeathDateLabel(person.Value.PartnerDeathDate);

                if (GenerationGridList[generationGridIndex].RowDefinitions.Count < 3)
                {
                    GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
                }
                Grid.SetRow(deathDateLabelList[deathDateLabelList.Count - 1], 3);
                Grid.SetColumn(deathDateLabelList[deathDateLabelList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

                GenerationGridList[generationGridIndex].Children.Add(deathDateLabelList[deathDateLabelList.Count - 1]);
            }

            if (ParentsGridList != null)
            {
                ConnectChildrenToParents(person);
            }
        }

        public void AddPartner(string childName, string partnerName, Nullable<DateTime> birthDate, Nullable<DateTime> deathDate)
        {
            TextBox childTextBox = null;
            foreach (TextBox textBox in textboxlist)
            {
                if (textBox.Name == "child" + childName)
                {
                    childTextBox = textBox;
                }
            }
            int gridIndex = 0;

            foreach (Grid grid in GenerationGridList)
            {
                if (grid.Children.Contains(childTextBox))
                {
                    gridIndex = GenerationGridList.IndexOf(grid);
                }
            }

            for (int i = 0; i < GenerationGridList[gridIndex].Children.Count; i++)
            {
                if (GenerationGridList[gridIndex].Children[i].GetType() == typeof(Line))
                {
                    GenerationGridList[gridIndex].Children.Remove(GenerationGridList[gridIndex].Children[i]);
                    i--;
                }
            }

            int childColumnIndex = Grid.GetColumn(childTextBox);
            GenerationGridList[gridIndex].ColumnDefinitions.Add(new ColumnDefinition());
            GenerationGridList[gridIndex].ColumnDefinitions.Add(new ColumnDefinition());

            foreach (TextBox textbox in textboxlist)
            {
                if(textboxlist.IndexOf(textbox) > textboxlist.IndexOf(childTextBox))
                {
                    Grid.SetColumn(textbox, Grid.GetColumn(textbox) + 2);
                }
            }

            foreach (Label label in birthDateLabelList)
            {
                if (birthDateLabelList.IndexOf(label) > textboxlist.IndexOf(childTextBox))
                {
                    Grid.SetColumn(label, Grid.GetColumn(label) + 2);
                }
            }

            foreach (Label label in deathDateLabelList)
            {
                if (deathDateLabelList.IndexOf(label) > textboxlist.IndexOf(childTextBox))
                {
                    Grid.SetColumn(label, Grid.GetColumn(label) + 2);
                }
            }

            textboxlist.Insert(textboxlist.IndexOf(childTextBox) + 1, new TextBox());
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].Text = partnerName;
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].HorizontalAlignment = HorizontalAlignment.Center;
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].VerticalAlignment = VerticalAlignment.Center;
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].HorizontalContentAlignment = HorizontalAlignment.Center;
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].Width = 250;
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].Name = "partner" + partnerName;
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].Margin = new Thickness(0, 0, 0, 0);
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].TextChanged += NameChanged;
            GenerationGridList[gridIndex].Children.Add(textboxlist[textboxlist.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(textboxlist[textboxlist.IndexOf(childTextBox) + 1], childColumnIndex + 2);

            birthDateLabelList.Insert(textboxlist.IndexOf(childTextBox) + 1, new Label());
            if (birthDate != null)
            {
                birthDateLabelList[textboxlist.IndexOf(childTextBox) + 1].Content = "☆" + birthDate.Value.ToShortDateString();
                birthDateLabelList[textboxlist.IndexOf(childTextBox) + 1].BorderBrush = Brushes.Transparent;
            }
            else
            {
                birthDateLabelList[textboxlist.IndexOf(childTextBox) + 1].Content = "☆";
                birthDateLabelList[textboxlist.IndexOf(childTextBox) + 1].BorderBrush = Brushes.Transparent;
            }
            GenerationGridList[gridIndex].Children.Add(birthDateLabelList[textboxlist.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(birthDateLabelList[textboxlist.IndexOf(childTextBox) + 1], childColumnIndex + 2);
            Grid.SetRow(birthDateLabelList[textboxlist.IndexOf(childTextBox) + 1], 1);

            deathDateLabelList.Insert(textboxlist.IndexOf(childTextBox) + 1, new Label());
            if (birthDate != null)
            {
                deathDateLabelList[textboxlist.IndexOf(childTextBox) + 1].Content = "☆" + birthDate.Value.ToShortDateString();
                deathDateLabelList[textboxlist.IndexOf(childTextBox) + 1].BorderBrush = Brushes.Transparent;
            }
            else
            {
                deathDateLabelList[textboxlist.IndexOf(childTextBox) + 1].Content = "☆";
                deathDateLabelList[textboxlist.IndexOf(childTextBox) + 1].BorderBrush = Brushes.Transparent;
            }
            GenerationGridList[gridIndex].Children.Add(deathDateLabelList[textboxlist.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(deathDateLabelList[textboxlist.IndexOf(childTextBox) + 1], childColumnIndex + 2);
            Grid.SetRow(deathDateLabelList[textboxlist.IndexOf(childTextBox) + 1], 2);

            ConnectChildrenToParents(PersonTree.GetNodeByName(PersonTree.Tree, childName));
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
            textboxlist[textboxlist.Count - 1].TextChanged += NameChanged;
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
            int parentIndex = 0;
            int parentColumnIndex = 0;
            bool found = false;
            for(int i = 0; i < ParentsGridList.Count; i++)
            {
                for (int j = 0; j < ParentsGridList[i].Children.Count && !found; j++)
                {
                    if (ParentsGridList[i].Children[j].GetType() == typeof(TextBox))
                    {
                        if ((string)ParentsGridList[i].Children[j].GetValue(TextBox.TextProperty) == person.Parent.Value.Name)
                        {
                            parentIndex = i;
                            parentColumnIndex = Grid.GetColumn(ParentsGridList[i].Children[j]) + 1;
                            found = true;
                        }
                    }
                }
            }

            Line verticalLine1 = new Line()
            {
                Stroke = Brushes.Black,
                Visibility = Visibility.Visible,
                StrokeThickness = 1,
                X1 = 12.5,
                X2 = 12.5,
                Y1 = SystemFonts.MessageFontSize,
                Y2 = SystemFonts.MessageFontSize * 2 + 1,
                Stretch = Stretch.None
            };
            ParentsGridList[parentIndex].Children.Add(verticalLine1);
            Grid.SetRow(verticalLine1, 0);
            Grid.SetColumn(verticalLine1, parentColumnIndex);

            Line verticalLine2 = new Line()
            {
                Stroke = Brushes.Black,
                Visibility = Visibility.Visible,
                StrokeThickness = 1,
                X1 = 12.5,
                X2 = 12.5,
                Y1 = 0,
                Y2 = SystemFonts.MessageFontSize * 2 + 2,
                Stretch = Stretch.None
            };
            ParentsGridList[parentIndex].Children.Add(verticalLine2);
            Grid.SetRow(verticalLine2, 1);
            Grid.SetColumn(verticalLine2, parentColumnIndex);

            Line verticalLine3 = new Line()
            {
                Stroke = Brushes.Black,
                Visibility = Visibility.Visible,
                StrokeThickness = 1,
                X1 = 12.5,
                X2 = 12.5,
                Y1 = 0,
                Y2 = SystemFonts.MessageFontSize * 2 + 1,
                Stretch = Stretch.None
            };
            ParentsGridList[parentIndex].Children.Add(verticalLine3);
            Grid.SetRow(verticalLine3, 2);
            Grid.SetColumn(verticalLine3, parentColumnIndex);

            for (int i = 0; i < GenerationGridList.Count; i++)
            {
                for (int j = 0; j < GenerationGridList[i].Children.Count; j++)
                {
                    if (GenerationGridList[i].Children[j].GetType() == typeof(TextBox))
                    {
                        int childColumnIndex = Grid.GetColumn(GenerationGridList[i].Children[j]);
                        double middleColumnIndex = ((GenerationGridList[i].ColumnDefinitions.Count - 1) *
                                              (GenerationGridList[i].ColumnDefinitions.Count / 2.0)) /
                                              (GenerationGridList[i].ColumnDefinitions.Count);
                        if (((TextBox)GenerationGridList[i].Children[j]).Name.Substring(0, 5) == "child")
                        {
                            Line verticalLine4 = new Line()
                            {
                                Stroke = Brushes.Black,
                                Visibility = Visibility.Visible,
                                StrokeThickness = 1,
                                X1 = 125,
                                X2 = 125,
                                Y1 = 3,
                                Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                Stretch = Stretch.None
                            };
                            GenerationGridList[i].Children.Add(verticalLine4);
                            Grid.SetRow(verticalLine4, 0);
                            Grid.SetColumn(verticalLine4, childColumnIndex);

                            if (childColumnIndex < middleColumnIndex)
                            {
                                Line horizontalLine1 = new Line()
                                {
                                    Stroke = Brushes.Black,
                                    Visibility = Visibility.Visible,
                                    StrokeThickness = 1,
                                    X1 = 125,
                                    X2 = 250,
                                    Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Stretch = Stretch.None
                                };
                                GenerationGridList[i].Children.Add(horizontalLine1);
                                Grid.SetRow(horizontalLine1, 0);
                                Grid.SetColumn(horizontalLine1, childColumnIndex);

                                Line horizontalLine2 = new Line()
                                {
                                    Stroke = Brushes.Black,
                                    Visibility = Visibility.Visible,
                                    StrokeThickness = 1,
                                    X1 = 0,
                                    X2 = 12.5,
                                    Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Stretch = Stretch.None
                                };
                                GenerationGridList[i].Children.Add(horizontalLine2);
                                Grid.SetRow(horizontalLine2, 0);
                                Grid.SetColumn(horizontalLine2, childColumnIndex + 1);

                                if(childColumnIndex > 0)
                                {
                                    Line horizontalLine3 = new Line()
                                    {
                                        Stroke = Brushes.Black,
                                        Visibility = Visibility.Visible,
                                        StrokeThickness = 1,
                                        X1 = 0,
                                        X2 = 125,
                                        Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Stretch = Stretch.None
                                    };
                                    GenerationGridList[i].Children.Add(horizontalLine3);
                                    Grid.SetRow(horizontalLine3, 0);
                                    Grid.SetColumn(horizontalLine3, childColumnIndex);

                                    Line horizontalLine4 = new Line()
                                    {
                                        Stroke = Brushes.Black,
                                        Visibility = Visibility.Visible,
                                        StrokeThickness = 1,
                                        X1 = 0,
                                        X2 = 25,
                                        Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Stretch = Stretch.None
                                    };
                                    GenerationGridList[i].Children.Add(horizontalLine4);
                                    Grid.SetRow(horizontalLine4, 0);
                                    Grid.SetColumn(horizontalLine4, childColumnIndex - 1);
                                }
                            }
                            else if (childColumnIndex > middleColumnIndex)
                            {
                                Line horizontalLine1 = new Line()
                                {
                                    Stroke = Brushes.Black,
                                    Visibility = Visibility.Visible,
                                    StrokeThickness = 1,
                                    X1 = 0,
                                    X2 = 125,
                                    Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Stretch = Stretch.None
                                };
                                GenerationGridList[i].Children.Add(horizontalLine1);
                                Grid.SetRow(horizontalLine1, 0);
                                Grid.SetColumn(horizontalLine1, childColumnIndex);

                                Line horizontalLine2 = new Line()
                                {
                                    Stroke = Brushes.Black,
                                    Visibility = Visibility.Visible,
                                    StrokeThickness = 1,
                                    X1 = 0,
                                    X2 = 25,
                                    Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Stretch = Stretch.None
                                };
                                GenerationGridList[i].Children.Add(horizontalLine2);
                                Grid.SetRow(horizontalLine2, 0);
                                Grid.SetColumn(horizontalLine2, childColumnIndex - 1);

                                if (childColumnIndex != GenerationGridList[i].ColumnDefinitions.Count - 1)
                                {
                                    TextBox nextPerson = (TextBox)GenerationGridList[i].Children.OfType<TextBox>().Cast<TextBox>().First(e => Grid.GetRow(e) == 0 &&
                                                                                                      Grid.GetColumn(e) == childColumnIndex + 2);
                                    if(nextPerson.Name.Substring(0, 5) == "child")
                                    {
                                        Line horizontalLine3 = new Line()
                                        {
                                            Stroke = Brushes.Black,
                                            Visibility = Visibility.Visible,
                                            StrokeThickness = 1,
                                            X1 = 125,
                                            X2 = 250,
                                            Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                            Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                            Stretch = Stretch.None
                                        };
                                        GenerationGridList[i].Children.Add(horizontalLine3);
                                        Grid.SetRow(horizontalLine3, 0);
                                        Grid.SetColumn(horizontalLine3, childColumnIndex);

                                        Line horizontalLine4 = new Line()
                                        {
                                            Stroke = Brushes.Black,
                                            Visibility = Visibility.Visible,
                                            StrokeThickness = 1,
                                            X1 = 0,
                                            X2 = 12.5,
                                            Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                            Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                            Stretch = Stretch.None
                                        };
                                        GenerationGridList[i].Children.Add(horizontalLine4);
                                        Grid.SetRow(horizontalLine4, 0);
                                        Grid.SetColumn(horizontalLine4, childColumnIndex - 1);
                                    }
                                }
                            }
                            else
                            {
                                if(childColumnIndex > 0)
                                {
                                    Line horizontalLine1 = new Line()
                                    {
                                        Stroke = Brushes.Black,
                                        Visibility = Visibility.Visible,
                                        StrokeThickness = 1,
                                        X1 = 0,
                                        X2 = 250,
                                        Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Stretch = Stretch.None
                                    };
                                    GenerationGridList[i].Children.Add(horizontalLine1);
                                    Grid.SetRow(horizontalLine1, 0);
                                    Grid.SetColumn(horizontalLine1, childColumnIndex);

                                    Line horizontalLine2 = new Line()
                                    {
                                        Stroke = Brushes.Black,
                                        Visibility = Visibility.Visible,
                                        StrokeThickness = 1,
                                        X1 = 0,
                                        X2 = 25,
                                        Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Stretch = Stretch.None
                                    };
                                    GenerationGridList[i].Children.Add(horizontalLine2);
                                    Grid.SetRow(horizontalLine2, 0);
                                    Grid.SetColumn(horizontalLine2, childColumnIndex - 1);

                                    Line horizontalLine3 = new Line()
                                    {
                                        Stroke = Brushes.Black,
                                        Visibility = Visibility.Visible,
                                        StrokeThickness = 1,
                                        X1 = 0,
                                        X2 = 25,
                                        Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Stretch = Stretch.None
                                    };
                                    GenerationGridList[i].Children.Add(horizontalLine3);
                                    Grid.SetRow(horizontalLine3, 0);
                                    Grid.SetColumn(horizontalLine3, childColumnIndex + 1);
                                }
                            }
                        }
                        else if (((TextBox)GenerationGridList[i].Children[j]).Name.Substring(0, 7) == "partner")
                        {
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
                            GenerationGridList[i].Children.Add(horizontalLine1);
                            Grid.SetRow(horizontalLine1, 0);
                            Grid.SetColumn(horizontalLine1, childColumnIndex - 1);

                            if (childColumnIndex < middleColumnIndex)
                            {
                                Line horizontalLine2 = new Line()
                                {
                                    Stroke = Brushes.Black,
                                    Visibility = Visibility.Visible,
                                    StrokeThickness = 1,
                                    X1 = 12.5,
                                    X2 = 25,
                                    Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Stretch = Stretch.None
                                };
                                GenerationGridList[i].Children.Add(horizontalLine2);
                                Grid.SetRow(horizontalLine2, 0);
                                Grid.SetColumn(horizontalLine2, childColumnIndex - 1);

                                Line horizontalLine3 = new Line()
                                {
                                    Stroke = Brushes.Black,
                                    Visibility = Visibility.Visible,
                                    StrokeThickness = 1,
                                    X1 = 0,
                                    X2 = 250,
                                    Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Stretch = Stretch.None
                                };
                                GenerationGridList[i].Children.Add(horizontalLine3);
                                Grid.SetRow(horizontalLine3, 0);
                                Grid.SetColumn(horizontalLine3, childColumnIndex);

                                Line horizontalLine4 = new Line()
                                {
                                    Stroke = Brushes.Black,
                                    Visibility = Visibility.Visible,
                                    StrokeThickness = 1,
                                    X1 = 0,
                                    X2 = 12.5,
                                    Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Stretch = Stretch.None
                                };
                                GenerationGridList[i].Children.Add(horizontalLine4);
                                Grid.SetRow(horizontalLine4, 0);
                                Grid.SetColumn(horizontalLine4, childColumnIndex + 1);
                            }
                            else if (childColumnIndex > middleColumnIndex)
                            {
                                if(childColumnIndex == middleColumnIndex + 2)
                                {
                                    Line horizontalLine2 = new Line()
                                    {
                                        Stroke = Brushes.Black,
                                        Visibility = Visibility.Visible,
                                        StrokeThickness = 1,
                                        X1 = 12.5,
                                        X2 = 25,
                                        Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Stretch = Stretch.None
                                    };
                                    GenerationGridList[i].Children.Add(horizontalLine2);
                                    Grid.SetRow(horizontalLine2, 0);
                                    Grid.SetColumn(horizontalLine2, childColumnIndex - 1);

                                    Line horizontalLine3 = new Line()
                                    {
                                        Stroke = Brushes.Black,
                                        Visibility = Visibility.Visible,
                                        StrokeThickness = 1,
                                        X1 = 0,
                                        X2 = 250,
                                        Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Stretch = Stretch.None
                                    };
                                    GenerationGridList[i].Children.Add(horizontalLine3);
                                    Grid.SetRow(horizontalLine3, 0);
                                    Grid.SetColumn(horizontalLine3, childColumnIndex);
                                }
                               
                                else if(childColumnIndex != GenerationGridList[i].ColumnDefinitions.Count - 1)
                                {
                                    Line horizontalLine4 = new Line()
                                    {
                                        Stroke = Brushes.Black,
                                        Visibility = Visibility.Visible,
                                        StrokeThickness = 1,
                                        X1 = 0,
                                        X2 = 250,
                                        Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Stretch = Stretch.None
                                    };
                                    GenerationGridList[i].Children.Add(horizontalLine4);
                                    Grid.SetRow(horizontalLine4, 0);
                                    Grid.SetColumn(horizontalLine4, childColumnIndex);

                                    Line horizontalLine5 = new Line()
                                    {
                                        Stroke = Brushes.Black,
                                        Visibility = Visibility.Visible,
                                        StrokeThickness = 1,
                                        X1 = 0,
                                        X2 = 12.5,
                                        Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Stretch = Stretch.None
                                    };
                                    GenerationGridList[i].Children.Add(horizontalLine5);
                                    Grid.SetRow(horizontalLine5, 0);
                                    Grid.SetColumn(horizontalLine5, childColumnIndex + 1);

                                    Line horizontalLine6 = new Line()
                                    {
                                        Stroke = Brushes.Black,
                                        Visibility = Visibility.Visible,
                                        StrokeThickness = 1,
                                        X1 = 125,
                                        X2 = 250,
                                        Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Stretch = Stretch.None
                                    };
                                    GenerationGridList[i].Children.Add(horizontalLine6);
                                    Grid.SetRow(horizontalLine6, 0);
                                    Grid.SetColumn(horizontalLine6, childColumnIndex - 2);

                                    Line horizontalLine7 = new Line()
                                    {
                                        Stroke = Brushes.Black,
                                        Visibility = Visibility.Visible,
                                        StrokeThickness = 1,
                                        X1 = 0,
                                        X2 = 25,
                                        Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                        Stretch = Stretch.None
                                    };
                                    GenerationGridList[i].Children.Add(horizontalLine7);
                                    Grid.SetRow(horizontalLine7, 0);
                                    Grid.SetColumn(horizontalLine7, childColumnIndex - 1);
                                }
                            }
                            else
                            {
                                Line horizontalLine2 = new Line()
                                {
                                    Stroke = Brushes.Black,
                                    Visibility = Visibility.Visible,
                                    StrokeThickness = 1,
                                    X1 = 0,
                                    X2 = 250,
                                    Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Stretch = Stretch.None
                                };
                                GenerationGridList[i].Children.Add(horizontalLine2);
                                Grid.SetRow(horizontalLine2, 0);
                                Grid.SetColumn(horizontalLine2, childColumnIndex);

                                Line horizontalLine3 = new Line()
                                {
                                    Stroke = Brushes.Black,
                                    Visibility = Visibility.Visible,
                                    StrokeThickness = 1,
                                    X1 = 0,
                                    X2 = 25,
                                    Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Stretch = Stretch.None
                                };
                                GenerationGridList[i].Children.Add(horizontalLine3);
                                Grid.SetRow(horizontalLine3, 0);
                                Grid.SetColumn(horizontalLine3, childColumnIndex - 1);

                                Line horizontalLine4 = new Line()
                                {
                                    Stroke = Brushes.Black,
                                    Visibility = Visibility.Visible,
                                    StrokeThickness = 1,
                                    X1 = 0,
                                    X2 = 25,
                                    Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                                    Stretch = Stretch.None
                                };
                                GenerationGridList[i].Children.Add(horizontalLine4);
                                Grid.SetRow(horizontalLine4, 0);
                                Grid.SetColumn(horizontalLine4, childColumnIndex + 1);
                            }
                        }
                    }
                }
            }
        }

        private void NameChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Node<Person> person = null;
            if(textBox.Name.Substring(0, 5) == "child")
            {
                person = PersonTree.GetNodeByName(PersonTree.Tree, textBox.Name.Substring(5, textBox.Name.Length - 5));
                textBox.Name = "child" + textBox.Text;
                person.Value.Name = textBox.Text;
            }
            else if(textBox.Name.Substring(0, 7) == "partner")
            {
                person = PersonTree.GetNodeByName(PersonTree.Tree, textBox.Name.Substring(7, textBox.Name.Length - 7));
                textBox.Name = "partner" + textBox.Text;
                person.Value.Name = textBox.Text;
            }
        }
    }
}
