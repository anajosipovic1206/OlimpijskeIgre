using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OlimpijskeIgre
{
    public class TabelaEntry
    {
        public Tim Tim { get; set; }
        public int Odigrano { get; set; }
        public int Pobede { get; set; }
        public int Porazi { get; set; }
        public int PostignutiPoeni { get; set; }
        public int PrimljeniPoeni { get; set; }
        public int Bodovi { get; set; }  

        public TabelaEntry()
        {
            Odigrano = 0;
            Pobede = 0;
            Porazi = 0;
            PostignutiPoeni = 0;
            PrimljeniPoeni = 0;
            Bodovi = 0;  
        }

        public void DodajRezultat(int poeniZa, int poeniProtiv)
        {
            Odigrano++;
            PostignutiPoeni += poeniZa;
            PrimljeniPoeni += poeniProtiv;

            if (poeniZa > poeniProtiv)
            {
                Pobede++;
                Bodovi += 2;  // Sada se Bodovi ažuriraju ovde
            }
            else
            {
                Porazi++;
                Bodovi += 1;
            }
        }
    }

}



