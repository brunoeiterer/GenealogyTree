﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;

namespace GenealogyTree
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Grid ConnectionsGrid { get; set; }
        private DockPanel basePanel;
        private Menu menu;
        private Grid treeGrid;
        private GenerationManager generationManager;

        public MainWindow()
        {
            InitializeComponent();

            generationManager = new GenerationManager();
            generationManager.GenerationChanged += ConnectChildrenToParents;
            generationManager.NewGenerationAdded += AddNewGenerationToTreePanel;
            generationManager.OpenRequestedEvent += ClearGrid;
            generationManager.OpenCompletedEvent += OpenCompletedHandler;
            generationManager.NewGenerationInserted += InsertNewGenerationIntoTreePanel;

            menu = new Menu();
            menu.PartnerAdded += PartnerAdded;
            menu.SaveRequested += generationManager.Save;
            menu.OpenRequested += generationManager.Open;
            menu.FirstChildAddedEvent += generationManager.FirstChildAdded;
            menu.ChildRemovedEvent += ChildRemoved;
            menu.ParentsAddedEvent += ParentsAdded;

            PersonTree.NewChildAddedEvent += NewChildAdded;

            treeGrid = new Grid();
            treeGrid.ColumnDefinitions.Add(new ColumnDefinition());
            treeGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            treeGrid.RowDefinitions.Add(new RowDefinition());
            treeGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
            DockPanel.SetDock(treeGrid, Dock.Top);

            ConnectionsGrid = new Grid();
            DockPanel.SetDock(ConnectionsGrid, Dock.Top);
            ConnectionsGrid.Children.Add(treeGrid);

            Binding panelWidthBinding = new Binding()
            {
                Source = Application.Current.MainWindow,
                Path = new PropertyPath(MainWindow.ActualWidthProperty)
            };

            basePanel = new DockPanel();
            basePanel.SetBinding(DockPanel.WidthProperty, panelWidthBinding);
            this.BaseGrid.Children.Add(menu.BasePanel);
            Grid.SetRow(menu.BasePanel, 0);

            this.MainWindowScrollViewer.Content = ConnectionsGrid;

            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;
        }

        public void AddNewGenerationToTreePanel(object sender, NewGenerationAddedEventArgs e)
        {
            treeGrid.Children.Add(e.generation.BaseGrid);
            treeGrid.RowDefinitions.Add(new RowDefinition());
            treeGrid.RowDefinitions[treeGrid.RowDefinitions.Count - 1].Height = new GridLength(1, GridUnitType.Auto);
            Grid.SetRow(e.generation.BaseGrid, treeGrid.RowDefinitions.Count - 1);

            Grid.SetColumn(e.generation.BaseGrid, 0);
        }

        public void InsertNewGenerationIntoTreePanel(object sender, NewGenerationAddedEventArgs e)
        {
            treeGrid.Children.Add(e.generation.BaseGrid);
            treeGrid.RowDefinitions.Add(new RowDefinition());
            treeGrid.RowDefinitions[treeGrid.RowDefinitions.Count - 1].Height = new GridLength(1, GridUnitType.Auto);

            Grid.SetRow(e.generation.BaseGrid, generationManager.generationList.IndexOf(e.generation));
            Grid.SetColumn(e.generation.BaseGrid, 0);
        }

        private void NewChildAdded(object sender, NewChildAddedEventArgs<Person> e)
        {
            generationManager.AddChild(PersonTree.GetNodeByName(PersonTree.Tree, e.child.Name, e.child.Partner));
        }

        private void PartnerAdded(object sender, PartnerAddedEventArgs e)
        {
            generationManager.AddPartner(e.childName, e.partnerName, e.birthDate, e.deathDate, e.birthPlace);
        }

        private void ConnectChildrenToParents(object sender, GenerationChangedEventArgs e)
        {
            Generation generation = (Generation)sender;
            List<Node<Person>> personList = new List<Node<Person>>();

            foreach(Grid grid in generation.GenerationGridList)
            {
                foreach (TextBox textBox in grid.Children.OfType<TextBox>())
                {
                    Node<Person> nextPerson = null;
                    if(generation.TextBoxList.IndexOf(textBox) != generation.TextBoxList.Count - 1)
                    {
                        if (PersonTree.GetNodeByName(PersonTree.Tree, textBox.Text,
                            generation.TextBoxList[generation.TextBoxList.IndexOf(textBox) + 1].Text) != null)
                        {
                            nextPerson = PersonTree.GetNodeByName(PersonTree.Tree, textBox.Text,
                                                generation.TextBoxList[generation.TextBoxList.IndexOf(textBox) + 1].Text);
                        }
                    }

                    else
                    {
                        nextPerson = PersonTree.GetNodeByName(PersonTree.Tree, textBox.Text, string.Empty);
                    }
                    if(nextPerson != null)
                    {
                        if (nextPerson.Value.GenerationID == generation.GenerationID)
                        {
                            if (!personList.Any(item => item.Value.Name == nextPerson.Value.Name))
                            {
                                personList.Add(nextPerson);
                            }
                        }
                    }
                }
            }

            for(int i = 0; i < ConnectionsGrid.Children.Count; i++)
            {
                if(ConnectionsGrid.Children[i].GetType() == typeof(Line) && 
                    ((Line)ConnectionsGrid.Children[i]).Name == "Child" + generation.GenerationID.ToString().Replace("-", string.Empty))
                {
                    ConnectionsGrid.Children.Remove(ConnectionsGrid.Children[i]);
                    i--;
                }
            }

            //for (int i = 0; i < generation.ParentsGridList[parentIndex].Children.Count - 1; i++)
            //{
            //    if (generation.ParentsGridList[parentIndex].Children[i].GetType() == typeof(Line) &&
            //        ((Line)generation.ParentsGridList[parentIndex].Children[i]).Name ==
            //        "Child" + generation.GenerationID.ToString().Replace("-", string.Empty))
            //    {
            //        generation.ParentsGridList[parentIndex].Children.Remove(generation.ParentsGridList[parentIndex].Children[i]);
            //        i--;
            //    }
            //}

            foreach (Node<Person> person in personList)
            {
                int parentIndex = 0;
                int parentColumnIndex = 0;
                bool found = false;
                for (int i = 0; i < generation.ParentsGridList.Count; i++)
                {
                    for (int j = 0; j < generation.ParentsGridList[i].Children.Count && !found; j++)
                    {
                        if (generation.ParentsGridList[i].Children[j].GetType() == typeof(TextBox))
                        {
                            if ((string)generation.ParentsGridList[i].Children[j].GetValue(TextBox.TextProperty) == person.Parent.Value.Name)
                            {
                                parentIndex = i;
                                parentColumnIndex = Grid.GetColumn(generation.ParentsGridList[i].Children[j]);
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
                    Stretch = Stretch.None,
                    Name = "Child" + generation.GenerationID.ToString().Replace("-", string.Empty)
                };
                generation.ParentsGridList[parentIndex].Children.Add(verticalLine1);
                Grid.SetRow(verticalLine1, 0);
                Grid.SetColumn(verticalLine1, parentColumnIndex + 1);

                Line verticalLine2 = new Line()
                {
                    Stroke = Brushes.Black,
                    Visibility = Visibility.Visible,
                    StrokeThickness = 1,
                    X1 = 12.5,
                    X2 = 12.5,
                    Y1 = 0,
                    Y2 = SystemFonts.MessageFontSize * 2 + 2,
                    Stretch = Stretch.None,
                    Name = "Child" + generation.GenerationID.ToString().Replace("-", string.Empty)
                };
                generation.ParentsGridList[parentIndex].Children.Add(verticalLine2);
                Grid.SetRow(verticalLine2, 1);
                Grid.SetColumn(verticalLine2, parentColumnIndex + 1);

                Line verticalLine3 = new Line()
                {
                    Stroke = Brushes.Black,
                    Visibility = Visibility.Visible,
                    StrokeThickness = 1,
                    X1 = 12.5,
                    X2 = 12.5,
                    Y1 = 0,
                    Y2 = SystemFonts.MessageFontSize * 3 + 8,
                    Stretch = Stretch.None,
                    Name = "Child" + generation.GenerationID.ToString().Replace("-", string.Empty)
                };
                generation.ParentsGridList[parentIndex].Children.Add(verticalLine3);
                Grid.SetRow(verticalLine3, 2);
                Grid.SetColumn(verticalLine3, parentColumnIndex + 1);

                Line verticalLine4 = new Line()
                {
                    Stroke = Brushes.Black,
                    Visibility = Visibility.Visible,
                    StrokeThickness = 1,
                    X1 = 12.5,
                    X2 = 12.5,
                    Y1 = 0,
                    Y2 = SystemFonts.MessageFontSize * 3 + 8,
                    Stretch = Stretch.None,
                    Name = "Child" + generation.GenerationID.ToString().Replace("-", string.Empty)
                };
                generation.ParentsGridList[parentIndex].Children.Add(verticalLine4);
                Grid.SetRow(verticalLine4, 4);
                Grid.SetColumn(verticalLine4, parentColumnIndex + 1);

                verticalLine4.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                verticalLine4.Arrange(new Rect(verticalLine3.DesiredSize));
                verticalLine4.UpdateLayout();

                Point endPoint = verticalLine4.TranslatePoint(new Point(verticalLine4.X2, verticalLine4.Y2), ConnectionsGrid);

                int generationGridIndex = 0;
                int textBoxColumnIndex = 0;
                foreach(Grid grid in generation.GenerationGridList)
                {
                    foreach (TextBox textBox in grid.Children.OfType<TextBox>())
                    {
                        if(textBox.Text == person.Value.Name)
                        {
                            generationGridIndex = generation.GenerationGridList.IndexOf(grid);
                            textBoxColumnIndex = Grid.GetColumn(textBox);
                        }
                    }
                }
                
                Line verticalLine5 = new Line()
                {
                    Stroke = Brushes.Black,
                    Visibility = Visibility.Visible,
                    StrokeThickness = 1,
                    X1 = 125,
                    X2 = 125,
                    Y1 = 4,
                    Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                    Stretch = Stretch.None,
                    Name = "Child" + generation.GenerationID.ToString().Replace("-", string.Empty)
                };
                generation.GenerationGridList[generationGridIndex].Children.Add(verticalLine5);
                Grid.SetRow(verticalLine5, 0);
                Grid.SetColumn(verticalLine5, textBoxColumnIndex);

                verticalLine5.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                verticalLine5.Arrange(new Rect(verticalLine5.DesiredSize));
                verticalLine5.UpdateLayout();

                Point startPoint = verticalLine5.TranslatePoint(new Point(verticalLine5.X2, verticalLine5.Y2), ConnectionsGrid);

                Line horizontalLine1 = new Line()
                {
                    Stroke = Brushes.Black,
                    Visibility = Visibility.Visible,
                    StrokeThickness = 1,
                    X1 = startPoint.X,
                    X2 = endPoint.X,
                    Y1 = startPoint.Y,
                    Y2 = endPoint.Y,
                    Stretch = Stretch.None,
                    Name = "Child" + generation.GenerationID.ToString().Replace("-", string.Empty)
                };
                ConnectionsGrid.Children.Add(horizontalLine1);

                if (person.Value.Partner != string.Empty)
                {
                    Line horizontalLine = new Line()
                    {
                        Stroke = Brushes.Black,
                        Visibility = Visibility.Visible,
                        StrokeThickness = 1,
                        X1 = 0,
                        X2 = 25,
                        Y1 = SystemFonts.MessageFontSize,
                        Y2 = SystemFonts.MessageFontSize,
                        Stretch = Stretch.None,
                        Name = "Child" + generation.GenerationID.ToString().Replace("-", string.Empty)
                    };
                    generation.GenerationGridList[generationGridIndex].Children.Add(horizontalLine);
                    Grid.SetRow(horizontalLine, 0);
                    Grid.SetColumn(horizontalLine, textBoxColumnIndex + 1);
                }
            }

            List<Node<Person>> parentsList = new List<Node<Person>>();

            foreach(Node<Person> person in personList)
            {
                if(!parentsList.Contains(person.Parent))
                {
                    parentsList.Add(person.Parent);
                }
            }

            foreach (Node<Person> parent in parentsList)
            {
                Node<Person> firstPerson = personList.FirstOrDefault(p => p.Parent == parent);
                Node<Person> lastPerson = personList.LastOrDefault(p => p.Parent == parent);

                Grid generationGrid = null;

                foreach(Grid grid in generation.GenerationGridList)
                {
                    foreach(TextBox textBox in grid.Children.OfType<TextBox>())
                    {
                        if(textBox.Text == firstPerson.Value.Name)
                        {
                            generationGrid = grid;
                        }
                    }
                }

                int numberOfPartners = 0;
                for(int i = personList.IndexOf(firstPerson); i < personList.IndexOf(lastPerson); i++)
                {
                    if(personList[i].Value.Partner != string.Empty)
                    {
                        numberOfPartners++;
                    }
                }
            }
        }

        private void MainWindowInstance_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            for(int i = 1; i < generationManager.generationList.Count; i++)
            {
                ConnectChildrenToParents((object)generationManager.generationList[i], new GenerationChangedEventArgs());
            }
        }

        private void OpenCompletedHandler(object sender, OpenRequestedEventArgs e)
        {
            for (int i = 1; i < generationManager.generationList.Count; i++)
            {
                ConnectChildrenToParents((object)generationManager.generationList[i], new GenerationChangedEventArgs());
            }
        }

        private void ClearGrid(object sender, OpenRequestedEventArgs e)
        {
            for(int i = 0; i < treeGrid.Children.Count; i++)
            {
                treeGrid.Children.Remove(treeGrid.Children[i]);
                i--;
            }

            for(int i = 1; i < ConnectionsGrid.Children.Count; i++)
            {
                ConnectionsGrid.Children.Remove(ConnectionsGrid.Children[i]);
            }
        }

        private void ChildRemoved(object sender, ChildRemovedEventArgs e)
        {
            RemoveChildren(e.person);
            generationManager.RemoveChild(e.person, e.name);

            for (int i = 1; i < generationManager.generationList.Count; i++)
            {
                ConnectChildrenToParents((object)generationManager.generationList[i], new GenerationChangedEventArgs());
            }

            if (generationManager.GetGenerationByID(e.person.Value.GenerationID).GenerationGridList.Count == 0)
            {
                generationManager.generationList.Remove(generationManager.GetGenerationByID(e.person.Value.GenerationID));
                treeGrid.RowDefinitions.Remove(treeGrid.RowDefinitions[treeGrid.RowDefinitions.Count - 1]);
            }

            if(e.person != PersonTree.Tree)
            {
                PersonTree.GetNodeByName(PersonTree.Tree, e.person.Parent.Value.Name, e.person.Parent.Value.Partner).Remove(e.person);
            }
            else
            {
                PersonTree.Tree = new Node<Person>(new Person(), null);
                PersonTree.Tree.Parent = null;
            }
        }

        private void RemoveChildren(Node<Person> person)
        {
            for(int i = 0; i < person.Children.Count; i++)
            {
                Node<Person> child = person.Children[i];
                RemoveChildren(person.Children[i]);
                generationManager.RemoveChild(person.Children[i], person.Children[i].Value.Name);
                person.Children.Remove(person.Children[i]);

                ConnectChildrenToParents(generationManager.GetGenerationByID(child.Value.GenerationID), new GenerationChangedEventArgs());

                if(generationManager.GetGenerationByID(child.Value.GenerationID).GenerationGridList.Count == 0)
                {
                    generationManager.generationList.Remove(generationManager.GetGenerationByID(child.Value.GenerationID));
                    treeGrid.RowDefinitions.Remove(treeGrid.RowDefinitions[treeGrid.RowDefinitions.Count - 1]);
                }

                i--;
            }
        }

        private void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = (Exception)e.ExceptionObject;
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\ErrorLog.txt", exception.Source + "\n" + exception.Message + "\n" +
                exception.StackTrace);
        }

        private void ParentsAdded(object sender, ParentsAddedEventArgs e)
        {
            Generation childGeneration = generationManager.GetGenerationByID(e.parent.Children[0].Value.GenerationID);
            generationManager.InsertGeneration(new Generation(null), 0);
            e.parent.Value.GenerationID = generationManager.generationList[0].GenerationID;
            generationManager.generationList[0].AddPerson(e.parent);
            childGeneration.ParentsGridList = generationManager.generationList[0].GenerationGridList;
        }
    }
}
