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

            basePanel = new DockPanel();
            basePanel.SetBinding(DockPanel.WidthProperty, panelWidthBinding);
            this.BaseGrid.Children.Add(menu.BasePanel);
            Grid.SetRow(menu.BasePanel, 0);

            this.MainWindowScrollViewer.Content = treeGrid;
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
    }
}
