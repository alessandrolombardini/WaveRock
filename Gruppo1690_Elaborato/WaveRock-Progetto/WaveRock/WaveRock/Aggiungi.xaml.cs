using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WaveRock
{
    /// <summary>
    /// Logica di interazione per Aggiungi.xaml
    /// </summary>
    public partial class Aggiungi : Page
    {
        public Aggiungi()
        {
            InitializeComponent();
            frame_content.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        }

        private void Btt_aggiungiPersona_Click(object sender, RoutedEventArgs e)
        {
            frame_content.Navigate(new Uri("AggiungiPersona.xaml", UriKind.Relative));
        }

        private void Btt_aggiungiEdizioneCorso_Click(object sender, RoutedEventArgs e)
        {
            frame_content.Navigate(new Uri("AggiungiEdizioneCorso.xaml", UriKind.Relative));
        }

        private void Btt_aggiungiLezione_Click(object sender, RoutedEventArgs e)
        {
            frame_content.Navigate(new Uri("AggiungiLezione.xaml", UriKind.Relative));
        }

        private void Btt_tornaIndietro_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Accesso();
        }
    }
}
