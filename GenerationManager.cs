﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void AddChild(Node<Person> child)
        {
            Guid ParentID = child.Parent.Value.GenerationID;

            if (generationList.IndexOf(GetGenerationByID(ParentID)) == generationList.Count - 1)
            {
                AddGeneration(new Generation());
                child.Value.GenerationID = generationList[generationList.Count - 1].GenerationID;
                generationList[generationList.Count - 1].AddPerson(child, false);
            }
            else
            {
                child.Value.GenerationID = generationList[generationList.IndexOf(GetGenerationByID(ParentID)) + 1].GenerationID;
                generationList[generationList.IndexOf(GetGenerationByID(ParentID)) + 1].AddPerson(child, false);
            }
        }
    }
}
