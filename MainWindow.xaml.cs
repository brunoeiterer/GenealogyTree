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
        private StackPanel treePanel;
        private PersonTree personTree;
        private GenerationManager generationManager;

        public MainWindow()
        {
            InitializeComponent();

            menu = new Menu();

            generationManager = new GenerationManager();
            generationManager.AddGeneration(new Generation());

            personTree = new PersonTree();
            PersonTree.Tree.Value.Name = "test";
            PersonTree.Tree.Value.Partner = "test2";
            PersonTree.Tree.Value.BirthDate = new Nullable<DateTime>(DateTime.Now);
            PersonTree.Tree.Value.DeathDate = new Nullable<DateTime>(DateTime.Now);
            PersonTree.Tree.Value.GenerationID = generationManager.generationList[0].GenerationID;
            PersonTree.NewChildAddedEvent += NewChildAdded;

            generationManager.generationList[generationManager.generationList.Count - 1].AddPerson(PersonTree.Tree, true);
            generationManager.NewGenerationAdded += AddNewGenerationToTreePanel;



            Binding panelWidthBinding = new Binding()
            {
                Source = Application.Current.MainWindow,
                Path = new PropertyPath(MainWindow.ActualWidthProperty)
            };

            treePanel = new StackPanel()
            {
                Orientation = Orientation.Vertical
            };
            treePanel.SetBinding(StackPanel.WidthProperty, panelWidthBinding);
            treePanel.Children.Add(generationManager.generationList[generationManager.generationList.Count - 1].GenerationGrid);
            DockPanel.SetDock(treePanel, Dock.Top);

            basePanel = new DockPanel();
            basePanel.SetBinding(DockPanel.WidthProperty, panelWidthBinding);
            basePanel.Children.Add(menu.BasePanel);
            basePanel.Children.Add(treePanel);

            this.AddChild(basePanel);
        }

        public void AddNewGenerationToTreePanel(object sender, NewGenerationAddedEventArgs e)
        {
            treePanel.Children.Add(e.generation.GenerationGrid);
        }

        private void NewChildAdded(object sender, NewChildAddedEventArgs<Person> e)
        {
            generationManager.AddChild(PersonTree.GetNodeByName(PersonTree.Tree, e.child.Name));
        }
    }
}
