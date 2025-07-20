using IUiUS_Projekat.Models;
using IUiUS_Projekat.Services;
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

        public MainWindow(User user)
        {
            InitializeComponent();
            LoadData();
            this.Title = $"Dobrodošao, {user.Username} ({user.Role})";
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
        }
    }

    public class KursViewModel : Kurs
    {
        public bool IsSelected { get; set; }
    }
}
