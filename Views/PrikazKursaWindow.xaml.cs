using IUiUS_Projekat.Models;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace IUiUS_Projekat.Views
{
    public partial class PrikazKursaWindow : Window
    {
        public PrikazKursaWindow(Kurs kurs)
        {
            InitializeComponent();
            DataContext = kurs;

            if (File.Exists(kurs.OpisRtfPath))
            {
                using (FileStream fs = new FileStream(kurs.OpisRtfPath, FileMode.Open))
                {
                    TextRange range = new TextRange(OpisBox.Document.ContentStart, OpisBox.Document.ContentEnd);
                    range.Load(fs, System.Windows.DataFormats.Rtf);
                }
            }
        }

        private void Zatvori_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
