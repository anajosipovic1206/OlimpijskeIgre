using OlimpijskeIgre;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace OlimpijskeIgre
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                OlimpijskeIgre.OlimpijskeIgre igre = new OlimpijskeIgre.OlimpijskeIgre();

                igre.UcitajGrupe("groups.json");
                igre.UcitajIzlozbeneMeceve("exibitions.json");

                igre.PokreniSimulaciju();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Console.WriteLine("Program je završio sa radom.");
            }

            //Console.ReadKey();
        }

    }
}
