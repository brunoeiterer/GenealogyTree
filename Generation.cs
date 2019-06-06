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
        public List<TextBox> TextBoxList { get; set; }
        private List<string> textBoxTextList;
        private List<Label> birthDateLabelList;
        private List<Label> deathDateLabelList;
        private List<TextBox> birthDateTextBoxList;
        private List<TextBox> deathDateTextBoxList;
        private List<Label> birthPlaceLabelList;
        private List<TextBox> birthPlaceTextBoxList;
        private List<TextBlock> textBlockList;
        private List<string> nameList;
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

            TextBoxList = new List<TextBox>();
            textBoxTextList = new List<string>();
            birthDateLabelList = new List<Label>();
            deathDateLabelList = new List<Label>();
            textBlockList = new List<TextBlock>();
            birthDateTextBoxList = new List<TextBox>();
            deathDateTextBoxList = new List<TextBox>();
            birthPlaceLabelList = new List<Label>();
            birthPlaceTextBoxList = new List<TextBox>();
            nameList = new List<string>();

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
            GenerationGridList[generationGridIndex].Children.Add(TextBoxList[TextBoxList.Count - 1]);
            Grid.SetColumn(TextBoxList[TextBoxList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);

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
                GenerationGridList[generationGridIndex].Children.Add(TextBoxList[TextBoxList.Count - 1]);
                Grid.SetColumn(TextBoxList[TextBoxList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);
                if(person.Value.IsPartnerInFamily)
                {
                    TextBoxList[TextBoxList.Count - 1].Foreground = Brushes.Red;
                    TextBoxList.Where(t => t.Text == person.Value.Partner).First().Foreground = Brushes.Red;
                }

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
                if (person.Value.IsPartnerInFamily)
                {
                    birthDateLabelList[birthDateLabelList.Count - 1].Foreground = Brushes.Red;
                    birthDateTextBoxList[birthDateTextBoxList.Count - 1].Foreground = Brushes.Red;
                    birthDateLabelList[TextBoxList.IndexOf(TextBoxList.Where(t => t.Text == person.Value.Partner).First())].Foreground = 
                        Brushes.Red;
                    birthDateTextBoxList[TextBoxList.IndexOf(TextBoxList.Where(t => t.Text == person.Value.Partner).First())].Foreground =
                        Brushes.Red;
                }

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
                if (person.Value.IsPartnerInFamily)
                {
                    deathDateLabelList[deathDateLabelList.Count - 1].Foreground = Brushes.Red;
                    deathDateTextBoxList[deathDateTextBoxList.Count - 1].Foreground = Brushes.Red;
                    deathDateLabelList[TextBoxList.IndexOf(TextBoxList.Where(t => t.Text == person.Value.Partner).First())].Foreground = 
                        Brushes.Red;
                    deathDateTextBoxList[TextBoxList.IndexOf(TextBoxList.Where(t => t.Text == person.Value.Partner).First())].Foreground =
                        Brushes.Red;
                }

                GenerationGridList[generationGridIndex].Children.Add(deathDateLabelList[deathDateLabelList.Count - 1]);
                GenerationGridList[generationGridIndex].Children.Add(deathDateTextBoxList[deathDateTextBoxList.Count - 1]);

                AddBirthPlaceLabel(person.Value.PartnerBirthPlace);

                if (GenerationGridList[generationGridIndex].RowDefinitions.Count < 4)
                {
                    GenerationGridList[generationGridIndex].RowDefinitions.Add(new RowDefinition());
                }
                Grid.SetRow(birthPlaceLabelList[birthPlaceLabelList.Count - 1], 3);
                Grid.SetColumn(birthPlaceLabelList[birthPlaceLabelList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);
                Grid.SetRow(birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1], 3);
                Grid.SetColumn(birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1], GenerationGridList[generationGridIndex].ColumnDefinitions.Count - 1);
                if (person.Value.IsPartnerInFamily)
                {
                    birthPlaceLabelList[birthPlaceLabelList.Count - 1].Foreground = Brushes.Red;
                    birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1].Foreground = Brushes.Red;
                    birthPlaceLabelList[TextBoxList.IndexOf(TextBoxList.Where(t => t.Text == person.Value.Partner).First())].Foreground =
                        Brushes.Red;
                    birthPlaceTextBoxList[TextBoxList.IndexOf(TextBoxList.Where(t => t.Text == person.Value.Partner).First())].Foreground =
                        Brushes.Red;
                }

                GenerationGridList[generationGridIndex].Children.Add(birthPlaceLabelList[birthPlaceLabelList.Count - 1]);
                GenerationGridList[generationGridIndex].Children.Add(birthPlaceTextBoxList[birthPlaceTextBoxList.Count - 1]);
            }

            GenerationChangedEventArgs eventArgs = new GenerationChangedEventArgs();
            if(person.Value.IsPartnerInFamily)
            {
                eventArgs.duplicatedName = person.Value.Partner;
            }
            GenerationChanged?.Invoke(this, eventArgs);
        }

        public void AddPartner(string childName, string partnerName, Nullable<DateTime> birthDate, Nullable<DateTime> deathDate, string birthPlace)
        {
            TextBox childTextBox = null;
            foreach (TextBox textBox in TextBoxList)
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

            foreach (TextBox textbox in TextBoxList)
            {
                if(TextBoxList.IndexOf(textbox) > TextBoxList.IndexOf(childTextBox))
                {
                    Grid.SetColumn(textbox, Grid.GetColumn(textbox) + 2);
                }
            }

            foreach (Label label in birthDateLabelList)
            {
                if (birthDateLabelList.IndexOf(label) > TextBoxList.IndexOf(childTextBox))
                {
                    Grid.SetColumn(label, Grid.GetColumn(label) + 2);
                }
            }

            foreach (Label label in deathDateLabelList)
            {
                if (deathDateLabelList.IndexOf(label) > TextBoxList.IndexOf(childTextBox))
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

            TextBoxList.Insert(TextBoxList.IndexOf(childTextBox) + 1, new TextBox());
            textBoxTextList.Insert(TextBoxList.IndexOf(childTextBox) + 1, new String(partnerName.ToArray()));
            TextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Text = partnerName;
            TextBoxList[TextBoxList.IndexOf(childTextBox) + 1].HorizontalAlignment = HorizontalAlignment.Center;
            TextBoxList[TextBoxList.IndexOf(childTextBox) + 1].VerticalAlignment = VerticalAlignment.Center;
            TextBoxList[TextBoxList.IndexOf(childTextBox) + 1].HorizontalContentAlignment = HorizontalAlignment.Center;
            TextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Width = 250;
            TextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Name = "partner" + new String(partnerName.Where(ch => Char.IsLetterOrDigit(ch)).ToArray());
            TextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Margin = new Thickness(0, 0, 0, 0);
            TextBoxList[TextBoxList.IndexOf(childTextBox) + 1].LostFocus += NameChanged;
            GenerationGridList[gridIndex].Children.Add(TextBoxList[TextBoxList.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(TextBoxList[TextBoxList.IndexOf(childTextBox) + 1], childColumnIndex + 2);

            birthDateLabelList.Insert(TextBoxList.IndexOf(childTextBox) + 1, new Label());
            birthDateLabelList[birthDateLabelList.Count - 1].Margin = new Thickness(0, 0, 25, 0);
            birthDateLabelList[TextBoxList.IndexOf(childTextBox) + 1].Content = "☆";
            birthDateLabelList[TextBoxList.IndexOf(childTextBox) + 1].BorderBrush = Brushes.Transparent;
            GenerationGridList[gridIndex].Children.Add(birthDateLabelList[TextBoxList.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(birthDateLabelList[TextBoxList.IndexOf(childTextBox) + 1], childColumnIndex + 2);
            Grid.SetRow(birthDateLabelList[TextBoxList.IndexOf(childTextBox) + 1], 1);

            deathDateLabelList.Insert(TextBoxList.IndexOf(childTextBox) + 1, new Label());
            deathDateLabelList[deathDateLabelList.Count - 1].Margin = new Thickness(0, 0, 25, 0);
            deathDateLabelList[TextBoxList.IndexOf(childTextBox) + 1].Content = "✞";
            deathDateLabelList[TextBoxList.IndexOf(childTextBox) + 1].BorderBrush = Brushes.Transparent;

            GenerationGridList[gridIndex].Children.Add(deathDateLabelList[TextBoxList.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(deathDateLabelList[TextBoxList.IndexOf(childTextBox) + 1], childColumnIndex + 2);
            Grid.SetRow(deathDateLabelList[TextBoxList.IndexOf(childTextBox) + 1], 2);

            birthPlaceLabelList.Insert(TextBoxList.IndexOf(childTextBox) + 1, new Label());
            birthPlaceLabelList[birthPlaceLabelList.Count - 1].Margin = new Thickness(0, 0, 25, 25);
            birthPlaceLabelList[birthPlaceLabelList.Count - 1].Content = "📌";
            birthPlaceLabelList[birthPlaceLabelList.Count - 1].BorderBrush = Brushes.Transparent;
            GenerationGridList[gridIndex].Children.Add(birthPlaceLabelList[TextBoxList.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(birthPlaceLabelList[TextBoxList.IndexOf(childTextBox) + 1], childColumnIndex + 2);
            Grid.SetRow(birthPlaceLabelList[TextBoxList.IndexOf(childTextBox) + 1], 3);

            birthDateTextBoxList.Insert(TextBoxList.IndexOf(childTextBox) + 1, new TextBox());
            if (birthDate != null)
            {
                birthDateTextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Text = birthDate.Value.ToShortDateString();
            }
            else
            {
                birthDateTextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Text = string.Empty;
            }
            birthDateTextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Width = 200;
            birthDateTextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Height = 20;
            GenerationGridList[gridIndex].Children.Add(birthDateTextBoxList[TextBoxList.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(birthDateTextBoxList[TextBoxList.IndexOf(childTextBox) + 1], childColumnIndex + 2);
            Grid.SetRow(birthDateTextBoxList[TextBoxList.IndexOf(childTextBox) + 1], 1);

            deathDateTextBoxList.Insert(TextBoxList.IndexOf(childTextBox) + 1, new TextBox());
            if (birthDate != null)
            {
                deathDateTextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Text = birthDate.Value.ToShortDateString();
            }
            else
            {
                deathDateTextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Text = string.Empty;
            }
            deathDateTextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Width = 200;
            deathDateTextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Height = 20;
            GenerationGridList[gridIndex].Children.Add(deathDateTextBoxList[TextBoxList.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(deathDateTextBoxList[TextBoxList.IndexOf(childTextBox) + 1], childColumnIndex + 2);
            Grid.SetRow(deathDateTextBoxList[TextBoxList.IndexOf(childTextBox) + 1], 2);

            birthPlaceTextBoxList.Insert(TextBoxList.IndexOf(childTextBox) + 1, new TextBox());
            birthPlaceTextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Text = birthPlace;
            birthPlaceTextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Margin = new Thickness(0, 0, 0, 50);
            birthPlaceTextBoxList[TextBoxList.IndexOf(childTextBox) + 1].LostFocus += BirthPlaceChanged;
            birthPlaceTextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Width = 200;
            birthPlaceTextBoxList[TextBoxList.IndexOf(childTextBox) + 1].Height = 20;
            GenerationGridList[gridIndex].Children.Add(birthPlaceTextBoxList[TextBoxList.IndexOf(childTextBox) + 1]);
            Grid.SetColumn(birthPlaceTextBoxList[TextBoxList.IndexOf(childTextBox) + 1], childColumnIndex + 2);
            Grid.SetRow(birthPlaceTextBoxList[TextBoxList.IndexOf(childTextBox) + 1], 3);

            GenerationChangedEventArgs eventArgs = new GenerationChangedEventArgs();
            GenerationChanged?.Invoke(this, eventArgs);
        }

        private void BirthPlaceChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Node<Person> person = null;
            if (TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Name.Substring(0, 5) == "child" &&
                birthPlaceTextBoxList.IndexOf(textBox) != birthPlaceTextBoxList.Count - 1)
            {
                if (PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Text,
                TextBoxList[birthPlaceTextBoxList.IndexOf(textBox) + 1].Text) != null)
                {
                    person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Text,
                        TextBoxList[birthPlaceTextBoxList.IndexOf(textBox) + 1].Text);
                }
                else
                {
                    person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Text, string.Empty);
                }
            }
            else if(TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Name.Substring(0, 7) == "partner")
            {
                person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Text,
                    TextBoxList[birthPlaceTextBoxList.IndexOf(textBox) - 1].Text);
            }
            else
            {
                person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Text, string.Empty);
            }

            if(person.Value.Name == TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Text)
            {
                person.Value.BirthPlace = textBox.Text;
            }
            else if(person.Value.Partner == TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Text)
            {
                person.Value.PartnerBirthPlace = textBox.Text;
            }
        }

        public event GenerationChangedEventHandler GenerationChanged;

        private void AddTextBox(string value, string type)
        {
            TextBoxList.Add(new TextBox());
            TextBoxList[TextBoxList.Count - 1].Text = value;
            TextBoxList[TextBoxList.Count - 1].HorizontalAlignment = HorizontalAlignment.Center;
            TextBoxList[TextBoxList.Count - 1].VerticalAlignment = VerticalAlignment.Center;
            TextBoxList[TextBoxList.Count - 1].HorizontalContentAlignment = HorizontalAlignment.Center;
            TextBoxList[TextBoxList.Count - 1].Width = 250;
            TextBoxList[TextBoxList.Count - 1].Name = type + new String(value.Where(ch => Char.IsLetterOrDigit(ch)).ToArray());
            TextBoxList[TextBoxList.Count - 1].Margin = new Thickness(0, 0, 0, 0);
            TextBoxList[TextBoxList.Count - 1].LostFocus += NameChanged;

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

            if (TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Name.Substring(0, 5) == "child" &&
                    birthPlaceTextBoxList.IndexOf(textBox) != birthPlaceTextBoxList.Count - 1)
            {
                if (PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Text,
                        TextBoxList[birthPlaceTextBoxList.IndexOf(textBox) + 1].Text) != null)
                {
                    person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Text,
                        TextBoxList[birthPlaceTextBoxList.IndexOf(textBox) + 1].Text);
                }
                else
                {
                    person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Text, string.Empty);
                }
            }
            else if(TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Name.Substring(0, 7) == "partner")
            {
                person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Text,
                            TextBoxList[birthPlaceTextBoxList.IndexOf(textBox) - 1].Text);
            }
            else
            {
                person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Text, string.Empty);
            }
            
            if(person.Value.Name == TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Text)
            {
                person.Value.BirthPlace = textBox.Text;
            }
            else if(person.Value.Partner == TextBoxList[birthPlaceTextBoxList.IndexOf(textBox)].Text)
            {
                person.Value.PartnerBirthPlace = textBox.Text;
            }
        }

        private void ChangeBirthDate(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Node<Person> person = null;
            string name;

            if(TextBoxList[birthDateTextBoxList.IndexOf(textBox)].Name.Substring(0, 5) == "child" && 
                birthDateTextBoxList.IndexOf(textBox) != birthDateTextBoxList.Count - 1)
            {
                if (PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthDateTextBoxList.IndexOf(textBox)].Text,
                        TextBoxList[birthDateTextBoxList.IndexOf(textBox) + 1].Text) != null)
                {
                    person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthDateTextBoxList.IndexOf(textBox)].Text,
                        TextBoxList[birthDateTextBoxList.IndexOf(textBox) + 1].Text);
                }
                else
                {
                    person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthDateTextBoxList.IndexOf(textBox)].Text, string.Empty);
                }
            }
            else if(TextBoxList[birthDateTextBoxList.IndexOf(textBox)].Name.Substring(0, 7) == "partner")
            {
                person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthDateTextBoxList.IndexOf(textBox)].Text,
                    TextBoxList[birthDateTextBoxList.IndexOf(textBox) - 1].Text);

            }
            else
            {
                person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[birthDateTextBoxList.IndexOf(textBox)].Text, string.Empty);
            }
            name = TextBoxList[birthDateTextBoxList.IndexOf(textBox)].Text;

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

            if(TextBoxList[deathDateTextBoxList.IndexOf(textBox)].Name.Substring(0, 5) == "child" &&
                deathDateTextBoxList.IndexOf(textBox) != deathDateTextBoxList.Count - 1)
            {
                if (PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[deathDateTextBoxList.IndexOf(textBox)].Text,
                        TextBoxList[deathDateTextBoxList.IndexOf(textBox) + 1].Text) != null)
                {
                    person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[deathDateTextBoxList.IndexOf(textBox)].Text,
                        TextBoxList[deathDateTextBoxList.IndexOf(textBox) + 1].Text);
                }
                else
                {
                    person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[deathDateTextBoxList.IndexOf(textBox)].Text, string.Empty);
                }
            }
            else if(TextBoxList[deathDateTextBoxList.IndexOf(textBox)].Name.Substring(0, 7) == "partner")
            {
                person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[deathDateTextBoxList.IndexOf(textBox)].Text,
                    TextBoxList[deathDateTextBoxList.IndexOf(textBox) - 1].Text);
            }
            else
            {
                person = PersonTree.GetNodeByName(PersonTree.Tree, TextBoxList[deathDateTextBoxList.IndexOf(textBox)].Text, string.Empty);
            }

            name = TextBoxList[deathDateTextBoxList.IndexOf(textBox)].Text;

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
                if (textBox.Text != textBoxTextList[TextBoxList.IndexOf(textBox)])
                {
                    Action<Person, Node<Person>> addToNameListDelegate = AddToNameList;
                    PersonTree.Tree.Traverse(PersonTree.Tree, addToNameListDelegate);
                    if (nameList.Contains(textBox.Text))
                    {
                        MessageBox.Show(Application.Current.Resources["NameChangeDuplicatedNameError"].ToString(),
                            Application.Current.Resources["NameChangeDuplicatedNameErrorMessageBoxTitle"].ToString());

                        textBox.Text = textBoxTextList[TextBoxList.IndexOf(textBox)];
                    }
                    else
                    {
                        if(PersonTree.GetNodeByName(PersonTree.Tree, textBoxTextList[TextBoxList.IndexOf(textBox)],
                            textBoxTextList[TextBoxList.IndexOf(textBox) + 1]) != null)
                        {
                            person = PersonTree.GetNodeByName(PersonTree.Tree, textBoxTextList[TextBoxList.IndexOf(textBox)],
                                textBoxTextList[TextBoxList.IndexOf(textBox) + 1]);
                        }
                        else
                        {
                            person = PersonTree.GetNodeByName(PersonTree.Tree, textBoxTextList[TextBoxList.IndexOf(textBox)], string.Empty);
                        }
                        
                        textBox.Name = "child" + new String(textBox.Text.Where(ch => Char.IsLetterOrDigit(ch)).ToArray());
                        person.Value.Name = textBox.Text;
                        textBoxTextList[TextBoxList.IndexOf(textBox)] = textBox.Text;
                    }
                }
            }
            else if(textBox.Name.Substring(0, 7) == "partner")
            {
                if (textBox.Text != textBoxTextList[TextBoxList.IndexOf(textBox)])
                {
                    Action<Person, Node<Person>> addToNameListDelegate = AddToNameList;
                    PersonTree.Tree.Traverse(PersonTree.Tree, addToNameListDelegate);
                    if (nameList.Contains(textBox.Text))
                    {
                        MessageBox.Show(Application.Current.Resources["NameChangeDuplicatedNameError"].ToString(),
                            Application.Current.Resources["NameChangeDuplicatedNameErrorMessageBoxTitle"].ToString());

                        textBox.Text = textBoxTextList[TextBoxList.IndexOf(textBox)];
                    }
                    else
                    {
                        if(PersonTree.GetNodeByName(PersonTree.Tree, textBoxTextList[TextBoxList.IndexOf(textBox)],
                            textBoxTextList[TextBoxList.IndexOf(textBox) + 1]) != null)
                        {
                            person = PersonTree.GetNodeByName(PersonTree.Tree, textBoxTextList[TextBoxList.IndexOf(textBox)],
                                textBoxTextList[TextBoxList.IndexOf(textBox) + 1]);
                        }
                        else
                        {
                            person = PersonTree.GetNodeByName(PersonTree.Tree, textBoxTextList[TextBoxList.IndexOf(textBox)], string.Empty);
                        }
                       
                        textBox.Name = "partner" + new String(textBox.Text.Where(ch => Char.IsLetterOrDigit(ch)).ToArray());
                        person.Value.Partner = textBox.Text;
                        textBoxTextList[TextBoxList.IndexOf(textBox)] = textBox.Text;
                    }
                }
            }
        }

        private void AddToNameList(Person person, Node<Person> node)
        {
            if(!nameList.Contains(node.Value.Name))
            {
                nameList.Add(node.Value.Name);
            }
            if (!nameList.Contains(node.Value.Partner))
            {
                nameList.Add(node.Value.Partner);
            }
        }

        public void RemovePerson(Node<Person> person, string name)
        {
            TextBox childTextBox = TextBoxList.Find(i => i.Text == person.Value.Name);
            TextBox partnerTextBox = TextBoxList.Find(i => i.Text == person.Value.Partner);

            Grid generationGrid = GenerationGridList.Find(i => i.Children.Contains(childTextBox));

            if(person.Value.Name == name)
            {
                if (partnerTextBox != null)
                {
                    foreach (TextBox textbox in TextBoxList)
                    {
                        if (TextBoxList.IndexOf(textbox) > TextBoxList.IndexOf(partnerTextBox))
                        {
                            Grid.SetColumn(textbox, Grid.GetColumn(textbox) - 4);
                        }
                    }

                    foreach (Label label in birthDateLabelList)
                    {
                        if (birthDateLabelList.IndexOf(label) > TextBoxList.IndexOf(partnerTextBox))
                        {
                            Grid.SetColumn(label, Grid.GetColumn(label) - 4);
                        }
                    }
                    
                    foreach(TextBox textBox in birthDateTextBoxList)
                    {
                        if (birthDateTextBoxList.IndexOf(textBox) > TextBoxList.IndexOf(partnerTextBox))
                        {
                            Grid.SetColumn(textBox, Grid.GetColumn(textBox) - 4);
                        }
                    }

                    foreach (Label label in deathDateLabelList)
                    {
                        if (deathDateLabelList.IndexOf(label) > TextBoxList.IndexOf(partnerTextBox))
                        {
                            Grid.SetColumn(label, Grid.GetColumn(label) - 4);
                        }
                    }

                    foreach (TextBox textBox in deathDateTextBoxList)
                    {
                        if (deathDateTextBoxList.IndexOf(textBox) > TextBoxList.IndexOf(partnerTextBox))
                        {
                            Grid.SetColumn(textBox, Grid.GetColumn(textBox) - 4);
                        }
                    }

                    foreach (Label label in birthPlaceLabelList)
                    {
                        if (birthPlaceLabelList.IndexOf(label) > TextBoxList.IndexOf(partnerTextBox))
                        {
                            Grid.SetColumn(label, Grid.GetColumn(label) - 4);
                        }
                    }

                    foreach (TextBox textBox in birthPlaceTextBoxList)
                    {
                        if (birthPlaceTextBoxList.IndexOf(textBox) > TextBoxList.IndexOf(partnerTextBox))
                        {
                            Grid.SetColumn(textBox, Grid.GetColumn(textBox) - 4);
                        }
                    }

                    foreach (TextBlock textBlock in textBlockList)
                    {
                        if (Grid.GetColumn(textBlock) > Grid.GetColumn(partnerTextBox) && Grid.GetColumn(textBlock) > 3)
                        {
                            Grid.SetColumn(textBlock, Grid.GetColumn(textBlock) - 4);
                        }
                    }

                    generationGrid.Children.Remove(birthDateLabelList[TextBoxList.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(birthDateLabelList[TextBoxList.IndexOf(partnerTextBox)]);
                    generationGrid.Children.Remove(deathDateLabelList[TextBoxList.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(deathDateLabelList[TextBoxList.IndexOf(partnerTextBox)]);
                    generationGrid.Children.Remove(birthDateTextBoxList[TextBoxList.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(birthDateTextBoxList[TextBoxList.IndexOf(partnerTextBox)]);
                    generationGrid.Children.Remove(deathDateTextBoxList[TextBoxList.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(deathDateTextBoxList[TextBoxList.IndexOf(partnerTextBox)]);
                    generationGrid.Children.Remove(birthPlaceLabelList[TextBoxList.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(birthPlaceLabelList[TextBoxList.IndexOf(partnerTextBox)]);
                    generationGrid.Children.Remove(birthPlaceTextBoxList[TextBoxList.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(birthPlaceTextBoxList[TextBoxList.IndexOf(partnerTextBox)]);

                    birthDateLabelList.Remove(birthDateLabelList[TextBoxList.IndexOf(childTextBox)]);
                    birthDateLabelList.Remove(birthDateLabelList[TextBoxList.IndexOf(partnerTextBox) - 1]);
                    deathDateLabelList.Remove(deathDateLabelList[TextBoxList.IndexOf(childTextBox)]);
                    deathDateLabelList.Remove(deathDateLabelList[TextBoxList.IndexOf(partnerTextBox) - 1]);
                    birthPlaceLabelList.Remove(birthPlaceLabelList[TextBoxList.IndexOf(childTextBox)]);
                    birthPlaceLabelList.Remove(birthPlaceLabelList[TextBoxList.IndexOf(partnerTextBox) - 1]);
                    birthDateTextBoxList.Remove(birthDateTextBoxList[TextBoxList.IndexOf(childTextBox)]);
                    birthDateTextBoxList.Remove(birthDateTextBoxList[TextBoxList.IndexOf(partnerTextBox) - 1]);
                    deathDateTextBoxList.Remove(deathDateTextBoxList[TextBoxList.IndexOf(childTextBox)]);
                    deathDateTextBoxList.Remove(deathDateTextBoxList[TextBoxList.IndexOf(partnerTextBox) - 1]);
                    birthPlaceTextBoxList.Remove(birthPlaceTextBoxList[TextBoxList.IndexOf(childTextBox)]);
                    birthPlaceTextBoxList.Remove(birthPlaceTextBoxList[TextBoxList.IndexOf(partnerTextBox) - 1]);

                    generationGrid.Children.Remove(TextBoxList[TextBoxList.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(TextBoxList[TextBoxList.IndexOf(partnerTextBox)]);
                    
                    TextBoxList.Remove(childTextBox);
                    TextBoxList.Remove(partnerTextBox);

                    generationGrid.ColumnDefinitions.Remove(generationGrid.ColumnDefinitions[generationGrid.ColumnDefinitions.Count - 1]);
                    generationGrid.ColumnDefinitions.Remove(generationGrid.ColumnDefinitions[generationGrid.ColumnDefinitions.Count - 1]);
                    generationGrid.ColumnDefinitions.Remove(generationGrid.ColumnDefinitions[generationGrid.ColumnDefinitions.Count - 1]);
                    if(generationGrid.ColumnDefinitions.Count > 0)
                    {
                        generationGrid.ColumnDefinitions.Remove(generationGrid.ColumnDefinitions[generationGrid.ColumnDefinitions.Count - 1]);
                    }

                }
                else
                {
                    foreach (TextBox textbox in TextBoxList)
                    {
                        if (TextBoxList.IndexOf(textbox) > TextBoxList.IndexOf(childTextBox))
                        {
                            Grid.SetColumn(textbox, Grid.GetColumn(textbox) - 2);
                        }
                    }

                    foreach (Label label in birthDateLabelList)
                    {
                        if (birthDateLabelList.IndexOf(label) > TextBoxList.IndexOf(childTextBox))
                        {
                            Grid.SetColumn(label, Grid.GetColumn(label) - 2);
                        }
                    }

                    foreach (TextBox textBox in birthDateTextBoxList)
                    {
                        if (birthDateTextBoxList.IndexOf(textBox) > TextBoxList.IndexOf(childTextBox))
                        {
                            Grid.SetColumn(textBox, Grid.GetColumn(textBox) - 2);
                        }
                    }

                    foreach (Label label in deathDateLabelList)
                    {
                        if (deathDateLabelList.IndexOf(label) > TextBoxList.IndexOf(childTextBox))
                        {
                            Grid.SetColumn(label, Grid.GetColumn(label) - 2);
                        }
                    }

                    foreach (TextBox textBox in deathDateTextBoxList)
                    {
                        if (deathDateTextBoxList.IndexOf(textBox) > TextBoxList.IndexOf(childTextBox))
                        {
                            Grid.SetColumn(textBox, Grid.GetColumn(textBox) - 2);
                        }
                    }

                    foreach (Label label in birthPlaceLabelList)
                    {
                        if (birthPlaceLabelList.IndexOf(label) > TextBoxList.IndexOf(childTextBox))
                        {
                            Grid.SetColumn(label, Grid.GetColumn(label) - 2);
                        }
                    }

                    foreach (TextBox textBox in birthPlaceTextBoxList)
                    {
                        if (birthPlaceTextBoxList.IndexOf(textBox) > TextBoxList.IndexOf(childTextBox))
                        {
                            Grid.SetColumn(textBox, Grid.GetColumn(textBox) - 2);
                        }
                    }

                    foreach (TextBlock textBlock in textBlockList)
                    {
                        if(Grid.GetColumn(textBlock) > 1)
                        {
                            if (Grid.GetColumn(textBlock) > Grid.GetColumn(childTextBox))
                            {
                                Grid.SetColumn(textBlock, Grid.GetColumn(textBlock) - 2);
                            }
                        }
                    }

                    generationGrid.Children.Remove(birthDateLabelList[TextBoxList.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(deathDateLabelList[TextBoxList.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(birthDateTextBoxList[TextBoxList.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(deathDateTextBoxList[TextBoxList.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(TextBoxList[TextBoxList.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(birthPlaceLabelList[TextBoxList.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(birthPlaceTextBoxList[TextBoxList.IndexOf(childTextBox)]);
                    generationGrid.Children.Remove(textBlockList[TextBoxList.IndexOf(childTextBox)]);

                    birthDateLabelList.Remove(birthDateLabelList[TextBoxList.IndexOf(childTextBox)]);
                    deathDateLabelList.Remove(deathDateLabelList[TextBoxList.IndexOf(childTextBox)]);
                    birthDateTextBoxList.Remove(birthDateTextBoxList[TextBoxList.IndexOf(childTextBox)]);
                    deathDateTextBoxList.Remove(deathDateTextBoxList[TextBoxList.IndexOf(childTextBox)]);
                    birthPlaceLabelList.Remove(birthPlaceLabelList[TextBoxList.IndexOf(childTextBox)]);
                    birthPlaceTextBoxList.Remove(birthPlaceTextBoxList[TextBoxList.IndexOf(childTextBox)]);
                    textBlockList.Remove(textBlockList[TextBoxList.IndexOf(childTextBox)]);
                    TextBoxList.Remove(childTextBox);

                    generationGrid.ColumnDefinitions.Remove(generationGrid.ColumnDefinitions[generationGrid.ColumnDefinitions.Count - 1]);
                    generationGrid.ColumnDefinitions.Remove(generationGrid.ColumnDefinitions[generationGrid.ColumnDefinitions.Count - 1]);
                }
            }
            else
            {
                foreach (TextBox textbox in TextBoxList)
                {
                    if (TextBoxList.IndexOf(textbox) > TextBoxList.IndexOf(partnerTextBox))
                    {
                        Grid.SetColumn(textbox, Grid.GetColumn(textbox) - 2);
                    }
                }

                foreach (Label label in birthDateLabelList)
                {
                    if (birthDateLabelList.IndexOf(label) > TextBoxList.IndexOf(partnerTextBox))
                    {
                        Grid.SetColumn(label, Grid.GetColumn(label) - 2);
                    }
                }

                foreach (Label label in deathDateLabelList)
                {
                    if (deathDateLabelList.IndexOf(label) > TextBoxList.IndexOf(partnerTextBox))
                    {
                        Grid.SetColumn(label, Grid.GetColumn(label) - 2);
                    }
                }

                foreach (Label label in birthPlaceLabelList)
                {
                    if (birthPlaceLabelList.IndexOf(label) > TextBoxList.IndexOf(childTextBox))
                    {
                        Grid.SetColumn(label, Grid.GetColumn(label) - 2);
                    }
                }

                foreach (TextBox textBox in birthPlaceTextBoxList)
                {
                    if (birthPlaceTextBoxList.IndexOf(textBox) > TextBoxList.IndexOf(childTextBox))
                    {
                        Grid.SetColumn(textBox, Grid.GetColumn(textBox) - 2);
                    }
                }

                foreach (TextBlock textBlock in textBlockList)
                {
                    if (Grid.GetColumn(textBlock) > Grid.GetColumn(partnerTextBox))
                    {
                        Grid.SetColumn(textBlock, Grid.GetColumn(textBlock) - 2);
                    }
                }

                generationGrid.Children.Remove(birthDateLabelList[TextBoxList.IndexOf(partnerTextBox)]);
                generationGrid.Children.Remove(deathDateLabelList[TextBoxList.IndexOf(partnerTextBox)]);
                generationGrid.Children.Remove(birthDateTextBoxList[TextBoxList.IndexOf(partnerTextBox)]);
                generationGrid.Children.Remove(deathDateTextBoxList[TextBoxList.IndexOf(partnerTextBox)]);
                generationGrid.Children.Remove(birthPlaceLabelList[TextBoxList.IndexOf(partnerTextBox)]);
                generationGrid.Children.Remove(birthPlaceTextBoxList[TextBoxList.IndexOf(partnerTextBox)]);

                birthDateLabelList.Remove(birthDateLabelList[TextBoxList.IndexOf(partnerTextBox)]);
                deathDateLabelList.Remove(deathDateLabelList[TextBoxList.IndexOf(partnerTextBox)]);
                birthDateTextBoxList.Remove(birthDateTextBoxList[TextBoxList.IndexOf(partnerTextBox)]);
                deathDateTextBoxList.Remove(deathDateTextBoxList[TextBoxList.IndexOf(partnerTextBox)]);
                birthPlaceLabelList.Remove(birthPlaceLabelList[TextBoxList.IndexOf(partnerTextBox)]);
                birthPlaceTextBoxList.Remove(birthPlaceTextBoxList[TextBoxList.IndexOf(partnerTextBox)]);

                generationGrid.Children.Remove(TextBoxList[TextBoxList.IndexOf(partnerTextBox)]);
                TextBoxList.Remove(partnerTextBox);

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
