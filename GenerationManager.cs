using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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

        public Generation GetGenerationByID(Guid ID)
        {
            int index;
            index = generationList.IndexOf(generationList.Where(i => i.GenerationID == ID).FirstOrDefault());

            return generationList[index];
        }

        public void FirstChildAdded(object sender, NewChildAddedEventArgs<Person> e)
        {
            AddGeneration(new Generation(null));
            generationList[generationList.Count - 1].AddPerson(PersonTree.GetNodeByName(PersonTree.Tree, e.child.Name));
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

        public void AddPartner(string childName, string partnerName, Nullable<DateTime> birthDate, Nullable<DateTime> deathDate)
        {
            Node<Person> child = PersonTree.GetNodeByName(PersonTree.Tree, childName);
            GetGenerationByID(child.Value.GenerationID).AddPartner(childName, partnerName, birthDate, deathDate);
        }

        private void GenerationChangedHandler(object sender, GenerationChangedEventArgs e)
        {
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
            BinaryFormatter formatter = new BinaryFormatter();
            Node<Person> tempTree;
            using (Stream stream = File.Open(e.filename, FileMode.Open))
            {
                stream.Position = 0;
                tempTree = (Node<Person>)formatter.Deserialize(stream);
            }

            PersonTree.Tree.Value.Name = tempTree.Value.Name;
            PersonTree.Tree.Value.Partner = tempTree.Value.Partner;
            PersonTree.Tree.Value.BirthDate = tempTree.Value.BirthDate;
            PersonTree.Tree.Value.DeathDate = tempTree.Value.DeathDate;
            PersonTree.Tree.Value.PartnerBirthDate = tempTree.Value.PartnerBirthDate;
            PersonTree.Tree.Value.PartnerDeathDate = tempTree.Value.PartnerDeathDate;

            this.AddGeneration(new Generation(null));
            this.generationList[generationList.Count - 1].GenerationID = tempTree.Value.GenerationID;
            PersonTree.Tree.Value.GenerationID = this.generationList[0].GenerationID;
            this.generationList[generationList.Count - 1].AddPerson(PersonTree.Tree);

            Action<Person, Node<Person>> action = LoadTree;

            tempTree.Traverse(tempTree, action);
        }

        private void LoadTree(Person person, Node<Person> node)
        {
            if(node.Parent != null)
            {
                Node<Person> parent = PersonTree.GetNodeByName(PersonTree.Tree, node.Parent.Value.Name);
                parent.AddChild(person, node.Parent);
                parent.Children[parent.Children.Count - 1].SubscribeToNewChildAdded(PersonTree.NewChildAdded);
            }
        }
    }
}
