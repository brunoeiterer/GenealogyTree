using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;

namespace GenealogyTree
{
    class GenerationManager
    {
        public List<Generation> generationList { get; set; }

        public GenerationManager()
        {
            generationList = new List<Generation>();
        }

        public void AddGeneration(Generation generation)
        {
            generationList.Add(generation);

            if(generation.ParentsGridList != null)
            {
                generation.GenerationChanged += GenerationChangedHandler;
            }

            NewGenerationAddedEventArgs eventArgs = new NewGenerationAddedEventArgs()
            {
                generation = generation
            };
            NewGenerationAdded?.Invoke(this, eventArgs);
        }

        public event NewGenerationAddedEventHandler NewGenerationAdded;

        public void InsertGeneration(Generation generation, int index)
        {
            generationList.Insert(index, generation);

            if (generation.ParentsGridList != null)
            {
                generation.GenerationChanged += GenerationChangedHandler;
            }

            NewGenerationAddedEventArgs eventArgs = new NewGenerationAddedEventArgs()
            {
                generation = generation
            };
            NewGenerationInserted?.Invoke(this, eventArgs);
        }

        public event NewGenerationAddedEventHandler NewGenerationInserted;

        public Generation GetGenerationByID(Guid ID)
        {
            int index;
            index = generationList.IndexOf(generationList.Where(i => i.GenerationID == ID).FirstOrDefault());

            return generationList[index];
        }

        public void FirstChildAdded(object sender, NewChildAddedEventArgs<Person> e)
        {
            AddGeneration(new Generation(null));
            generationList[generationList.Count - 1].AddPerson(PersonTree.GetNodeByName(PersonTree.Tree, e.child.Name, e.child.Partner));
            PersonTree.Tree.Value.GenerationID = generationList[generationList.Count - 1].GenerationID;
        }

        public void AddChild(Node<Person> child)
        {
            Guid ParentID = child.Parent.Value.GenerationID;

            if (generationList.IndexOf(GetGenerationByID(ParentID)) == generationList.Count - 1)
            {
                AddGeneration(new Generation(generationList[generationList.Count - 1].GenerationGridList));

                if(child.Value.GenerationID == Guid.Empty)
                {
                    child.Value.GenerationID = generationList[generationList.Count - 1].GenerationID;
                }
                else
                {
                    generationList[generationList.Count - 1].GenerationID = child.Value.GenerationID;
                }
                
                generationList[generationList.Count - 1].AddPerson(child);
            }
            else
            {
                child.Value.GenerationID = generationList[generationList.IndexOf(GetGenerationByID(ParentID)) + 1].GenerationID;
                generationList[generationList.IndexOf(GetGenerationByID(ParentID)) + 1].AddPerson(child);
            }
        }

        public void RemoveChild(Node<Person> child, string name)
        {
            Generation generation = GetGenerationByID(child.Value.GenerationID);
            generation.RemovePerson(child, name);
        }

        public void AddPartner(string childName, string partnerName, Nullable<DateTime> birthDate, Nullable<DateTime> deathDate, string birthPlace)
        {
            Node<Person> child = PersonTree.GetNodeByName(PersonTree.Tree, childName, partnerName);
            GetGenerationByID(child.Value.GenerationID).AddPartner(childName, partnerName, birthDate, deathDate, birthPlace);
        }

        private void GenerationChangedHandler(object sender, GenerationChangedEventArgs e)
        {
            if(e.duplicatedName != string.Empty)
            {
                foreach(Generation generation in generationList)
                {
                    foreach(Grid grid in generation.GenerationGridList)
                    {
                        IEnumerable<TextBox> textBoxList = grid.Children.OfType<TextBox>().Where(t => t.Text == e.duplicatedName);
                        foreach(TextBox textBox in textBoxList)
                        {
                            textBox.Foreground = Brushes.Red;
                            foreach(Label label in grid.Children.OfType<Label>().Where(l => Grid.GetColumn(l) == Grid.GetColumn(textBox)))
                            {
                                label.Foreground = Brushes.Red;
                            }
                            foreach(TextBox textbox in grid.Children.OfType<TextBox>().Where(t => Grid.GetColumn(t) == Grid.GetColumn(textBox)))
                            {
                                textbox.Foreground = Brushes.Red;
                            }
                        }
                    }
                }
            }

            GenerationChanged?.Invoke(sender, e);
        }

        public event GenerationChangedEventHandler GenerationChanged;

        public void Save(object sender, SaveRequestedEventArgs e)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (Stream stream = File.Create(e.filename))
            {
                Node<Person> tempTree = PersonTree.Tree;
                formatter.Serialize(stream, tempTree);
            }
        }

        public void Open(object sender, OpenRequestedEventArgs e)
        {
            OpenRequestedEvent?.Invoke(this, new OpenRequestedEventArgs());

            BinaryFormatter formatter = new BinaryFormatter();
            Node<Person> tempTree;
            using (Stream stream = File.Open(e.filename, FileMode.Open))
            {
                stream.Position = 0;
                tempTree = (Node<Person>)formatter.Deserialize(stream);
            }

            generationList.Clear();

            PersonTree.Tree = new Node<Person>(new Person(), null);

            PersonTree.Tree.Value.Name = tempTree.Value.Name;
            PersonTree.Tree.Value.Partner = tempTree.Value.Partner;
            PersonTree.Tree.Value.BirthDate = tempTree.Value.BirthDate;
            PersonTree.Tree.Value.DeathDate = tempTree.Value.DeathDate;
            PersonTree.Tree.Value.PartnerBirthDate = tempTree.Value.PartnerBirthDate;
            PersonTree.Tree.Value.PartnerDeathDate = tempTree.Value.PartnerDeathDate;
            PersonTree.Tree.SubscribeToNewChildAdded(PersonTree.NewChildAdded);

            this.AddGeneration(new Generation(null));
            this.generationList[generationList.Count - 1].GenerationID = tempTree.Value.GenerationID;
            PersonTree.Tree.Value.GenerationID = this.generationList[0].GenerationID;
            this.generationList[generationList.Count - 1].AddPerson(PersonTree.Tree);

            Action<Person, Node<Person>> action = LoadTree;

            tempTree.Traverse(tempTree, action);

            OpenCompletedEvent?.Invoke(this, new OpenRequestedEventArgs());
        }

        public event OpenRequestedEventHandler OpenRequestedEvent;
        public event OpenRequestedEventHandler OpenCompletedEvent;

        private void LoadTree(Person person, Node<Person> node)
        {
            if(node.Parent != null)
            {
                Node<Person> parent = PersonTree.GetNodeByName(PersonTree.Tree, node.Parent.Value.Name, node.Parent.Value.Partner);
                parent.AddChild(person, node.Parent);
                parent.Children[parent.Children.Count - 1].SubscribeToNewChildAdded(PersonTree.NewChildAdded);
            }
        }
    }
}
