using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenealogyTree
{
    [Serializable]
    public class Person
    {
        public string Name { get; set; }
        public string Partner { get; set; }
        public Nullable<DateTime> BirthDate { get; set; }
        public Nullable<DateTime> DeathDate { get; set; }
        public Nullable<DateTime> PartnerBirthDate { get; set; }
        public Nullable<DateTime> PartnerDeathDate { get; set; }
        public string BirthPlace { get; set; }
        public string PartnerBirthPlace { get; set; }
        public Guid GenerationID {get; set;}
        public bool IsPartnerInFamily { get; set; }

        public Person() : this(string.Empty, string.Empty, null, null, null, null, string.Empty, string.Empty)
        {

        }

        public Person(string name) : this(name, string.Empty, null, null, null, null, string.Empty, string.Empty)
        {

        }

        public Person(string name, string partner) : this(name, partner, null, null, null, null, string.Empty, string.Empty)
        {

        }

        public Person(string name, string partner, Nullable<DateTime> birthDate) : this(name, partner, birthDate, null, null, null, 
            string.Empty, string.Empty)
        {

        }

        public Person(string name, string partner, Nullable<DateTime> birthDate, Nullable<DateTime> deathDate): 
            this(name, partner, birthDate, deathDate, null, null, string.Empty, string.Empty)
        {

        }

        public Person(string name, string partner, Nullable<DateTime> birthDate, Nullable<DateTime> deathDate,
            Nullable<DateTime> partnerBirthDate) : this(name, partner, birthDate, deathDate, partnerBirthDate, null, string.Empty, string.Empty)
        {

        }

        public Person(string name, string partner, Nullable<DateTime> birthDate, Nullable<DateTime> deathDate, 
            Nullable<DateTime> partnerBirthDate, Nullable<DateTime> partnerDeathDate, string birthPlace) : 
            this(name, partner, birthDate, deathDate, partnerBirthDate, null, birthPlace, string.Empty)
        {

        }

        public Person(string name, string partner, Nullable<DateTime> birthDate, Nullable<DateTime> deathDate,
            Nullable<DateTime> partnerBirthDate, Nullable<DateTime> partnerDeathDate, string birthPlace, string PartnerBirthPlace)
        {
            this.Name = name;
            this.Partner = partner;
            this.BirthDate = birthDate;
            this.DeathDate = deathDate;
            this.PartnerBirthDate = PartnerBirthDate;
            this.PartnerDeathDate = PartnerDeathDate;
            this.BirthPlace = birthPlace;
            this.PartnerBirthPlace = PartnerBirthPlace;
        }
    }
}
