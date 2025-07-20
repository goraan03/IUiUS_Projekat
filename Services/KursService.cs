using IUiUS_Projekat.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace IUiUS_Projekat.Services
{
    public static class KursService
    {
        private static readonly string filePath = "Kursevi.xml";

        public static List<Kurs> LoadKursevi()
        {
            if (!File.Exists(filePath))
                return new List<Kurs>();

            XmlSerializer serializer = new XmlSerializer(typeof(List<Kurs>));
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                return (List<Kurs>)serializer.Deserialize(fs);
            }
        }

        public static void SaveKursevi(List<Kurs> kursevi)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Kurs>));
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(fs, kursevi);
            }
        }
    }
}
