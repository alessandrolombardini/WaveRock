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
    /// Logica di interazione per IscrizioneCorso.xaml
    /// </summary>
    public partial class IscrizioneCorso : Page
    {
        DataClassesWRDataContext db = new DataClassesWRDataContext();

        public IscrizioneCorso()
        {
            InitializeComponent();
        }

        private void Btt_back_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Accesso();
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
                    var corsi = from EDIZIONE_CORSO in db.EDIZIONE_CORSO
                                join ISCRIZIONE in db.ISCRIZIONE on EDIZIONE_CORSO.IDEdizioneCorso equals ISCRIZIONE.IDEdizioneCorso into ISCRIZIONE_join
                                from ISCRIZIONE in ISCRIZIONE_join.DefaultIfEmpty()
                                where EDIZIONE_CORSO.DataInizio > DateTime.Now
                                group new { EDIZIONE_CORSO, ISCRIZIONE } by new
                                {
                                    EDIZIONE_CORSO.IDEdizioneCorso,
                                    EDIZIONE_CORSO.NumeroPartecipanti,
                                    EDIZIONE_CORSO.DataInizio
                                } into g
                                where g.Count(p => p.ISCRIZIONE.DataPagamento != null) < g.Key.NumeroPartecipanti
                                select new
                                {
                                    IDEdizioneCorso = (int?)g.Key.IDEdizioneCorso,
                                    DataInizio = g.Key.DataInizio,
                                    ISCRITTI = g.Count(p => p.ISCRIZIONE.DataPagamento != null),
                                    g.Key.NumeroPartecipanti
                                };

                    var edizioniCorsi = from c in corsi
                                        join edizione in db.EDIZIONE_CORSO on c.IDEdizioneCorso equals edizione.IDEdizioneCorso
                                        where !(from c in db.ISCRIZIONE
                                                join p in db.PERSONA on c.IDPersona equals p.IDPersona
                                                where p.IDPersona == matricola
                                                select c.IDEdizioneCorso
                                        ).Contains(edizione.IDEdizioneCorso)
                                        select new { c.IDEdizioneCorso, c.DataInizio, edizione.IDCorso };
                    var corsiFinale = from c in edizioniCorsi
                                      join corso in db.CORSO on c.IDCorso equals corso.IDCorso
                                      select new { c.IDEdizioneCorso, NomeEData = corso.Nome + " (Data inizio: "+ c.DataInizio+")" };
                    combo_corso.ItemsSource = corsiFinale.ToList();
                    checkbox_trovata.IsChecked = true;
                }
            }
        }

        private void Btt_acquista_Click(object sender, RoutedEventArgs e)
        {
            if(combo_corso.SelectedItem is null)
            {
                MessageBox.Show("Valori d'iscrizione non validi", "Alert", MessageBoxButton.OK);
            }
            else
            {
                ISCRIZIONE iscrizione = new ISCRIZIONE();
                iscrizione.IDPersona = Int32.Parse(txt_matricola.Text);
                iscrizione.IDEdizioneCorso = (int)combo_corso.SelectedValue;
                iscrizione.DataPagamento = DateTime.Now;
                db.ISCRIZIONE.InsertOnSubmit(iscrizione);
                // Aggiorno il numero di iscritti
                var modificare = (from mod in db.EDIZIONE_CORSO
                                 where mod.IDEdizioneCorso == (int)combo_corso.SelectedValue
                                 select new {mod}).First();
                modificare.mod.NumeroIscritti = modificare.mod.NumeroIscritti + 1;
                db.SubmitChanges();
                MessageBox.Show("Iscrizione completata con successo", "Completato", MessageBoxButton.OK);
                reset();
            }
        }

        private void reset()
        {
            combo_corso.SelectedItem = null;
            combo_corso.ItemsSource = null;
            txt_matricola.Text = String.Empty;
            checkbox_trovata.IsChecked = false;
        }

        private void Combo_corso_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            double prezzo = 0;
            if (!(combo_corso.SelectedItem is null))
            {
                prezzo =  (from edizione in db.EDIZIONE_CORSO
                           join corso in db.CORSO on edizione.IDCorso equals corso.IDCorso
                           where edizione.IDEdizioneCorso == Int32.Parse(combo_corso.SelectedValue.ToString())
                           select new { corso.Prezzo }).First().Prezzo;
            }
            label_totale.Content = prezzo;
        }

        private void Txt_matricola_TextChanged(object sender, TextChangedEventArgs e)
        {
            combo_corso.SelectedItem = null;
            combo_corso.ItemsSource = null;
            checkbox_trovata.IsChecked = false;
        }
    }
}
