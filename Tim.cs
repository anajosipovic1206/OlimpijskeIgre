using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OlimpijskeIgre
{
        public class Tim
        {
            public string Naziv { get; set; }
            public string IsoKod { get; set; }
            public int FIBARang { get; set; }
            public int Bodovi { get; set; } = 0; 
            public int PostignutiPoeni { get; set; } = 0;
            public int PrimljeniPoeni { get; set; } = 0;
        
    }


}
