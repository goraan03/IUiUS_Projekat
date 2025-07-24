using IUiUS_Projekat.Models;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Forms;
using IUiUS_Projekat.Services;

namespace IUiUS_Projekat.Views
{
    public partial class IzmeniKursWindow : Window
    {
        private readonly Kurs kurs;
        private string slikaPath;

        public IzmeniKursWindow(Kurs kursZaIzmenu)
        {
            InitializeComponent();
            kurs = kursZaIzmenu;
            slikaPath = kurs.SlikaPath;

            NazivBox.Text = kurs.Naziv;
            CenaBox.Text = kurs.Cena.ToString();
            SlikaPreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(slikaPath));

            if (File.Exists(kurs.OpisRtfPath))
            {
                using (FileStream fs = new FileStream(kurs.OpisRtfPath, FileMode.Open))
                {
                    TextRange textRange = new TextRange(OpisBox.Document.ContentStart, OpisBox.Document.ContentEnd);
                    textRange.Load(fs, System.Windows.DataFormats.Rtf);
                }
            }

            FontFamilyBox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontSizeBox.ItemsSource = new[] { 8, 10, 12, 14, 16, 18, 20, 24 };
            OpisBox.TextChanged += OpisBox_TextChanged;
        }

        private void IzaberiSliku_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "Image files (*.jpg;*.png)|*.jpg;*.png";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                slikaPath = dlg.FileName;
                SlikaPreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(slikaPath));
            }
        }

        private void Sacuvaj_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NazivBox.Text) || !double.TryParse(CenaBox.Text, out double cena))
            {
                System.Windows.MessageBox.Show("Popunite validno naziv i cenu.");
                return;
            }

            // Snimi RTF
            string rtfPutanja = kurs.OpisRtfPath;
            using (FileStream fs = new FileStream(rtfPutanja, FileMode.Create))
            {
                new TextRange(OpisBox.Document.ContentStart, OpisBox.Document.ContentEnd).Save(fs, System.Windows.DataFormats.Rtf);
            }

            kurs.Naziv = NazivBox.Text;
            kurs.Cena = cena;
            kurs.SlikaPath = slikaPath;

            var sviKursevi = KursService.LoadKursevi();
            var indeks = sviKursevi.FindIndex(k => k.Naziv == kurs.Naziv && k.OpisRtfPath == kurs.OpisRtfPath);
            if (indeks >= 0)
            {
                sviKursevi[indeks] = kurs;
                KursService.SaveKursevi(sviKursevi);
            }

            DialogResult = true;
            Close();
        }

        private void Otkazi_Click(object sender, RoutedEventArgs e) => Close();

        private void BoldButton_Click(object sender, RoutedEventArgs e) => EditingCommands.ToggleBold.Execute(null, OpisBox);
        private void ItalicButton_Click(object sender, RoutedEventArgs e) => EditingCommands.ToggleItalic.Execute(null, OpisBox);
        private void UnderlineButton_Click(object sender, RoutedEventArgs e) => EditingCommands.ToggleUnderline.Execute(null, OpisBox);

        private void FontFamilyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyBox.SelectedItem != null)
                OpisBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, FontFamilyBox.SelectedItem);
        }

        private void FontSizeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeBox.SelectedItem != null)
                OpisBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, FontSizeBox.SelectedItem);
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var drawingColor = dlg.Color;
                var mediaColor = System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
                OpisBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(mediaColor));
            }
        }

        private void OpisBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textRange = new TextRange(OpisBox.Document.ContentStart, OpisBox.Document.ContentEnd);
            var text = textRange.Text;
            var wordCount = text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            WordCountText.Text = $"Broj reči: {wordCount}";
        }
    }
}
