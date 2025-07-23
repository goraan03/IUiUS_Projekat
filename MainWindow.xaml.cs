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

            // Sakrij dugmad ako nije admin
            if (_loggedInUser.Role != UserRole.Admin)
            {
                DodajBtn.Visibility = Visibility.Collapsed;
                ObrisiBtn.Visibility = Visibility.Collapsed;
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

        private void Naziv_Click(object sender, RoutedEventArgs e)
        {
            var link = (Hyperlink)sender;
            var kurs = (KursViewModel)((TextBlock)link.Parent).DataContext;

            MessageBox.Show($"Kliknuto na: {kurs.Naziv}");
            // Ovde će kasnije ići otvaranje prozora za prikaz ili izmenu
        }

        private void DodajBtn_Click(object sender, RoutedEventArgs e)
        {
            var prozor = new DodajKursWindow();
            if (prozor.ShowDialog() == true)
            {
                var kursevi = KursService.LoadKursevi();
                kursevi.Add(prozor.NoviKurs);
                KursService.SaveKursevi(kursevi);
                LoadData(); // osvežavanje prikaza
            }
        }

        private void ObrisiBtn_Click(object sender, RoutedEventArgs e)
        {
            var selektovani = kurseviVM.Where(k => k.IsSelected).ToList();
            if (selektovani.Count == 0)
            {
                MessageBox.Show("Nijedan kurs nije označen.");
                return;
            }

            var potvrda = MessageBox.Show("Da li ste sigurni da želite da obrišete označene kurseve?", "Potvrda", MessageBoxButton.YesNo);
            if (potvrda == MessageBoxResult.Yes)
            {
                var svi = KursService.LoadKursevi();
                var preostali = svi.Where(k => !selektovani.Any(s => s.Naziv == k.Naziv && s.Cena == k.Cena)).ToList();
                KursService.SaveKursevi(preostali);
                LoadData();
            }
        }

        private void IzlazBtn_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }

    public class KursViewModel : Kurs
    {
        public bool IsSelected { get; set; }
    }
}
