using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        private PersonTree personTree;
        private GenerationManager generationManager;

        public MainWindow()
        {
            InitializeComponent();

            menu = new Menu();
            menu.PartnerAdded += PartnerAdded;

            generationManager = new GenerationManager();
            generationManager.AddGeneration(new Generation(null));
            generationManager.GenerationChanged += ConnectChildrenToParents;

            personTree = new PersonTree();
            PersonTree.Tree.Value.Name = "test";
            PersonTree.Tree.Value.Partner = "test2";
            PersonTree.Tree.Value.BirthDate = new Nullable<DateTime>(DateTime.Now);
            PersonTree.Tree.Value.DeathDate = new Nullable<DateTime>(DateTime.Now);
            PersonTree.Tree.Value.GenerationID = generationManager.generationList[0].GenerationID;
            PersonTree.NewChildAddedEvent += NewChildAdded;

            generationManager.generationList[generationManager.generationList.Count - 1].AddPerson(PersonTree.Tree);
            generationManager.NewGenerationAdded += AddNewGenerationToTreePanel;



            Binding panelWidthBinding = new Binding()
            {
                Source = Application.Current.MainWindow,
                Path = new PropertyPath(MainWindow.ActualWidthProperty)
            };
            
            treeGrid = new Grid();
            treeGrid.Children.Add(generationManager.generationList[generationManager.generationList.Count - 1].BaseGrid);
            treeGrid.ColumnDefinitions.Add(new ColumnDefinition());
            treeGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            treeGrid.RowDefinitions.Add(new RowDefinition());
            treeGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
            Grid.SetRow(generationManager.generationList[generationManager.generationList.Count - 1].BaseGrid, 0);
            Grid.SetColumn(generationManager.generationList[generationManager.generationList.Count - 1].BaseGrid, 0);
            DockPanel.SetDock(treeGrid, Dock.Top);


            ConnectionsGrid = new Grid();
            DockPanel.SetDock(ConnectionsGrid, Dock.Top);
            //ConnectionsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            //ConnectionsGrid.RowDefinitions.Add(new RowDefinition());
            //ConnectionsGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
            //ConnectionsGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            ConnectionsGrid.Children.Add(treeGrid);

            basePanel = new DockPanel();
            basePanel.SetBinding(DockPanel.WidthProperty, panelWidthBinding);
            this.BaseGrid.Children.Add(menu.BasePanel);
            Grid.SetRow(menu.BasePanel, 0);

            this.MainWindowScrollViewer.Content = ConnectionsGrid;
        }

        public void AddNewGenerationToTreePanel(object sender, NewGenerationAddedEventArgs e)
        {
            treeGrid.Children.Add(e.generation.BaseGrid);
            treeGrid.RowDefinitions.Add(new RowDefinition());
            treeGrid.RowDefinitions[treeGrid.RowDefinitions.Count - 1].Height = new GridLength(1, GridUnitType.Auto);
            Grid.SetRow(e.generation.BaseGrid, treeGrid.RowDefinitions.Count - 1);

            Grid.SetColumn(e.generation.BaseGrid, 0);
        }

        private void NewChildAdded(object sender, NewChildAddedEventArgs<Person> e)
        {
            generationManager.AddChild(PersonTree.GetNodeByName(PersonTree.Tree, e.child.Name));
        }

        private void PartnerAdded(object sender, PartnerAddedEventArgs e)
        {
            generationManager.AddPartner(e.childName, e.partnerName, e.birthDate, e.deathDate);
        }

        private void ConnectChildrenToParents(object sender, GenerationChangedEventArgs e)
        {
            Generation generation = (Generation)sender;
            List<Node<Person>> personList = new List<Node<Person>>();

            foreach(Grid grid in generation.GenerationGridList)
            {
                foreach (TextBox textBox in grid.Children.OfType<TextBox>())
                {
                    Node<Person> nextPerson = PersonTree.GetNodeByName(PersonTree.Tree, textBox.Text);
                    if (nextPerson.Value.GenerationID == generation.GenerationID)
                    {
                        if(!personList.Any(item => item.Value.Name == nextPerson.Value.Name))
                        {
                            personList.Add(nextPerson);
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

                //if (person.Parent.Value.Partner != string.Empty)
                //{
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
                    //if(generation.ParentsGridList[parentIndex].Children.Count <= 7)
                    //{
                        generation.ParentsGridList[parentIndex].Children.Add(verticalLine1);
                        Grid.SetRow(verticalLine1, 0);
                        Grid.SetColumn(verticalLine1, parentColumnIndex + 1);
                    //}


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
                    //if (generation.ParentsGridList[parentIndex].Children.Count <= 8)
                    //{
                        generation.ParentsGridList[parentIndex].Children.Add(verticalLine2);
                        Grid.SetRow(verticalLine2, 1);
                        Grid.SetColumn(verticalLine2, parentColumnIndex + 1);
                    //}

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
                    //if (generation.ParentsGridList[parentIndex].Children.Count <= 9)
                    //{
                        generation.ParentsGridList[parentIndex].Children.Add(verticalLine3);
                        Grid.SetRow(verticalLine3, 2);
                        Grid.SetColumn(verticalLine3, parentColumnIndex + 1);
                //}
                verticalLine3.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                verticalLine3.Arrange(new Rect(verticalLine3.DesiredSize));
                verticalLine3.UpdateLayout();

                Point endPoint = verticalLine3.TranslatePoint(new Point(verticalLine3.X2, verticalLine3.Y2), ConnectionsGrid);

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

                Line verticalLine4 = new Line()
                {
                    Stroke = Brushes.Black,
                    Visibility = Visibility.Visible,
                    StrokeThickness = 1,
                    X1 = 125,
                    X2 = 125,
                    Y1 = 4,
                    Y2 = -SystemFonts.MessageFontSize * 2 - 2,
                    Stretch = Stretch.None,
                    
                };
                generation.GenerationGridList[generationGridIndex].Children.Add(verticalLine4);
                Grid.SetRow(verticalLine4, 0);
                Grid.SetColumn(verticalLine4, textBoxColumnIndex);

                verticalLine4.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                verticalLine4.Arrange(new Rect(verticalLine4.DesiredSize));
                verticalLine4.UpdateLayout();

                Point startPoint = verticalLine4.TranslatePoint(new Point(verticalLine4.X2, verticalLine4.Y2), ConnectionsGrid);

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
                //Grid.SetRow(horizontalLine1, 0);
                //Grid.SetColumn(horizontalLine1, textBoxColumnIndex + 1);

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

                /*
                int generationGridIndex = 0;
                int firstTextBoxColumnIndex = 0;
                int lastTextBoxColumnIndex = 0;
                foreach (Grid grid in generation.GenerationGridList)
                {
                    foreach (TextBox textBox in grid.Children.OfType<TextBox>())
                    {
                        if (textBox.Text == firstPerson.Value.Name)
                        {
                            generationGridIndex = generation.GenerationGridList.IndexOf(grid);
                            firstTextBoxColumnIndex = Grid.GetColumn(textBox);
                        }
                        if(textBox.Text == lastPerson.Value.Name)
                        {
                            lastTextBoxColumnIndex = Grid.GetColumn(textBox);
                        }
                    }
                }

                int columnSpan = lastTextBoxColumnIndex - firstTextBoxColumnIndex + 1;

                double x2 = 125;
                if (firstPerson != lastPerson)
                {
                    x2 = 125 + 250 + (250 * ((columnSpan / 2) - 1)) + (25 * (columnSpan / 2));
                }
                else if(firstPerson.Value.Partner != string.Empty)
                {
                    x2 = 250 + 12.5;
                    columnSpan = 2;
                }

                Line horizontalLine = new Line()
                {
                    Stroke = Brushes.Black,
                    Visibility = Visibility.Visible,
                    StrokeThickness = 1,
                    X1 = 125,
                    X2 = x2,
                    Y1 = -SystemFonts.MessageFontSize * 2 - 2,
                    Y2 = -SystemFonts.MessageFontSize * 2 - 2
                };
                Grid.SetColumnSpan(horizontalLine, columnSpan);
                Grid.SetRow(horizontalLine, 0);
                */
            }
        }
    }
}
