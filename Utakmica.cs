using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OlimpijskeIgre
{
    public class Utakmica
    {
        public Tim Tim1 { get; set; }  
        public Tim Tim2 { get; set; } 
        public string Grupa { get; set; }  
        public DateTime Datum { get; set; }  
        public string Rezultat { get; set; }  
        public Utakmica(Tim tim1, Tim tim2, string grupa, DateTime datum)
        {
            Tim1 = tim1;
            Tim2 = tim2;
            Grupa = grupa;
            Datum = datum;
            Rezultat = string.Empty;  
        }

       
        public Utakmica() { }

      
        public override string ToString()
        {
            return $"{Grupa}: {Tim1.Naziv} vs {Tim2.Naziv} - {Datum.ToShortDateString()} {(!string.IsNullOrEmpty(Rezultat) ? $"Rezultat: {Rezultat}" : string.Empty)}";
        }
    }
}
