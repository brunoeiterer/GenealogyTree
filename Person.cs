using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenealogyTree
{
    public class Person
    {
        public string Name { get; set; }
        public string Partner { get; set; }
        public Nullable<DateTime> BirthDate { get; set; }
        public Nullable<DateTime> DeathDate { get; set; }
        public Guid GenerationID {get; set;}

        public Person() : this(string.Empty, string.Empty, null, null)
        {

        }

        public Person(string name) : this(name, string.Empty, null, null)
        {

        }

        public Person(string name, string partner) : this(name, partner, null, null)
        {
            this.Name = name;
            this.Partner = partner;
        }

        public Person(string name, string partner, Nullable<DateTime> birthDate) : this(name, partner, birthDate, null)
        {

        }

        public Person(string name, string partner, Nullable<DateTime> birthDate, Nullable<DateTime> deathDate)
        {
            this.Name = name;
            this.Partner = partner;
            this.BirthDate = birthDate;
            this.DeathDate = deathDate;
        }
    }
}
