using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OlimpijskeIgre
{
    namespace OlimpijskeIgre
    {
        public class OlimpijskeIgre
        {
            public List<Grupa> Grupe { get; private set; }
            public Dictionary<string, List<IzlozbeniMec>> IzlozbeniMecevi { get; private set; }
            public List<Utakmica> Utakmice { get; private set; }
            private Dictionary<string, List<TabelaEntry>> tabela;



            public OlimpijskeIgre()
            {
                Grupe = new List<Grupa>();
                IzlozbeniMecevi = new Dictionary<string, List<IzlozbeniMec>>();
            }
            public void UcitajGrupe(string putanjaDoJson)
            {
                string json = File.ReadAllText(putanjaDoJson);
                var grupe = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<TimJson>>>(json);

                foreach (var grupa in grupe)
                {
                    Grupa novaGrupa = new Grupa
                    {
                        NazivGrupe = grupa.Key,
                        Timovi = grupa.Value.Select(t => new Tim
                        {
                            Naziv = t.Team, 
                            IsoKod = t.ISOCode, 
                            FIBARang = t.FIBARanking 
                        }).ToList()
                    };

                   
                    foreach (var tim in novaGrupa.Timovi)
                    {
                        if (string.IsNullOrEmpty(tim.Naziv) || string.IsNullOrEmpty(tim.IsoKod))
                        {
                            Console.WriteLine($"Greška: Tim u grupi {grupa.Key} nije ispravno učitan.");
                        }
                    }

                    Grupe.Add(novaGrupa);

                }
            }
            public void UcitajIzlozbeneMeceve(string putanjaDoJson)
            {
                string json = File.ReadAllText(putanjaDoJson);
                var mecevi = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<IzlozbeniMec>>>(json);

                foreach (var par in mecevi)
                {
                    string isoKod = par.Key;
                    IzlozbeniMecevi[isoKod] = par.Value;
                }
            }


            public void PrikaziRangiranjeTimova()
            {
                Console.WriteLine("Rangiranje timova po FIBA rankingu:");
                foreach (var grupa in Grupe)
                {
                    Console.WriteLine($"Grupa {grupa.NazivGrupe}:");
                    foreach (var tim in grupa.Timovi.OrderBy(t => t.FIBARang))
                    {
                        Console.WriteLine($"- {tim.Naziv} (FIBA Ranking: {tim.FIBARang})");
                    }
                    Console.WriteLine();
                }
            }
            public void DodajRezultateIzlozbenihMeca()
            {
                Console.WriteLine("Rezultati izložbenih mečeva:");
                foreach (var par in IzlozbeniMecevi)
                {
                    string isoKod = par.Key;
                    Console.WriteLine($"Tim {NadjiTimPoISOKodu(isoKod).Naziv}:");
                    foreach (var mec in par.Value)
                    {
                        Console.WriteLine($"- Protivnik: {mec.Protivnik}, Datum: {mec.Datum.ToShortDateString()}, Rezultat: {mec.Rezultat}");
                    }
                    Console.WriteLine(); 
                }
            }

            private Tim NadjiTimPoISOKodu(string isoKod)
            {
                foreach (var grupa in Grupe)
                {
                    var tim = grupa.Timovi.FirstOrDefault(t => t.IsoKod == isoKod);
                    if (tim != null)
                    {
                        return tim;
                    }
                }
                return null;
            }
            public void PrikaziRezultateMeceva()
            {
                Console.WriteLine("Rezultati svih mečeva:");
                foreach (var par in IzlozbeniMecevi)
                {
                    var tim = NadjiTimPoISOKodu(par.Key);
                    Console.WriteLine($"Tim {tim.Naziv}:");
                    foreach (var mec in par.Value)
                    {
                        Console.WriteLine($"- Protivnik: {mec.Protivnik}, Datum: {mec.Datum.ToShortDateString()}, Rezultat: {mec.Rezultat}");
                    }
                    Console.WriteLine(); 
                }
            }
            public void PrikaziRezultateTima(string isoKod)
            {
                var tim = NadjiTimPoISOKodu(isoKod);
                if (tim != null)
                {
                    Console.WriteLine($"Rezultati izložbenih mečeva za tim {tim.Naziv}:");
                    if (IzlozbeniMecevi.ContainsKey(isoKod))
                    {
                        foreach (var mec in IzlozbeniMecevi[isoKod])
                        {
                            Console.WriteLine($"- Protivnik: {mec.Protivnik}, Datum: {mec.Datum.ToShortDateString()}, Rezultat: {mec.Rezultat}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nema zabeleženih rezultata.");
                    }
                }
                else
                {
                    Console.WriteLine($"Tim sa ISO kodom {isoKod} nije pronađen.");
                }
            }
            public void PrikaziProsecnePoene()
            {
                Console.WriteLine("Prosečni postignuti poeni po timu:");

                foreach (var par in IzlozbeniMecevi)
                {
                    var tim = NadjiTimPoISOKodu(par.Key);
                    if (tim != null)
                    {
                        double ukupniPoeni = 0;
                        int brojMeceva = par.Value.Count;

                        foreach (var mec in par.Value)
                        {
                            string[] rezultat = mec.Rezultat.Split('-');
                            if (rezultat.Length == 2 && rezultat[0].All(char.IsDigit))
                            {
                                ukupniPoeni += int.Parse(rezultat[0]);
                            }
                        }

                        double prosecniPoeni = brojMeceva > 0 ? ukupniPoeni / brojMeceva : 0;
                        Console.WriteLine($"- {tim.Naziv}: {prosecniPoeni} poena po meču");
                    }
                }
            }
            public void PrikaziProsecnePoeneProtivnika()
            {
                Console.WriteLine("Prosečni postignuti poeni protivničkih timova:");

                foreach (var par in IzlozbeniMecevi)
                {
                    var tim = NadjiTimPoISOKodu(par.Key);
                    if (tim != null)
                    {
                        double ukupniPoeniProtivnika = 0;
                        int brojMeceva = par.Value.Count;

                        foreach (var mec in par.Value)
                        {
                            string[] rezultat = mec.Rezultat.Split('-');
                            if (rezultat.Length == 2 && rezultat[1].All(char.IsDigit))
                            {
                                ukupniPoeniProtivnika += int.Parse(rezultat[1]);
                            }
                        }

                        double prosecniPoeniProtivnika = brojMeceva > 0 ? ukupniPoeniProtivnika / brojMeceva : 0;
                        Console.WriteLine($"- Protivnici tima {tim.Naziv}: {prosecniPoeniProtivnika} poena po meču");
                    }
                }
            }
            public List<Utakmica> KreirajRasporedUtakmica()
            {
                List<Utakmica> raspored = new List<Utakmica>();

                foreach (var grupa in Grupe)
                {
                    var timovi = grupa.Timovi;

                    for (int i = 0; i < timovi.Count; i++)
                    {
                        for (int j = i + 1; j < timovi.Count; j++)
                        {
                            raspored.Add(new Utakmica
                            {
                                Tim1 = timovi[i],
                                Tim2 = timovi[j],
                                Grupa = grupa.NazivGrupe,
                                Datum = DateTime.Now.AddDays(raspored.Count) 
                            });
                        }
                    }
                }

                return raspored;
            }
            public void PrikaziRasporedUtakmica(List<Utakmica> raspored)
            {
                Console.WriteLine("Raspored utakmica:");

                foreach (var utakmica in raspored)
                {
                    Console.WriteLine($"{utakmica.Grupa}: {utakmica.Tim1.Naziv} vs {utakmica.Tim2.Naziv} - {utakmica.Datum.ToShortDateString()}");
                }
            }
            public Dictionary<string, List<TabelaEntry>> KreirajTabelu()
            {
                var tabela = new Dictionary<string, List<TabelaEntry>>();

                foreach (var grupa in Grupe)
                {
                    tabela[grupa.NazivGrupe] = new List<TabelaEntry>();

                    foreach (var tim in grupa.Timovi)
                    {
                        tabela[grupa.NazivGrupe].Add(new TabelaEntry
                        {
                            Tim = tim,
                            Odigrano = 0,
                            Pobede = 0,
                            Porazi = 0,
                            PostignutiPoeni = 0,
                            PrimljeniPoeni = 0
                        });
                    }
                }

                return tabela;
            }
            public void KreirajUtakmice()
            {
                Utakmice = new List<Utakmica>();

                foreach (var grupa in Grupe)
    {
                    // Kombinovanje svih timova unutar jedne grupe za kreiranje utakmica
                    for (int i = 0; i < grupa.Timovi.Count; i++)
                    {
                        for (int j = i + 1; j < grupa.Timovi.Count; j++)
                        {
                            var utakmica = new Utakmica(grupa.Timovi[i], grupa.Timovi[j], grupa.NazivGrupe, DateTime.Now);
                            Utakmice.Add(utakmica);
                        }
                    }
                }
            }

            public void AzurirajBodove()
            {
                foreach (var utakmica in Utakmice)
                {
                    if (!string.IsNullOrEmpty(utakmica.Rezultat))
                    {
                        var rezultat = utakmica.Rezultat.Split('-');
                        int poeniTim1 = int.Parse(rezultat[0]);
                        int poeniTim2 = int.Parse(rezultat[1]);

                        if (poeniTim1 > poeniTim2)
                        {
                            utakmica.Tim1.Bodovi += 2; // Pobeda donosi 2 boda
                            utakmica.Tim2.Bodovi += 1; // Poraz donosi 1 bod
                        }
                        else if (poeniTim1 < poeniTim2)
                        {
                            utakmica.Tim1.Bodovi += 1;
                            utakmica.Tim2.Bodovi += 2;
                        }
                    }
                }
            }
            public void PrikaziTabele()
            {
                foreach (var grupa in Grupe)
                {
                    Console.WriteLine($"Tabela za grupu {grupa.NazivGrupe}:");

                    var sortiraniTimovi = grupa.Timovi.OrderByDescending(t => t.Bodovi).ToList();
                    foreach (var tim in sortiraniTimovi)
                    {
                        Console.WriteLine($"{tim.Naziv}: {tim.Bodovi} bodova");
                    }

                    Console.WriteLine();
                }
            }
            public void AzurirajTabelu()
            {
                foreach (var grupa in Grupe)
                {
                    foreach (var tim in grupa.Timovi)
                    {
                        if (IzlozbeniMecevi.ContainsKey(tim.IsoKod))
                        {
                            foreach (var mec in IzlozbeniMecevi[tim.IsoKod])
                            {
                                var timoviUTabeli = tabela.ContainsKey(grupa.NazivGrupe) ? tabela[grupa.NazivGrupe] : null;
                                if (timoviUTabeli == null)
                                {
                                    Console.WriteLine($"Tabela za grupu {grupa.NazivGrupe} ne postoji.");
                                    continue;
                                }

                                var timTabelaEntry = timoviUTabeli.FirstOrDefault(te => te.Tim.IsoKod == tim.IsoKod);
                                var protivnikTabelaEntry = timoviUTabeli.FirstOrDefault(te => te.Tim.IsoKod == mec.Protivnik);

                                if (timTabelaEntry != null && protivnikTabelaEntry != null)
                                {
                                    var rezultat = mec.Rezultat.Split('-');
                                    if (rezultat.Length == 2 && int.TryParse(rezultat[0], out int poeniTim) && int.TryParse(rezultat[1], out int poeniProtivnik))
                                    {
                                        // Ažuriraj statistiku za trenutni tim
                                        timTabelaEntry.PostignutiPoeni += poeniTim;
                                        timTabelaEntry.PrimljeniPoeni += poeniProtivnik;
                                        timTabelaEntry.Odigrano++;

                                        // Ažuriraj statistiku za protivnički tim
                                        protivnikTabelaEntry.PostignutiPoeni += poeniProtivnik;
                                        protivnikTabelaEntry.PrimljeniPoeni += poeniTim;
                                        protivnikTabelaEntry.Odigrano++;

                                        // Ažuriranje pobeda i poraza
                                        if (poeniTim > poeniProtivnik)
                                        {
                                            timTabelaEntry.Pobede++;
                                            protivnikTabelaEntry.Porazi++;
                                        }
                                        else
                                        {
                                            timTabelaEntry.Porazi++;
                                            protivnikTabelaEntry.Pobede++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Prikazivanje ažurirane tabele
                PrikaziTabelu();
            }

            // Inicijalizacija tabele za svaku grupu
            public void InicijalizujTabelu()
            {
                tabela = new Dictionary<string, List<TabelaEntry>>();

                foreach (var grupa in Grupe)
                {
                    tabela[grupa.NazivGrupe] = new List<TabelaEntry>();

                    foreach (var tim in grupa.Timovi)
                    {
                        if (tim == null)
                        {
                            Console.WriteLine($"Tim u grupi {grupa.NazivGrupe} je NULL!");
                        }

                        tabela[grupa.NazivGrupe].Add(new TabelaEntry
                        {
                            Tim = tim,
                            Odigrano = 0,
                            Pobede = 0,
                            Porazi = 0,
                            PostignutiPoeni = 0,
                            PrimljeniPoeni = 0
                        });
                    }
                }
            }

            // Sortiranje tabele po broju pobeda i koš razlici
            private void SortirajTabelu()
            {
                foreach (var grupa in tabela.Keys)
                {
                    tabela[grupa] = tabela[grupa]
                        .OrderByDescending(te => te.Pobede)
                        .ThenByDescending(te => te.PostignutiPoeni - te.PrimljeniPoeni)
                        .ToList();
                }
            }

            // Prikaz tabele u konzoli
            public void PrikaziTabelu()
            {
                foreach (var grupa in tabela.Keys)
                {
                    Console.WriteLine($"Tabela za grupu: {grupa}");

                    foreach (var entry in tabela[grupa])
                    {
                        Console.WriteLine($"{entry.Tim.Naziv}: Odigrano {entry.Odigrano}, Pobede {entry.Pobede}, Porazi {entry.Porazi}, Poeni {entry.PostignutiPoeni}:{entry.PrimljeniPoeni}");
                    }

                    Console.WriteLine();
                }
            }
       
            public void SimulirajGrupnuFazu()
            {
                Random random = new Random();

                foreach (var grupa in Grupe)
                {
                    Console.WriteLine($"Grupa {grupa.NazivGrupe}:");

                    for (int i = 0; i < grupa.Timovi.Count; i++)
                    {
                        for (int j = i + 1; j < grupa.Timovi.Count; j++)
                        {
                            var tim1 = grupa.Timovi[i];
                            var tim2 = grupa.Timovi[j];

                            if (tim1 == null || tim2 == null)
                            {
                                Console.WriteLine("Jedan od timova u grupi je NULL!");
                                continue;
                            }

                            // Simulacija rezultata
                            int poeniTim1 = random.Next(70, 100);
                            int poeniTim2 = random.Next(70, 100);

                            //prema FIBA rangu
                            if (tim1.FIBARang < tim2.FIBARang)
                            {
                                poeniTim1 += random.Next(0, 15);
                            }
                            else if (tim2.FIBARang < tim1.FIBARang)
                            {
                                poeniTim2 += random.Next(0, 15);
                            }

                            // Ažuriranje bodova
                            var entryTim1 = tabela[grupa.NazivGrupe].First(te => te.Tim == tim1);
                            var entryTim2 = tabela[grupa.NazivGrupe].First(te => te.Tim == tim2);

                            entryTim1.DodajRezultat(poeniTim1, poeniTim2);
                            entryTim2.DodajRezultat(poeniTim2, poeniTim1);

                            Console.WriteLine($"{tim1.Naziv} {poeniTim1} - {poeniTim2} {tim2.Naziv}");
                        }
                    }
                    Console.WriteLine();
                }
            }

            public void PrikaziKrajnjiPlasman()
            {
                Console.WriteLine("Konačan plasman u grupama:");

                foreach (var grupa in Grupe)
                {
                    Console.WriteLine($"    Grupa {grupa.NazivGrupe} (Ime - pobede/porazi/bodovi/postignuti koševi/primljeni koševi/koš razlika):");

                    var timoviUTabeli = tabela[grupa.NazivGrupe]
                        .OrderByDescending(t => t.Bodovi)
                        .ThenByDescending(t => t.PostignutiPoeni - t.PrimljeniPoeni)
                        .ThenByDescending(t => t.PostignutiPoeni)
                        .ToList();

                    int pozicija = 1;
                    foreach (var entry in timoviUTabeli)
                    {
                        int kosRazlika = entry.PostignutiPoeni - entry.PrimljeniPoeni;
                        Console.WriteLine($"        {pozicija}. {entry.Tim.Naziv} - {entry.Pobede} pobede, {entry.Porazi} porazi, {entry.Bodovi} bodova, " +
                                          $"{entry.PostignutiPoeni} postignuti koševi, {entry.PrimljeniPoeni} primljeni koševi, Koš razlika: {kosRazlika}");
                        pozicija++;
                    }

                    Console.WriteLine();
                }
            }


            public void FormirajSesire()
            {
                var prvoplasirani = tabela.Values.SelectMany(x => x)
                    .OrderByDescending(te => te.Tim.Bodovi)
                    .ThenByDescending(te => te.PostignutiPoeni - te.PrimljeniPoeni)
                    .Take(3).Select(te => te.Tim).ToList();

                var drugoplasirani = tabela.Values.SelectMany(x => x)
                    .OrderByDescending(te => te.Tim.Bodovi)
                    .ThenByDescending(te => te.PostignutiPoeni - te.PrimljeniPoeni)
                    .Skip(3).Take(3).Select(te => te.Tim).ToList();

                var treceplasirani = tabela.Values.SelectMany(x => x)
                    .OrderByDescending(te => te.Tim.Bodovi)
                    .ThenByDescending(te => te.PostignutiPoeni - te.PrimljeniPoeni)
                    .Skip(6).Take(3).Select(te => te.Tim).ToList();

                if (prvoplasirani.Count != 3 || drugoplasirani.Count != 3 || treceplasirani.Count != 3)
                {
                    throw new InvalidOperationException("Došlo je do greške pri formiranju šešira.");
                }

                var sesirD = prvoplasirani;
                var sesirE = drugoplasirani;
                var sesirF = treceplasirani;

                Console.WriteLine("Šeširi:");
                Console.WriteLine("    Šešir D: " + string.Join(", ", sesirD.Select(t => t.Naziv)));
                Console.WriteLine("    Šešir E: " + string.Join(", ", sesirE.Select(t => t.Naziv)));
                Console.WriteLine("    Šešir F: " + string.Join(", ", sesirF.Select(t => t.Naziv)));
                Console.WriteLine();
            }

            private List<Tuple<Tim, Tim>> FormirajCetvrtfinalneParove()
            {
                var sesirD = tabela.Values.SelectMany(x => x)
                    .OrderByDescending(te => te.Tim.Bodovi)
                    .ThenByDescending(te => te.PostignutiPoeni - te.PrimljeniPoeni)
                    .Take(4).Select(te => te.Tim).ToList();  // Koristi sve timove sa rangom 1 i 2

                var sesirE = tabela.Values.SelectMany(x => x)
                    .OrderByDescending(te => te.Tim.Bodovi)
                    .ThenByDescending(te => te.PostignutiPoeni - te.PrimljeniPoeni)
                    .Skip(4).Take(4).Select(te => te.Tim).ToList();  // Koristi sve timove sa rangom 3 i 4

                if (sesirD.Count != 4 || sesirE.Count != 4)
                {
                    throw new InvalidOperationException("Došlo je do greške: Nedovoljan broj timova za formiranje četvrtfinala.");
                }

                var cetvrtfinalniParovi = new List<Tuple<Tim, Tim>>();
                for (int i = 0; i < sesirD.Count; i++)
                {
                    cetvrtfinalniParovi.Add(Tuple.Create(sesirD[i], sesirE[i]));
                }

                return cetvrtfinalniParovi;
            }


            private void SimulirajUtakmice(string faza, List<Tuple<Tim, Tim>> parovi)
            {
                Random random = new Random();

                Console.WriteLine($"{faza}:");
                foreach (var par in parovi)
                {
                    if (par.Item1 == null || par.Item2 == null || string.IsNullOrEmpty(par.Item1.Naziv) || string.IsNullOrEmpty(par.Item2.Naziv))
                    {
                        Console.WriteLine("Greška: Jedan od timova u utakmici je NULL ili nema naziv.");
                        continue;
                    }

                    int poeniTim1 = random.Next(70, 100);
                    int poeniTim2 = random.Next(70, 100);

                  
                    while (poeniTim1 == poeniTim2)
                    {
                        poeniTim1 += random.Next(1, 5); 
                        poeniTim2 += random.Next(1, 5); 
                    }

                    Console.WriteLine($"{par.Item1.Naziv} {poeniTim1} - {poeniTim2} {par.Item2.Naziv}");

                    if (poeniTim1 > poeniTim2)
                    {
                        par.Item1.Bodovi = poeniTim1;
                        par.Item2.Bodovi = poeniTim2;
                        Console.WriteLine($"Pobednik: {par.Item1.Naziv} sa {par.Item1.Bodovi} bodova");
                    }
                    else
                    {
                        par.Item2.Bodovi = poeniTim2;
                        par.Item1.Bodovi = poeniTim1;
                        Console.WriteLine($"Pobednik: {par.Item2.Naziv} sa {par.Item2.Bodovi} bodova");
                    }
                }
                Console.WriteLine();
            }
            private Tuple<Tim, Tim> FormirajUtakmicuZaTreceMesto(List<Tuple<Tim, Tim>> polufinalniParovi)
            {
                var porazeni = new List<Tim>();

                // Identifikovanje poraženih timova iz polufinala
                foreach (var par in polufinalniParovi)
                {
                    // Dodaj poraženog u listu
                    porazeni.Add(par.Item1.Bodovi > par.Item2.Bodovi ? par.Item2 : par.Item1);
                }

                if (porazeni.Count == 2)
                {
                    return Tuple.Create(porazeni[0], porazeni[1]);
                }
                else
                {
                    throw new InvalidOperationException("Greška: Nedovoljan broj timova za utakmicu za treće mesto.");
                }
            }

            private List<Tuple<Tim, Tim>> FormirajPolufinalneParove(List<Tuple<Tim, Tim>> cetvrtfinalniParovi)
            {
                var pobednici = new List<Tim>();

                // Dodavanje pobednika iz četvrtfinala u listu
                foreach (var par in cetvrtfinalniParovi)
                {
                    pobednici.Add(par.Item1.Bodovi > par.Item2.Bodovi ? par.Item1 : par.Item2);
                }

                if (pobednici.Count != 4)
                {
                    throw new InvalidOperationException("Greška: Nedovoljan broj pobednika za formiranje polufinala.");
                }

                // Formiranje polufinalnih parova
                return new List<Tuple<Tim, Tim>>
                {
                    Tuple.Create(pobednici[0], pobednici[1]),
                    Tuple.Create(pobednici[2], pobednici[3])
                 };
            }
            private Tuple<Tim, Tim> FormirajFinalnuUtakmicu(List<Tuple<Tim, Tim>> polufinalniParovi)
            {
                var pobednici = new List<Tim>();

                // Dodavanje pobednika iz polufinala u listu
                foreach (var par in polufinalniParovi)
                {
                    pobednici.Add(par.Item1.Bodovi > par.Item2.Bodovi ? par.Item1 : par.Item2);
                }

                if (pobednici.Count != 2)
                {
                    throw new InvalidOperationException("Greška: Nedovoljan broj pobednika za formiranje finala.");
                }

                return Tuple.Create(pobednici[0], pobednici[1]);
            }
            public void SimulirajEliminacionuFazu()
            {
                try
                {
                    var cetvrtfinalniParovi = FormirajCetvrtfinalneParove();
                    Console.WriteLine("Eliminaciona faza:");

                    SimulirajUtakmice("Četvrtfinale", cetvrtfinalniParovi);

                    var polufinalniParovi = FormirajPolufinalneParove(cetvrtfinalniParovi);
                    SimulirajUtakmice("Polufinale", polufinalniParovi);

                   
                    var utakmicaZaTrećeMesto = FormirajUtakmicuZaTreceMesto(polufinalniParovi);
                    SimulirajUtakmice("Utakmica za treće mesto", new List<Tuple<Tim, Tim>> { utakmicaZaTrećeMesto });

                    var finalnaUtakmica = FormirajFinalnuUtakmicu(polufinalniParovi);
                    
                    SimulirajUtakmice("Finale", new List<Tuple<Tim, Tim>> { finalnaUtakmica });

                    Console.WriteLine("\nMedalje:");
              
                    Console.WriteLine("    1. mesto: " + (finalnaUtakmica.Item1.Bodovi > finalnaUtakmica.Item2.Bodovi ? finalnaUtakmica.Item1.Naziv : finalnaUtakmica.Item2.Naziv));
                    Console.WriteLine("    2. mesto: " + (finalnaUtakmica.Item1.Bodovi < finalnaUtakmica.Item2.Bodovi ? finalnaUtakmica.Item1.Naziv : finalnaUtakmica.Item2.Naziv));
                    Console.WriteLine("    3. mesto: " + (utakmicaZaTrećeMesto.Item1.Bodovi > utakmicaZaTrećeMesto.Item2.Bodovi ? utakmicaZaTrećeMesto.Item1.Naziv : utakmicaZaTrećeMesto.Item2.Naziv));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Greska: {ex.Message}");
                }
            }


            public void ResetujStanjeTimova()
            {
                foreach (var grupa in Grupe)
                {
                    foreach (var tim in grupa.Timovi)
                    {
                        tim.Bodovi = 0;
                        tim.PostignutiPoeni = 0;
                        tim.PrimljeniPoeni = 0;
                    }
                }
            }

            public void ResetujTabelu()
            {
                tabela = new Dictionary<string, List<TabelaEntry>>();
                foreach (var grupa in Grupe)
                {
                    tabela[grupa.NazivGrupe] = new List<TabelaEntry>();
                    foreach (var tim in grupa.Timovi)
        {
                        tabela[grupa.NazivGrupe].Add(new TabelaEntry
                        {
                            Tim = tim
                        });
                    }
                }
            }

            public void PokreniSimulaciju()
            {
            
                ResetujStanjeTimova();
                ResetujTabelu();
                InicijalizujTabelu();
                KreirajUtakmice();    
                SimulirajGrupnuFazu();
                SortirajTabelu();      
                PrikaziKrajnjiPlasman(); 

                FormirajSesire();      
                SimulirajEliminacionuFazu(); 
            }

        }
        }
    }
