using System;

namespace GenealogyTree
{
    public class PartnerAddedEventArgs : EventArgs
    {
        public string childName;
        public string partnerName;
        public Nullable<DateTime> birthDate;
        public Nullable<DateTime> deathDate;
        public string birthPlace;
    }

    public delegate void PartnerAddedEventHandler(object sender, PartnerAddedEventArgs e);

}
