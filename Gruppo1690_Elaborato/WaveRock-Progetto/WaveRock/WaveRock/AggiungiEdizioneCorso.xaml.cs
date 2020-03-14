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
    /// Logica di interazione per AggiungiEdizioneCorso.xaml
    /// </summary>
    public partial class AggiungiEdizioneCorso : Page
    {
        DataClassesWRDataContext db = new DataClassesWRDataContext();

        public AggiungiEdizioneCorso()
        {
            InitializeComponent();
            var corsi = from c in db.CORSO
                        select new { c.IDCorso, c.Nome, c.Descrizione };
            combo_tipologia.ItemsSource = corsi.ToList();
            var istruttore = from p in db.PERSONA
                             where p.Istruttore == '1'
                             select new { p.IDPersona, p.Nome, p.Cognome };
            combo_istruttore.ItemsSource = istruttore.ToList();
            date_data.DisplayDateStart = DateTime.Now.Date;
            combo_numeroPartecipanti.ItemsSource = Enumerable.Range(1, 100);

        }

        private void Btt_aggiungi_Click(object sender, RoutedEventArgs e)
        {
            if (combo_tipologia.SelectedItem is null || combo_istruttore.SelectedItem is null || 
                !date_data.SelectedDate.HasValue || combo_numeroPartecipanti.SelectedItem is null)
            {
                MessageBox.Show("Controllare i campi", "Inserimento non corretto/completo", MessageBoxButton.OK);
            }
            else
            {
                var esistenza = from c in db.EDIZIONE_CORSO
                                where c.IDCorso == (int)combo_tipologia.SelectedValue
                                && c.DataInizio.Date == date_data.SelectedDate.Value.Date
                                select new { c };
                if (esistenza.Count() > 0)
                {
                    MessageBox.Show("Esiste già un corso uguale in questa data", "Data non valida", MessageBoxButton.OK);
                }
                else
                {
                    EDIZIONE_CORSO ec = new EDIZIONE_CORSO();
                    ec.DataInizio = date_data.SelectedDate.Value;
                    ec.IDCorso = (int)combo_tipologia.SelectedValue;
                    ec.IDPersona = (int)combo_istruttore.SelectedValue;
                    ec.NumeroPartecipanti = (int)combo_numeroPartecipanti.SelectedValue;
                    db.EDIZIONE_CORSO.InsertOnSubmit(ec);
                    db.SubmitChanges();
                    MessageBox.Show("Inserimento completato", "Successo", MessageBoxButton.OK);
                    reset();
                }
            }
        }

        private void reset()
        {
            combo_numeroPartecipanti.SelectedItem = null;
            combo_istruttore.SelectedItem = null;
            combo_tipologia.SelectedItem = null;
            date_data.SelectedDate = null;
        }

        private void Combo_tipologia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(combo_tipologia.SelectedValue is null))
            {
                date_data.BlackoutDates.Clear();
                date_data.DisplayDateStart = DateTime.Now.Date;
                var dateInizioEdizioni = from c in db.EDIZIONE_CORSO
                                         where c.IDCorso == (int)combo_tipologia.SelectedValue
                                         && c.DataInizio > DateTime.Now.Date
                                         select new { c.DataInizio};
                foreach (var elem in dateInizioEdizioni)
                {
                    date_data.BlackoutDates.Add(new CalendarDateRange(elem.DataInizio, elem.DataInizio));
                }
            }
        }
    }
}
