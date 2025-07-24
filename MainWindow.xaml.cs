using IUiUS_Projekat.Models;
using IUiUS_Projekat.Services;
using IUiUS_Projekat.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

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
            this.Title = $"elcome, {user.Username} ({user.Role})";

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

            foreach (var kvm in kurseviVM)
            {
                kvm.PropertyChanged += KursViewModel_PropertyChanged;
            }

            KurseviGrid.ItemsSource = kurseviVM;
        }

        private void KursViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(KursViewModel.IsSelected))
            {
                KurseviGrid.Items.Refresh();
            }
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
                System.Windows.MessageBox.Show("No course is choosen for deleting.");
                return;
            }

            var rezultat = System.Windows.MessageBox.Show(
            "Are you sure you want to delete selected courses?",
            "Aproval",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }
    }

    public class KursViewModel : Kurs, INotifyPropertyChanged
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
