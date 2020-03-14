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
    /// Logica di interazione per SottoscrizioneAbbonamenti.xaml
    /// </summary>
    public partial class SottoscrizioneAbbonamenti : Page
    {

        DataClassesWRDataContext db = new DataClassesWRDataContext();

        public SottoscrizioneAbbonamenti()
        {
            InitializeComponent();
        }

        private void Btt_back_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Accesso();
        }

        private void Btt_acquista_Click(object sender, RoutedEventArgs e)
        {
            if(combo_durata.SelectedItem is null || !date_data.SelectedDate.HasValue || checkbox_trovata.IsChecked == false)
            {
                MessageBox.Show("Valori d'acquisto non validi", "Alert", MessageBoxButton.OK);
            }
            else
            {
                // Aggiungo l'abbonamento
                ABBONAMENTO abbonamento = new ABBONAMENTO();
                abbonamento.DataInizio = date_data.SelectedDate.Value;
                abbonamento.IDPersona = int.Parse(txt_matricola.Text);
                abbonamento.IDTipoAbbonamento = (int)combo_durata.SelectedValue;
                abbonamento.DataOraPagamento = DateTime.Now;
                db.ABBONAMENTO.InsertOnSubmit(abbonamento);
                db.SubmitChanges();

                MessageBox.Show("Abbonamento completato con successo", "Completato", MessageBoxButton.OK);
                reset();
            }
        }

        private void reset()
        {
            txt_matricola.Text = String.Empty;
            checkbox_trovata.IsChecked = false;
            combo_durata.SelectedItem = null;
            date_data.SelectedDate = null;
            date_data.BlackoutDates.Clear();
            combo_durata.ItemsSource = null;
        }

        private void Btt_cercaMatricola_Click(object sender, RoutedEventArgs e)
        {
            int matricola;
            if (!int.TryParse(txt_matricola.Text, out matricola))
            {
                MessageBox.Show("Valore matricola non valido", "Alert", MessageBoxButton.OK);
            }
            else
            {
                var persone = from persona in db.PERSONA
                              where persona.IDPersona == matricola
                              select persona;
                if (persone.Count() <= 0)
                {
                    MessageBox.Show("Matricola non trovata", "Alert", MessageBoxButton.OK);
                }
                else
                {
                    var persona = persone.First();
                    int fasciaEta = trovaFasciaEta(persona.IDPersona);
                    var tipiAbbonamento = from abbonamenti in db.TIPO_ABBONAMENTO
                                          where abbonamenti.IDFasciaEta==fasciaEta
                                          select new { MesiDurataEPrezzo = abbonamenti.MesiDurata+" mesi (Prezzo: "+abbonamenti.Prezzo+")", abbonamenti.IDTipoAbbonamento};
                    combo_durata.ItemsSource = tipiAbbonamento.ToList();
                    checkbox_trovata.IsChecked = true;
                }
            }

            // Oscuro le date in cui è già presente un abbonamento
            date_data.DisplayDateStart = DateTime.Now;
            var abbonamentiAttivi = from abbonamenti in db.ABBONAMENTO
                                    join tipoAbbonamenti in db.TIPO_ABBONAMENTO 
                                    on abbonamenti.IDTipoAbbonamento equals tipoAbbonamenti.IDTipoAbbonamento
                                    where abbonamenti.IDPersona == matricola
                                    select new { abbonamenti.DataInizio, tipoAbbonamenti.MesiDurata };
            foreach(var elem in abbonamentiAttivi)
            {
                date_data.BlackoutDates.Add(new CalendarDateRange(elem.DataInizio, elem.DataInizio.AddDays(30 * (int)elem.MesiDurata)));
            }
        }

        private int trovaFasciaEta(int matricola)
        {
            var persona = (from persone in db.PERSONA
                           where persone.IDPersona == matricola
                           select persone).First();
            int eta = anni(persona.DataNascita);
            var fascia = (from fasce in db.FASCIA_ETA
                          where fasce.InizioEta <= eta && fasce.FineEta > eta
                          select fasce).First();
            return fascia.IDFasciaEta;
        }

        private int anni(DateTime dataDiNascita)
        {
            DateTime dob = dataDiNascita;
            DateTime Today = DateTime.Now;
            TimeSpan ts = Today - dob;
            DateTime Age = DateTime.MinValue + ts;
            return Age.Year - 1;
        }

        private void Combo_durata_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            double prezzoAbbonamento = 0;
            if(!(combo_durata.SelectedItem is null))
            {
                prezzoAbbonamento = (from p in db.TIPO_ABBONAMENTO
                                    where p.IDTipoAbbonamento == Int32.Parse(combo_durata.SelectedValue.ToString())
                                    select new { p.Prezzo }).First().Prezzo;
            }
            label_totale.Content = prezzoAbbonamento;
        }

        private void Txt_matricola_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkbox_trovata.IsChecked = false;
            combo_durata.SelectedItem = null;
            date_data.SelectedDate = null;
            date_data.BlackoutDates.Clear();
            combo_durata.ItemsSource = null;
        }
    }
}
