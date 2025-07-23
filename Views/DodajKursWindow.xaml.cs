using IUiUS_Projekat.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Forms; // For ColorDialog


namespace IUiUS_Projekat.Views
{
    public partial class DodajKursWindow : Window
    {
        public Kurs NoviKurs { get; private set; }
        private string izabranaSlikaPath;

        public DodajKursWindow()
        {
            InitializeComponent();

            // Popunjavanje fontova i veličina
            FontFamilyBox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontSizeBox.ItemsSource = new[] { 8, 10, 12, 14, 16, 18, 20, 24, 28, 32, 36 };
            OpisBox.TextChanged += OpisBox_TextChanged;
        }

        private void IzaberiSliku_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
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
                System.Windows.MessageBox.Show("Popunite sva obavezna polja.");
                return;
            }

            if (!double.TryParse(CenaBox.Text, out double cena))
            {
                System.Windows.MessageBox.Show("Cena mora biti broj.");
                return;
            }

            string opisRtfPath = $"Opis_{DateTime.Now.Ticks}.rtf";
            using (FileStream fs = new FileStream(opisRtfPath, FileMode.Create))
            {
                OpisBox.Selection.Save(fs, System.Windows.DataFormats.Rtf);
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

        // RichTextBox alati

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleBold.Execute(null, OpisBox);
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleItalic.Execute(null, OpisBox);
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleUnderline.Execute(null, OpisBox);
        }

        private void FontFamilyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyBox.SelectedItem != null)
            {
                OpisBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, FontFamilyBox.SelectedItem);
            }
        }

        private void FontSizeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeBox.SelectedItem != null)
            {
                OpisBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, FontSizeBox.SelectedItem);
            }
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = System.Windows.Media.Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
                OpisBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
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
