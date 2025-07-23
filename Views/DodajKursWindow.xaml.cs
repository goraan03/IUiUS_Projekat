using IUiUS_Projekat.Models;
using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace IUiUS_Projekat.Views
{
    public partial class DodajKursWindow : Window
    {
        public Kurs NoviKurs { get; private set; }

        private string izabranaSlikaPath;

        public DodajKursWindow()
        {
            InitializeComponent();
        }

        private void IzaberiSliku_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            if (dlg.ShowDialog() == true)
            {
                izabranaSlikaPath = dlg.FileName;
                SlikaPreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(izabranaSlikaPath));
            }
        }

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NazivBox.Text) || string.IsNullOrWhiteSpace(CenaBox.Text))
            {
                MessageBox.Show("Popunite sva obavezna polja.");
                return;
            }

            if (!double.TryParse(CenaBox.Text, out double cena))
            {
                MessageBox.Show("Cena mora biti broj.");
                return;
            }

            string opisRtfPath = $"Opis_{DateTime.Now.Ticks}.rtf";
            using (FileStream fs = new FileStream(opisRtfPath, FileMode.Create))
            {
                OpisBox.Selection.Save(fs, DataFormats.Rtf);
            }

            NoviKurs = new Kurs
            {
                Naziv = NazivBox.Text,
                Cena = cena,
                SlikaPath = izabranaSlikaPath,
                OpisRtfPath = opisRtfPath,
                DatumDodavanja = DateTime.Now
            };

            DialogResult = true;
            Close();
        }

        private void Otkazi_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
