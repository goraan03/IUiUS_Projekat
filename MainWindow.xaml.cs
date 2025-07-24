using IUiUS_Projekat.Models;
using IUiUS_Projekat.Services;
using IUiUS_Projekat.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace IUiUS_Projekat
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<KursViewModel> kurseviVM;
        private User _loggedInUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _loggedInUser = user;
            LoadData();
            this.Title = $"Dobrodošao, {user.Username} ({user.Role})";

            if (_loggedInUser.Role == UserRole.Visitor)
            {
                DodajButton.Visibility = Visibility.Collapsed;
                ObrisiButton.Visibility = Visibility.Collapsed;
                SelectAllBox.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadData()
        {
            var kursevi = KursService.LoadKursevi();
            kurseviVM = new ObservableCollection<KursViewModel>(
                kursevi.Select(k => new KursViewModel
                {
                    Naziv = k.Naziv,
                    Cena = k.Cena,
                    SlikaPath = k.SlikaPath,
                    OpisRtfPath = k.OpisRtfPath,
                    DatumDodavanja = k.DatumDodavanja,
                    IsSelected = false
                }));
            KurseviGrid.ItemsSource = kurseviVM;
        }

        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var kurs in kurseviVM)
                kurs.IsSelected = true;
            KurseviGrid.Items.Refresh();
        }

        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var kurs in kurseviVM)
                kurs.IsSelected = false;
            KurseviGrid.Items.Refresh();
        }

        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
            var prozor = new DodajKursWindow();
            if (prozor.ShowDialog() == true)
            {
                KursService.DodajKurs(prozor.NoviKurs);
                LoadData();
            }
        }

        private void Izlaz_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
            this.Close();
        }


        private void Obrisi_Click(object sender, RoutedEventArgs e)
        {
            var zaBrisanje = kurseviVM.Where(k => k.IsSelected).ToList();

            if (!zaBrisanje.Any())
            {
                System.Windows.MessageBox.Show("Niste izabrali nijedan kurs za brisanje.");
                return;
            }

            var rezultat = System.Windows.MessageBox.Show("Da li ste sigurni da želite da obrišete izabrane kurseve?", "Potvrda", MessageBoxButton.YesNo);
            if (rezultat == MessageBoxResult.Yes)
            {
                var svi = KursService.LoadKursevi();
                foreach (var k in zaBrisanje)
                {
                    var kurs = svi.FirstOrDefault(x => x.Naziv == k.Naziv && x.OpisRtfPath == k.OpisRtfPath);
                    if (kurs != null)
                        svi.Remove(kurs);
                }
                KursService.SaveKursevi(svi);
                LoadData();
            }
        }

        private void Naziv_Click(object sender, RoutedEventArgs e)
        {
            var link = (Hyperlink)sender;
            var kursVM = (KursViewModel)((TextBlock)link.Parent).DataContext;

            var kurs = new Kurs
            {
                Naziv = kursVM.Naziv,
                Cena = kursVM.Cena,
                SlikaPath = kursVM.SlikaPath,
                OpisRtfPath = kursVM.OpisRtfPath,
                DatumDodavanja = kursVM.DatumDodavanja
            };

            if (_loggedInUser.Role == UserRole.Admin)
            {
                var izmenaWindow = new IzmeniKursWindow(kurs);
                if (izmenaWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                var prikazWindow = new PrikazKursaWindow(kurs);
                prikazWindow.ShowDialog();
            }
        }
    }

    public class KursViewModel : Kurs
    {
        public bool IsSelected { get; set; }
    }
}
