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
    /// Logica di interazione per AggiungiLezione.xaml
    /// </summary>
    public partial class AggiungiLezione : Page
    {
        DataClassesWRDataContext db = new DataClassesWRDataContext();

        public AggiungiLezione()
        {
            InitializeComponent();
            var edizioniCorsi = from edizioneCorsi in db.EDIZIONE_CORSO
                                join corsi in db.CORSO on edizioneCorsi.IDCorso equals corsi.IDCorso
                                where edizioneCorsi.DataInizio > DateTime.Now
                                select new { edizioneCorsi.IDEdizioneCorso, NomeEData = corsi.Nome + " (Data inizio: "+edizioneCorsi.DataInizio +")" };
            combo_tipologia.ItemsSource = edizioniCorsi.ToList();
            var luoghi = from l in db.LUOGO
                         select new { l.Nome, l.IDLuogo };
            combo_luogo.ItemsSource = luoghi.ToList();
            combo_ore.ItemsSource = Enumerable.Range(0, 23);
            combo_minuti.ItemsSource = Enumerable.Range(0, 60);
            combo_oraSingola.ItemsSource = Enumerable.Range(8, 14);
            combo_minutoSingola.ItemsSource = Enumerable.Range(0, 60);
        }

        private void Btt_aggiungi_Click(object sender, RoutedEventArgs e)
        {
            if (combo_luogo.SelectedItem is null || combo_tipologia.SelectedItem is null || date_data.SelectedDate is null
                || (combo_ore.SelectedItem is null && combo_minuti.SelectedItem is null)
                || combo_oraSingola.SelectedItem is null
                || (combo_oraSingola.SelectedItem is null && combo_minutoSingola.SelectedItem is null))
            {
                MessageBox.Show("Controllare i campi", "Inserimento non corretto/completo", MessageBoxButton.OK);
            }
            else
            {
                var lezioneSovrapposta = from l in db.LEZIONE
                                         where l.DataOra.Date == date_data.SelectedDate.Value.Date
                                         && l.IDEdizioneCorso == (int)combo_tipologia.SelectedValue
                                         select new { l };
                if (lezioneSovrapposta.Count() > 0)
                {
                    MessageBox.Show("Esiste già una lezione di questo corso in questa data", "Data non valida", MessageBoxButton.OK);
                }
                else
                {
                    
                    LEZIONE lezione = new LEZIONE();
                    lezione.IDLuogo = (int)combo_luogo.SelectedValue;
                    lezione.DataOra = date_data.SelectedDate.Value;
                    int dataMinuti = combo_minutoSingola.SelectedItem is null ? 0 : Int32.Parse(combo_minutoSingola.Text);
                    int dataOra = combo_oraSingola.SelectedItem is null ? 0 : Int32.Parse(combo_oraSingola.Text);
                    lezione.DataOra=lezione.DataOra.AddHours(dataOra);
                    lezione.DataOra=lezione.DataOra.AddMinutes(dataMinuti);
                    lezione.IDEdizioneCorso = (int)combo_tipologia.SelectedValue;
                    int durataOre = combo_ore.SelectedItem is null ? 0 : Int32.Parse(combo_ore.Text);
                    int durataMinuti = combo_minuti.SelectedItem is null ? 0 : Int32.Parse(combo_minuti.Text);
                    lezione.Durata = new TimeSpan(durataOre, durataMinuti, 0);
                    db.LEZIONE.InsertOnSubmit(lezione);
                    db.SubmitChanges();
                    MessageBox.Show("Inserimento completato", "Successo", MessageBoxButton.OK);
                    reset();
                }
            }
        }

        private void reset()
        {
            combo_luogo.SelectedItem = null;
            combo_minuti.SelectedItem = null;
            combo_ore.SelectedItem = null;
            combo_tipologia.SelectedItem = null;
            combo_tipologia.SelectedItem = null;
            combo_minutoSingola.SelectedItem = null;
            combo_oraSingola.SelectedItem = null;
            date_data.SelectedDate = null;
        }

        private void Combo_tipologia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (!(combo_tipologia.SelectedValue is null))
            {
                date_data.BlackoutDates.Clear();
                var inizioData = from c in db.EDIZIONE_CORSO
                                 where c.IDEdizioneCorso == Int32.Parse(combo_tipologia.SelectedValue.ToString())
                                 select new { c.DataInizio };
                date_data.DisplayDateStart = inizioData.First().DataInizio;
                var lezioniPresenti = from c in db.LEZIONE
                                      where c.IDEdizioneCorso == Int32.Parse(combo_tipologia.SelectedValue.ToString())
                                      select new { c.DataOra };
                foreach(var elem in lezioniPresenti)
                {
                    date_data.BlackoutDates.Add(new CalendarDateRange(elem.DataOra.Date, elem.DataOra.Date));
                }
                var istruttore = (from c in db.EDIZIONE_CORSO
                                 where c.IDEdizioneCorso == Int32.Parse(combo_tipologia.SelectedValue.ToString())
                                 select new { c }).First();
                var giorniGiaOccupati = from ed in db.EDIZIONE_CORSO 
                                        join l in db.LEZIONE on ed.IDEdizioneCorso equals l.IDEdizioneCorso
                                        where ed.IDPersona == istruttore.c.IDPersona
                                        && l.DataOra.Date >= inizioData.First().DataInizio
                                        select new { l.DataOra };
                foreach (var elem in giorniGiaOccupati)
                {
                    date_data.BlackoutDates.Add(new CalendarDateRange(elem.DataOra.Date, elem.DataOra.Date));
                }
            }
        }
    }
}
