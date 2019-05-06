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
        private StackPanel basePanel;
        private Menu menu;
        private StackPanel treePanel;

        private PersonTree personTree;

        public MainWindow()
        {
            InitializeComponent();

            personTree = new PersonTree();          

            menu = new Menu();

            Generation generation = new Generation();
            generation.AddPerson(PersonTree.Tree);
            generation.SubscribeToGotFocus(menu.PersonGotFocus);
            generation.SubscribeToLostFocus(menu.PersonLostFocus);

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
            treePanel.Children.Add(generation.GenerationGrid);

            basePanel = new StackPanel()
            {
                Orientation = Orientation.Vertical
            };
            basePanel.SetBinding(StackPanel.WidthProperty, panelWidthBinding);
            basePanel.Children.Add(menu.BasePanel);
            basePanel.Children.Add(treePanel);

            this.AddChild(basePanel);
        }
    }
}
