using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OlimpijskeIgre
{
    public class Grupa
    {
        public string NazivGrupe { get; set; }
        public List<Tim> Timovi { get; set; }
        public Grupa()
        {
            Timovi = new List<Tim>();
        }
    }

}
