using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace GenealogyTree
{
    public class Generation
    {
        public Grid BaseGrid { get; set; }
        public List<Grid> GenerationGridList { get; set; }
        private List<string> GenerationParentsList { get; set; }
        public List<Grid> ParentsGridList { get; set; }
        private List<TextBox> textboxlist;
        private List<string> textBoxTextList;
        private List<Label> birthDateLabelList;
        private List<Label> deathDateLabelList;
        private List<TextBox> birthDateTextBoxList;
        private List<TextBox> deathDateTextBoxList;
        private List<Label> birthPlaceLabelList;
        private List<TextBox> birthPlaceTextBoxList;
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
                            if(((TextBox)parentsGridList[i].Children[j]).Name != string.Empty)
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
            textBoxTextList = new List<string>();
            birthDateLabelList = new List<Label>();
            deathDateLabelList = new List<Label>();
            textBlockList = new List<TextBlock>();
            birthDateTextBoxList = new List<TextBox>();
            deathDateTextBoxList = new List<TextBox>();
            birthPlaceLabelList = new List<Label>();
            birthPlaceTextBoxList = new List<TextBox>();

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

            if(GenerationGridList[generationGridIndex].RowDefinitions.Count < 4)
            {
                GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
                GenerationGridList[generationGridIndex].RowDefinitions[GenerationGridList[generationGridIndex].RowDefinitions.Count - 1].Height = new GridLength(25);
            }

            AddBirthDateLabel(person.Value.BirthDate);

            if (GenerationGridList[generationGridIndex].RowDefinitions.Count < 4)
            {
                GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
            }
            Grid.SetRow(birthDateLabelList[birthDateLabelList.Count - 1], 1);
            Grid.SetColumn(birthDateLabelList[birthDateLabelList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);
            Grid.SetRow(birthDateTextBoxList[birthDateTextBoxList.Count - 1], 1);
            Grid.SetColumn(birthDateTextBoxList[birthDateTextBoxList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

            GenerationGridList[generationGridIndex].Children.Add(birthDateLabelList[birthDateLabelList.Count - 1]);
            GenerationGridList[generationGridIndex].Children.Add(birthDateTextBoxList[birthDateTextBoxList.Count - 1]);

            AddDeathDateLabel(person.Value.DeathDate);

            if (GenerationGridList[generationGridIndex].RowDefinitions.Count < 4)
            {
                GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
            }
            Grid.SetRow(deathDateLabelList[deathDateLabelList.Count - 1], 2);
            Grid.SetColumn(deathDateLabelList[deathDateLabelList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);
            Grid.SetRow(deathDateTextBoxList[deathDateTextBoxList.Count - 1], 2);
            Grid.SetColumn(deathDateTextBoxList[deathDateTextBoxList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

            GenerationGridList[generationGridIndex].Children.Add(deathDateLabelList[deathDateLabelList.Count - 1]);
            GenerationGridList[generationGridIndex].Children.Add(deathDateTextBoxList[deathDateTextBoxList.Count - 1]);

            AddBirthPlaceLabel(person.Value.BirthPlace);

            if (GenerationGridList[generationGridIndex].RowDefinitions.Count < 4)
            {
                GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
            }
            Grid.SetRow(birthPlaceLabelList[birthPlaceLabelList.Count - 1], 3);
            Grid.SetColumn(birthPlaceLabelList[birthPlaceLabelList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);
            Grid.SetRow(birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1], 3);
            Grid.SetColumn(birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

            GenerationGridList[generationGridIndex].Children.Add(birthPlaceLabelList[birthPlaceLabelList.Count - 1]);
            GenerationGridList[generationGridIndex].Children.Add(birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1]);

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
                        Stretch = Stretch.None,
                        Name = "Child" + GenerationID.ToString().Replace("-", string.Empty)
                    };

                    GenerationGridList[generationGridIndex].Children.Add(horizontalLine1);
                    Grid.SetColumn(horizontalLine1, GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);
                }

                AddTextBox(person.Value.Partner, "partner");
                GenerationGridList[generationGridIndex].ColumnDefinitions.Add(new ColumnDefinition());
                GenerationGridList[generationGridIndex].ColumnDefinitions[GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1].Width = new GridLength(250);
                GenerationGridList[generationGridIndex].Children.Add(textboxlist[textboxlist.Count - 1]);
                Grid.SetColumn(textboxlist[textboxlist.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

                if (GenerationGridList[generationGridIndex].RowDefinitions.Count < 4)
                {
                    GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
                    GenerationGridList[generationGridIndex].RowDefinitions[GenerationGridList[generationGridIndex].RowDefinitions.Count - 1].Height = new GridLength(25);
                }

                AddBirthDateLabel(person.Value.PartnerBirthDate);

                if (GenerationGridList[generationGridIndex].RowDefinitions.Count < 4)
                {
                    GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
                }
                Grid.SetRow(birthDateLabelList[birthDateLabelList.Count - 1], 1);
                Grid.SetColumn(birthDateLabelList[birthDateLabelList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);
                Grid.SetRow(birthDateTextBoxList[birthDateTextBoxList.Count - 1], 1);
                Grid.SetColumn(birthDateTextBoxList[birthDateTextBoxList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

                GenerationGridList[generationGridIndex].Children.Add(birthDateLabelList[birthDateLabelList.Count - 1]);
                GenerationGridList[generationGridIndex].Children.Add(birthDateTextBoxList[birthDateTextBoxList.Count - 1]);

                AddDeathDateLabel(person.Value.PartnerDeathDate);

                if (GenerationGridList[generationGridIndex].RowDefinitions.Count < 4)
                {
                    GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
                }
                Grid.SetRow(deathDateLabelList[deathDateLabelList.Count - 1], 2);
                Grid.SetColumn(deathDateLabelList[deathDateLabelList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);
                Grid.SetRow(deathDateTextBoxList[deathDateTextBoxList.Count - 1], 2);
                Grid.SetColumn(deathDateTextBoxList[deathDateTextBoxList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

                GenerationGridList[generationGridIndex].Children.Add(deathDateLabelList[deathDateLabelList.Count - 1]);
                GenerationGridList[generationGridIndex].Children.Add(deathDateTextBoxList[deathDateTextBoxList.Count - 1]);

                AddBirthPlaceLabel(person.Value.BirthPlace);

                if (GenerationGridList[generationGridIndex].RowDefinitions.Count < 4)
                {
                    GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
                }
                Grid.SetRow(birthPlaceLabelList[birthPlaceLabelList.Count - 1], 3);
                Grid.SetColumn(birthPlaceLabelList[birthPlaceLabelList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);
                Grid.SetRow(birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1], 3);
                Grid.SetColumn(birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

                GenerationGridList[generationGridIndex].Children.Add(birthPlaceLabelList[birthPlaceLabelList.Count - 1]);
                GenerationGridList[generationGridIndex].Children.Add(birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1]);
            }

            GenerationChangedEventArgs eventArgs = new GenerationChangedEventArgs();
            GenerationChanged?.Invoke(this, eventArgs);
        }

        public void AddPartner(string childName, string partnerName, Nullable<DateTime> birthDate, Nullable<DateTime> deathDate, string birthPlace)
        {
            TextBox childTextBox = null;
            foreach (TextBox textBox in textboxlist)
            {
                if (textBox.Name == "child" + new String(childName.Where(ch => Char.IsLetterOrDigit(ch)).ToArray()))
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

            foreach(TextBox textBox in birthDateTextBoxList)
            {
                if (Grid.GetColumn(textBox) > Grid.GetColumn(childTextBox))
                {
                    Grid.SetColumn(textBox, Grid.GetColumn(textBox) + 2);
                }
            }

            foreach (TextBox textBox in deathDateTextBoxList)
            {
                if (Grid.GetColumn(textBox) > Grid.GetColumn(childTextBox))
                {
                    Grid.SetColumn(textBox, Grid.GetColumn(textBox) + 2);
                }
            }

            foreach(Label label in birthPlaceLabelList)
            {
                if(Grid.GetColumn(label) > Grid.GetColumn(childTextBox))
                {
                    Grid.SetColumn(label, Grid.GetColumn(label) + 2);
                }
            }

            foreach (TextBox textBox in birthPlaceTextBoxList)
            {
                if (Grid.GetColumn(textBox) > Grid.GetColumn(childTextBox))
                {
                    Grid.SetColumn(textBox, Grid.GetColumn(textBox) + 2);
                }
            }

            textboxlist.Insert(textboxlist.IndexOf(childTextBox) + 1, new TextBox());
            textBoxTextList.Insert(textboxlist.IndexOf(childTextBox) + 1, new String(partnerName.ToArray()));
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].Text = partnerName;
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].HorizontalAlignment = HorizontalAlignment.Center;
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].VerticalAlignment = VerticalAlignment.Center;
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].HorizontalContentAlignment = HorizontalAlignment.Center;
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].Width = 250;
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].Name = "partner" + new String(partnerName.Where(ch => Char.IsLetterOrDigit(ch)).ToArray());
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].Margin = new Thickness(0, 0, 0, 0);
            textboxlist[textboxlist.IndexOf(childTextBox) + 1].LostFocus += NameChanged;
            GenerationGridList[gridIndex].Children.Add(textboxlist[textboxlist.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(textboxlist[textboxlist.IndexOf(childTextBox) + 1], childColumnIndex + 2);

            birthDateLabelList.Insert(textboxlist.IndexOf(childTextBox) + 1, new Label());
            birthDateLabelList[birthDateLabelList.Count - 1].Margin = new Thickness(0, 0, 25, 0);
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
            deathDateLabelList[deathDateLabelList.Count - 1].Margin = new Thickness(0, 0, 25, 0);
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

            birthPlaceLabelList.Insert(textboxlist.IndexOf(childTextBox) + 1, new Label());
            birthPlaceLabelList[birthPlaceLabelList.Count - 1].Margin = new Thickness(0, 0, 25, 25);
            birthPlaceLabelList[birthPlaceLabelList.Count - 1].Content = "📌";
            birthPlaceLabelList[birthPlaceLabelList.Count - 1].BorderBrush = Brushes.Transparent;
            GenerationGridList[gridIndex].Children.Add(birthDateLabelList[textboxlist.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(birthDateLabelList[textboxlist.IndexOf(childTextBox) + 1], childColumnIndex + 2);
            Grid.SetRow(birthDateLabelList[textboxlist.IndexOf(childTextBox) + 1], 3);

            birthDateTextBoxList.Insert(textboxlist.IndexOf(childTextBox) + 1, new TextBox());
            if (birthDate != null)
            {
                birthDateTextBoxList[textboxlist.IndexOf(childTextBox) + 1].Text = birthDate.Value.ToShortDateString();
            }
            else
            {
                birthDateTextBoxList[textboxlist.IndexOf(childTextBox) + 1].Text = string.Empty;
            }
            GenerationGridList[gridIndex].Children.Add(birthDateTextBoxList[textboxlist.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(birthDateTextBoxList[textboxlist.IndexOf(childTextBox) + 1], childColumnIndex + 2);
            Grid.SetRow(birthDateTextBoxList[textboxlist.IndexOf(childTextBox) + 1], 1);

            deathDateTextBoxList.Insert(textboxlist.IndexOf(childTextBox) + 1, new TextBox());
            if (birthDate != null)
            {
                deathDateTextBoxList[textboxlist.IndexOf(childTextBox) + 1].Text = birthDate.Value.ToShortDateString();
            }
            else
            {
                deathDateTextBoxList[textboxlist.IndexOf(childTextBox) + 1].Text = string.Empty;
            }
            GenerationGridList[gridIndex].Children.Add(deathDateTextBoxList[textboxlist.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(deathDateTextBoxList[textboxlist.IndexOf(childTextBox) + 1], childColumnIndex + 2);
            Grid.SetRow(deathDateTextBoxList[textboxlist.IndexOf(childTextBox) + 1], 1);

            birthPlaceTextBoxList.Insert(textboxlist.IndexOf(childTextBox) + 1, new TextBox());
            birthPlaceTextBoxList[textboxlist.IndexOf(childTextBox) + 1].Text = birthPlace;
            birthPlaceTextBoxList[textboxlist.IndexOf(childTextBox) + 1].Margin = new Thickness(0, 0, 0, 50);
            birthPlaceTextBoxList[textboxlist.IndexOf(childTextBox) + 1].LostFocus += BirthPlaceChanged;
            GenerationGridList[gridIndex].Children.Add(birthPlaceTextBoxList[textboxlist.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(birthPlaceTextBoxList[textboxlist.IndexOf(childTextBox) + 1], childColumnIndex + 2);
            Grid.SetRow(birthPlaceTextBoxList[textboxlist.IndexOf(childTextBox) + 1], 3);

            GenerationChangedEventArgs eventArgs = new GenerationChangedEventArgs();
            GenerationChanged?.Invoke(this, eventArgs);
        }

        private void BirthPlaceChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Node<Person> person = PersonTree.GetNodeByName(PersonTree.Tree, textboxlist[birthDateTextBoxList.IndexOf(textBox)].Text);

            if(person.Value.Name == textboxlist[birthDateTextBoxList.IndexOf(textBox)].Text)
            {
                person.Value.BirthPlace = textBox.Text;
            }
            else if(person.Value.Partner == textboxlist[birthDateTextBoxList.IndexOf(textBox)].Text)
            {
                person.Value.PartnerBirthPlace = textBox.Text;
            }
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
            textboxlist[textboxlist.Count - 1].Name = type + new String(value.Where(ch => Char.IsLetterOrDigit(ch)).ToArray());
            textboxlist[textboxlist.Count - 1].Margin = new Thickness(0, 0, 0, 0);
            textboxlist[textboxlist.Count - 1].LostFocus += NameChanged;

            textBoxTextList.Add(new string(value.ToArray()));
        }

        private void AddBirthDateLabel(Nullable<DateTime> date)
        {
            Label newLabel = new Label()
            {
                Content = "☆",
                BorderBrush = Brushes.Transparent,
                Margin = new Thickness(0, 0, 25, 0)
            };
            birthDateLabelList.Add(newLabel);

            birthDateTextBoxList.Add(new TextBox());
            birthDateTextBoxList[birthDateTextBoxList.Count - 1].Width = 200;
            birthDateTextBoxList[birthDateTextBoxList.Count - 1].Height = 20;
            birthDateTextBoxList[birthDateTextBoxList.Count - 1].LostFocus += ChangeBirthDate;
            if (date != null)
            {
                birthDateTextBoxList[birthDateTextBoxList.Count - 1].Text = date.Value.ToShortDateString();
            }
        }

        private void AddBirthPlaceLabel(string birthPlace)
        {
            Label newLabel = new Label()
            {
                Content = "📌",
                BorderBrush = Brushes.Transparent,
                Margin = new Thickness(0, 0, 25, 0)
            };
            birthPlaceLabelList.Add(newLabel);

            birthPlaceTextBoxList.Add(new TextBox());
            birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1].Width = 200;
            birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1].Height = 20;
            birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1].LostFocus += ChangeBirthPlace;
            birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1].Text = birthPlace;
            birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1].Margin = new Thickness(0, 0, 0, 50);
        }

        private void ChangeBirthPlace(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Node<Person> person = null;

            person = PersonTree.GetNodeByName(PersonTree.Tree, textboxlist[birthPlaceTextBoxList.IndexOf(textBox)].Text);
            
            if(person.Value.Name == textboxlist[birthPlaceTextBoxList.IndexOf(textBox)].Text)
            {
                person.Value.BirthPlace = textBox.Text;
            }
            else if(person.Value.Partner == textboxlist[birthPlaceTextBoxList.IndexOf(textBox)].Text)
            {
                person.Value.PartnerBirthPlace = textBox.Text;
            }
        }

        private void ChangeBirthDate(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Node<Person> person = null;
            string name = string.Empty;

            person = PersonTree.GetNodeByName(PersonTree.Tree, textboxlist[birthDateTextBoxList.IndexOf(textBox)].Text);
            name = textboxlist[birthDateTextBoxList.IndexOf(textBox)].Text;

            DateTime newDate;

            try
            {
                newDate = DateTime.ParseExact(textBox.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None);

                if(name == person.Value.Name)
                {
                    person.Value.BirthDate = (Nullable<DateTime>)newDate;
                }
                else if (name == person.Value.Partner)
                {
                    person.Value.PartnerBirthDate = (Nullable<DateTime>)newDate;
                }
            }
            catch (FormatException)
            {
                if (textBox.Text.ToString() == string.Empty)
                {
                    if (name == person.Value.Name)
                    {
                        person.Value.BirthDate = null;
                    }
                    else if (name == person.Value.Partner)
                    {
                        person.Value.PartnerBirthDate = null;
                    }
                }
                else
                {
                    if (name == person.Value.Name)
                    {
                        if (person.Value.BirthDate == null)
                        {
                            textBox.Text = string.Empty;
                        }
                        else
                        {
                            textBox.Text = person.Value.BirthDate.Value.ToShortDateString();
                        }
                    }
                    else if (name == person.Value.Partner)
                    {
                        if (person.Value.PartnerBirthDate == null)
                        {
                            textBox.Text = string.Empty;
                        }
                        else
                        {
                            textBox.Text = person.Value.PartnerBirthDate.Value.ToShortDateString();
                        }
                    }

                    MessageBox.Show(Application.Current.Resources["DateChangeInvalidDateError"].ToString(),
                        Application.Current.Resources["DateChangeInvalidDateErrorMessageBoxTitle"].ToString());
                }
            }
        }

        private void AddDeathDateLabel(Nullable<DateTime> date)
        {
            Label newLabel = new Label()
            {
                Content = "✞",
                BorderBrush = Brushes.Transparent,
                Margin = new Thickness(0, 0, 25, 0)
            };
            deathDateLabelList.Add(newLabel);

            deathDateTextBoxList.Add(new TextBox());
            deathDateTextBoxList[deathDateTextBoxList.Count - 1].Width = 200;
            deathDateTextBoxList[deathDateTextBoxList.Count - 1].Height = 20;
            deathDateTextBoxList[deathDateTextBoxList.Count - 1].LostFocus += ChangeDeathDate;
            if (date != null)
            {
                deathDateTextBoxList[deathDateTextBoxList.Count - 1].Text = date.Value.ToShortDateString();
            }
        }

        private void ChangeDeathDate(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Node<Person> person = null;
            string name = string.Empty;

            person = PersonTree.GetNodeByName(PersonTree.Tree, textboxlist[deathDateTextBoxList.IndexOf(textBox)].Text);
            name = textboxlist[deathDateTextBoxList.IndexOf(textBox)].Text;

            DateTime newDate;

            try
            {
                newDate = DateTime.ParseExact(textBox.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None);

                if (name == person.Value.Name)
                {
                    person.Value.DeathDate = (Nullable<DateTime>)newDate;
                }
                else if (name == person.Value.Partner)
                {
                    person.Value.PartnerDeathDate = (Nullable<DateTime>)newDate;
                }
            }
            catch (FormatException)
            {
                if (textBox.Text.ToString() == string.Empty)
                {
                    if (name == person.Value.Name)
                    {
                        person.Value.DeathDate = null;
                    }
                    else if (name == person.Value.Partner)
                    {
                        person.Value.PartnerDeathDate = null;
                    }
                }
                else
                {
                    if (name == person.Value.Name)
                    {
                        if(person.Value.DeathDate == null)
                        {
                            textBox.Text = string.Empty;
                        }
                        else
                        {
                            textBox.Text = person.Value.DeathDate.Value.ToShortDateString();
                        }
                    }
                    else if (name == person.Value.Partner)
                    {
                        if(person.Value.PartnerDeathDate == null)
                        {
                            textBox.Text = string.Empty;
                        }
                        else
                        {
                            textBox.Text = person.Value.PartnerDeathDate.Value.ToShortDateString();
                        }
                    }
                    MessageBox.Show(Application.Current.Resources["DateChangeInvalidDateError"].ToString(),
                        Application.Current.Resources["DateChangeInvalidDateErrorMessageBoxTitle"].ToString());
                }
            }
        }

        private void NameChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Node<Person> person = null;

            if(textBox.Name.Substring(0, 5) == "child")
            {
                if (textBox.Text != textBoxTextList[textboxlist.IndexOf(textBox)])
                {
                    if (PersonTree.GetNodeByName(PersonTree.Tree, textBox.Text) != null)
                    {
                        MessageBox.Show(Application.Current.Resources["NameChangeDuplicatedNameError"].ToString(),
                            Application.Current.Resources["NameChangeDuplicatedNameErrorMessageBoxTitle"].ToString());

                        textBox.Text = textBoxTextList[textboxlist.IndexOf(textBox)];
                    }
                    else
                    {
                        person = PersonTree.GetNodeByName(PersonTree.Tree, textBoxTextList[textboxlist.IndexOf(textBox)]);
                        textBox.Name = "child" + new String(textBox.Text.Where(ch => Char.IsLetterOrDigit(ch)).ToArray());
                        person.Value.Name = textBox.Text;
                        textBoxTextList[textboxlist.IndexOf(textBox)] = textBox.Text;
                    }
                }
            }
            else if(textBox.Name.Substring(0, 7) == "partner")
            {
                if (textBox.Text != textBoxTextList[textboxlist.IndexOf(textBox)])
                {
                    if (PersonTree.GetNodeByName(PersonTree.Tree, textBox.Text) != null)
                    {
                        MessageBox.Show(Application.Current.Resources["NameChangeDuplicatedNameError"].ToString(),
                            Application.Current.Resources["NameChangeDuplicatedNameErrorMessageBoxTitle"].ToString());

                        textBox.Text = textBoxTextList[textboxlist.IndexOf(textBox)];
                    }
                    else
                    {
                        person = PersonTree.GetNodeByName(PersonTree.Tree, textBoxTextList[textboxlist.IndexOf(textBox)]);
                        textBox.Name = "partner" + new String(textBox.Text.Where(ch => Char.IsLetterOrDigit(ch)).ToArray());
                        person.Value.Partner = textBox.Text;
                        textBoxTextList[textboxlist.IndexOf(textBox)] = textBox.Text;
                    }
                }
            }
        }

        public void RemovePerson(Node<Person> person, string name)
        {
            TextBox childTextBox = textboxlist.Find(i => i.Text == person.Value.Name);
            TextBox partnerTextBox = textboxlist.Find(i => i.Text == person.Value.Partner);

            Grid generationGrid = GenerationGridList.Find(i => i.Children.Contains(childTextBox));

            if(person.Value.Name == name)
            {
                if (partnerTextBox != null)
                {
                    foreach (TextBox textbox in textboxlist)
                    {
                        if (textboxlist.IndexOf(textbox) > textboxlist.IndexOf(partnerTextBox))
                        {
                            Grid.SetColumn(textbox, Grid.GetColumn(textbox) - 4);
                        }
                    }

                    foreach (Label label in birthDateLabelList)
                    {
                        if (birthDateLabelList.IndexOf(label) > textboxlist.IndexOf(partnerTextBox))
                        {
                            Grid.SetColumn(label, Grid.GetColumn(label) - 4);
                        }
                    }

                    foreach (Label label in deathDateLabelList)
                    {
                        if (deathDateLabelList.IndexOf(label) > textboxlist.IndexOf(partnerTextBox))
                        {
                            Grid.SetColumn(label, Grid.GetColumn(label) - 4);
                        }
                    }

                    foreach (TextBlock textBlock in textBlockList)
                    {
                        if (Grid.GetColumn(textBlock) > Grid.GetColumn(partnerTextBox) && Grid.GetColumn(textBlock) > 3)
                        {
                            Grid.SetColumn(textBlock, Grid.GetColumn(textBlock) - 4);
                        }
                    }

                    generationGrid.Children.Remove(birthDateLabelList[textboxlist.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(birthDateLabelList[textboxlist.IndexOf(partnerTextBox)]);
                    generationGrid.Children.Remove(deathDateLabelList[textboxlist.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(deathDateLabelList[textboxlist.IndexOf(partnerTextBox)]);
                    generationGrid.Children.Remove(birthDateTextBoxList[textboxlist.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(birthDateTextBoxList[textboxlist.IndexOf(partnerTextBox)]);
                    generationGrid.Children.Remove(deathDateTextBoxList[textboxlist.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(deathDateTextBoxList[textboxlist.IndexOf(partnerTextBox)]);

                    birthDateLabelList.Remove(birthDateLabelList[textboxlist.IndexOf(childTextBox)]);
                    birthDateLabelList.Remove(birthDateLabelList[textboxlist.IndexOf(partnerTextBox) - 1]);
                    deathDateLabelList.Remove(deathDateLabelList[textboxlist.IndexOf(childTextBox)]);
                    deathDateLabelList.Remove(deathDateLabelList[textboxlist.IndexOf(partnerTextBox) - 1]);
                    birthDateTextBoxList.Remove(birthDateTextBoxList[textboxlist.IndexOf(childTextBox)]);
                    birthDateTextBoxList.Remove(birthDateTextBoxList[textboxlist.IndexOf(partnerTextBox) - 1]);
                    deathDateTextBoxList.Remove(deathDateTextBoxList[textboxlist.IndexOf(childTextBox)]);
                    deathDateTextBoxList.Remove(deathDateTextBoxList[textboxlist.IndexOf(partnerTextBox) - 1]);

                    generationGrid.Children.Remove(textboxlist[textboxlist.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(textboxlist[textboxlist.IndexOf(partnerTextBox)]);
                    
                    textboxlist.Remove(childTextBox);
                    textboxlist.Remove(partnerTextBox);

                    generationGrid.ColumnDefinitions.Remove(generationGrid.ColumnDefinitions[generationGrid.ColumnDefinitions.Count - 1]);
                    generationGrid.ColumnDefinitions.Remove(generationGrid.ColumnDefinitions[generationGrid.ColumnDefinitions.Count - 1]);
                    generationGrid.ColumnDefinitions.Remove(generationGrid.ColumnDefinitions[generationGrid.ColumnDefinitions.Count - 1]);
                }
                else
                {
                    foreach (TextBox textbox in textboxlist)
                    {
                        if (textboxlist.IndexOf(textbox) > textboxlist.IndexOf(childTextBox))
                        {
                            Grid.SetColumn(textbox, Grid.GetColumn(textbox) - 2);
                        }
                    }

                    foreach (Label label in birthDateLabelList)
                    {
                        if (birthDateLabelList.IndexOf(label) > textboxlist.IndexOf(childTextBox))
                        {
                            Grid.SetColumn(label, Grid.GetColumn(label) - 2);
                        }
                    }

                    foreach (Label label in deathDateLabelList)
                    {
                        if (deathDateLabelList.IndexOf(label) > textboxlist.IndexOf(childTextBox))
                        {
                            Grid.SetColumn(label, Grid.GetColumn(label) - 2);
                        }
                    }

                    foreach (TextBlock textBlock in textBlockList)
                    {
                        if (Grid.GetColumn(textBlock) > Grid.GetColumn(childTextBox))
                        {
                            Grid.SetColumn(textBlock, Grid.GetColumn(textBlock) - 2);
                        }
                    }

                    generationGrid.Children.Remove(birthDateLabelList[textboxlist.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(deathDateLabelList[textboxlist.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(birthDateTextBoxList[textboxlist.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(deathDateTextBoxList[textboxlist.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(textboxlist[textboxlist.IndexOf(childTextBox)]);

                    birthDateLabelList.Remove(birthDateLabelList[textboxlist.IndexOf(childTextBox)]);
                    deathDateLabelList.Remove(deathDateLabelList[textboxlist.IndexOf(childTextBox)]);
                    birthDateTextBoxList.Remove(birthDateTextBoxList[textboxlist.IndexOf(childTextBox)]);
                    deathDateTextBoxList.Remove(deathDateTextBoxList[textboxlist.IndexOf(childTextBox)]);
                    textboxlist.Remove(childTextBox);

                    generationGrid.ColumnDefinitions.Remove(generationGrid.ColumnDefinitions[generationGrid.ColumnDefinitions.Count - 1]);
                    generationGrid.ColumnDefinitions.Remove(generationGrid.ColumnDefinitions[generationGrid.ColumnDefinitions.Count - 1]);
                }
            }
            else
            {
                foreach (TextBox textbox in textboxlist)
                {
                    if (textboxlist.IndexOf(textbox) > textboxlist.IndexOf(partnerTextBox))
                    {
                        Grid.SetColumn(textbox, Grid.GetColumn(textbox) - 2);
                    }
                }

                foreach (Label label in birthDateLabelList)
                {
                    if (birthDateLabelList.IndexOf(label) > textboxlist.IndexOf(partnerTextBox))
                    {
                        Grid.SetColumn(label, Grid.GetColumn(label) - 2);
                    }
                }

                foreach (Label label in deathDateLabelList)
                {
                    if (deathDateLabelList.IndexOf(label) > textboxlist.IndexOf(partnerTextBox))
                    {
                        Grid.SetColumn(label, Grid.GetColumn(label) - 2);
                    }
                }

                foreach (TextBlock textBlock in textBlockList)
                {
                    if (Grid.GetColumn(textBlock) > Grid.GetColumn(partnerTextBox))
                    {
                        Grid.SetColumn(textBlock, Grid.GetColumn(textBlock) - 2);
                    }
                }

                generationGrid.Children.Remove(birthDateLabelList[textboxlist.IndexOf(partnerTextBox)]);
                generationGrid.Children.Remove(deathDateLabelList[textboxlist.IndexOf(partnerTextBox)]);
                generationGrid.Children.Remove(birthDateTextBoxList[textboxlist.IndexOf(partnerTextBox)]);
                generationGrid.Children.Remove(deathDateTextBoxList[textboxlist.IndexOf(partnerTextBox)]);

                birthDateLabelList.Remove(birthDateLabelList[textboxlist.IndexOf(partnerTextBox)]);
                deathDateLabelList.Remove(deathDateLabelList[textboxlist.IndexOf(partnerTextBox)]);
                birthDateTextBoxList.Remove(birthDateTextBoxList[textboxlist.IndexOf(partnerTextBox)]);
                deathDateTextBoxList.Remove(deathDateTextBoxList[textboxlist.IndexOf(partnerTextBox)]);

                generationGrid.Children.Remove(textboxlist[textboxlist.IndexOf(partnerTextBox)]);
                textboxlist.Remove(partnerTextBox);

                person.Value.Partner = string.Empty;
                person.Value.PartnerBirthDate = null;
                person.Value.PartnerDeathDate = null;

                generationGrid.ColumnDefinitions.Remove(generationGrid.ColumnDefinitions[generationGrid.ColumnDefinitions.Count - 1]);
                generationGrid.ColumnDefinitions.Remove(generationGrid.ColumnDefinitions[generationGrid.ColumnDefinitions.Count - 1]);
            }

            for (int i = 0; i < generationGrid.Children.Count; i++)
            {
                if (generationGrid.Children[i].GetType() == typeof(Line) && ((Line)generationGrid.Children[i]).Name ==
                    "Child" + GenerationID.ToString().Replace("-", string.Empty))
                {
                    generationGrid.Children.Remove(generationGrid.Children[i]);
                    i--;
                }
            }

            if(ParentsGridList != null)
            {
                int parentIndex = 0;
                bool found = false;
                for (int i = 0; i < ParentsGridList.Count && !found; i++)
                {
                    for (int j = 0; j < ParentsGridList[i].Children.Count && !found; j++)
                    {
                        if (ParentsGridList[i].Children[j].GetType() == typeof(TextBox))
                        {
                            if ((string)ParentsGridList[i].Children[j].GetValue(TextBox.TextProperty) == person.Parent.Value.Name)
                            {
                                parentIndex = i;
                                found = true;
                            }
                        }
                    }
                }

                for (int i = 0; i < ParentsGridList[parentIndex].Children.Count; i++)
                {
                    if (ParentsGridList[parentIndex].Children[i].GetType() == typeof(Line) &&
                        ((Line)ParentsGridList[parentIndex].Children[i]).Name ==
                        "Child" + GenerationID.ToString().Replace("-", string.Empty))
                    {
                        ParentsGridList[parentIndex].Children.Remove(ParentsGridList[parentIndex].Children[i]);
                        i--;
                    }
                }
            }

            if (!generationGrid.Children.OfType<TextBox>().Any())
            {
                GenerationGridList.Remove(generationGrid);
            }
        }
    }
}
