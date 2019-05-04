using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenealogyTree
{
    class Person
    {
        public string Name { get; set; }
        public string Partner { get; set; }
        public DateTime birthDate { get; set; }
        public DateTime deathDate { get; set; }

        public Person() : this(string.Empty, string.Empty)
        {

        }

        public Person(string name) : this(name, string.Empty)
        {

        }

        public Person(string name, string partner)
        {
            this.Name = name;
            this.Partner = partner;
        }
    }
}
