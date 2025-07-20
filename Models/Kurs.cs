using System;

namespace IUiUS_Projekat.Models
{
    public class Kurs
    {
        public string Naziv { get; set; }
        public double Cena { get; set; }
        public string SlikaPath { get; set; }
        public string OpisRtfPath { get; set; }
        public DateTime DatumDodavanja { get; set; }

        public Kurs() { }

        public Kurs(string naziv, double cena, string slikaPath, string opisRtfPath, DateTime datum)
        {
            Naziv = naziv;
            Cena = cena;
            SlikaPath = slikaPath;
            OpisRtfPath = opisRtfPath;
            DatumDodavanja = datum;
        }
    }
}
