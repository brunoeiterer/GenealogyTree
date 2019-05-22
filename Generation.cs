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
                                Grid.SetColumn(GenerationGridList[GenerationGridList.Count - 1], BaseGrid.ColumnDefinitions.Count - 1);
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
                Grid.SetColumn(GenerationGridList[GenerationGridList.Count - 1], BaseGrid.ColumnDefinitions.Count - 1);
            }

            foreach(Grid grid in GenerationGridList)
            {
                if(GenerationGridList.IndexOf(grid) != 0)
                {
                    grid.Margin = new Thickness(25, 0, 0, 0);
                }
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

            if(person.Parent != null)
            {
                for (int i = 0; i < GenerationGridList.Count && !found; i++)
                {
                    if (person.Parent.Value.Name == GenerationParentsList[i])
                    {
                        generationGridIndex = i;
                        found = true;
                    }
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
                GenerationGridList[generationGridIndex].ColumnDefinitions.Add(new ColumnDefinition());
                GenerationGridList[generationGridIndex].ColumnDefinitions[GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1].Width = new GridLength(25);

                if(ParentsGridList == null)
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

                    GenerationGridList[generationGridIndex].Children.Add(horizontalLine1);
                    Grid.SetColumn(horizontalLine1, GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);
                }

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

            //if (ParentsGridList != null)
            //{
            //    ConnectChildrenToParents(person);
            //}

            GenerationChangedEventArgs eventArgs = new GenerationChangedEventArgs();
            GenerationChanged?.Invoke(this, eventArgs);
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
            GenerationGridList[gridIndex].ColumnDefinitions[GenerationGridList[gridIndex].ColumnDefinitions.Count - 1].Width = 
                new GridLength(25);
            GenerationGridList[gridIndex].ColumnDefinitions.Add(new ColumnDefinition());
            GenerationGridList[gridIndex].ColumnDefinitions[GenerationGridList[gridIndex].ColumnDefinitions.Count - 1].Width =
                new GridLength(250);

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

            foreach(TextBlock textBlock in textBlockList)
            {
                if(Grid.GetColumn(textBlock) > Grid.GetColumn(childTextBox))
                {
                    Grid.SetColumn(textBlock, Grid.GetColumn(textBlock) + 2);
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

            //ConnectChildrenToParents(PersonTree.GetNodeByName(PersonTree.Tree, childName));

            GenerationChangedEventArgs eventArgs = new GenerationChangedEventArgs();
            GenerationChanged?.Invoke(this, eventArgs);
    }

    public event GenerationChangedEventHandler GenerationChanged;

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
