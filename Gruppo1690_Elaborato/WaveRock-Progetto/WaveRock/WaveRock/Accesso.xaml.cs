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
    /// Logica di interazione per Accesso.xaml
    /// </summary>
    public partial class Accesso : Page
    {
        public Accesso()
        {
            InitializeComponent();
        }

        DataClassesWRDataContext db = new DataClassesWRDataContext();

        private void Btt_acquistoBiglietti_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Acquisti();
        }

        private void Btt_nuovoAbbonamento_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new SottoscrizioneAbbonamenti();
        }

        /// <summary>
        /// Verifico se la matricola inserita possa accedere per mezzo di un abbonamento attivo, una tessera con ingressi ancora
        /// validi oppure un biglietto non ancora utilizzato.
        /// Devo inoltre verificare che l'orario attuale sia consentito data la sua età.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btt_entra_Click(object sender, RoutedEventArgs e)
        {
            int matricola = Int32.Parse(label_matricola.Content.ToString());
            bool entrato = false;
            if (checkIstruttore(matricola))
            {
                MessageBox.Show("E' un istruttore, può accedere", "Accesso consentito", MessageBoxButton.OK);
                entrato = true;
            }
            else if (checkLezione(matricola))
            {
                MessageBox.Show("Ha una lezione in giornata, può accedere", "Accesso consentito", MessageBoxButton.OK);
                entrato = true;
            }
            else if (!checkOrario(matricola))
            {
                MessageBox.Show("Il cliente non può accedere in questa fascia oraria", "Accesso non consentito", MessageBoxButton.OK);
            }
            else if (checkAbbonamento(matricola))
            {
                MessageBox.Show("Abbonamento attivo", "Accesso consentito", MessageBoxButton.OK);
                entrato = true;
            }
            else if (checkIngressoAttivo(matricola))
            {
                MessageBox.Show("Il cliente ha un ingresso attivo", "Accesso consentito", MessageBoxButton.OK);
                entrato = true;
            }
            else if (checkTessera(matricola))
            {
                MessageBox.Show("Usata tessera", "Accesso consentito", MessageBoxButton.OK);
                entrato = true;
            }
            else if (checkBiglietto(matricola))
            {
                MessageBox.Show("1 biglietto usato", "Accesso consentito", MessageBoxButton.OK);
                entrato = true;
            }
            else
            {
                MessageBox.Show("Il cliente non può accedere, non ha ingressi validi", "Accesso non consentito", MessageBoxButton.OK);
            }
            if (entrato == true)
            {
                ACCESSO accesso = new ACCESSO();
                accesso.DataOra = DateTime.Now;
                accesso.IDPersona = matricola;
                db.ACCESSO.InsertOnSubmit(accesso);
                db.SubmitChanges();
                reset();
            }
        }

        private void reset()
        {
            label_nome.Content = String.Empty;
            label_cognome.Content = String.Empty;
            label_nascita.Content = String.Empty;
            label_matricola.Content = String.Empty;
            label_istruttore.Content = String.Empty;
            txt_matricola.Text = String.Empty;
            btt_entra.IsEnabled = false;
        }

        private bool checkIstruttore(int matricola)
        {
            var personale = from persona in db.PERSONA
                            where persona.IDPersona == matricola
                            && persona.Istruttore == '1'
                            select new { persona };
            if (personale.Count() > 0)
            {
                return true;
            }
            return false;
        }

        private bool checkLezione(int matricola)
        {
            var lezione = from persona in db.PERSONA
                          join iscrizione in db.ISCRIZIONE on persona.IDPersona equals iscrizione.IDPersona
                          join edizione in db.EDIZIONE_CORSO on iscrizione.IDEdizioneCorso equals edizione.IDEdizioneCorso
                          join lez in db.LEZIONE on edizione.IDEdizioneCorso equals lez.IDEdizioneCorso
                          join luogo in db.LUOGO on lez.IDLuogo equals luogo.IDLuogo
                          where lez.DataOra.Date == DateTime.Now.Date
                          && persona.IDPersona == matricola
                          && luogo.Nome == "Palestra"
                          select new { lez };
            return lezione.Count() > 0 ? true : false;
        }
        private bool checkOrario(int matricola)
        {
            var orario = from fasciaEta in db.FASCIA_ETA
                         join ora in db.ORARIO on fasciaEta.IDFasciaEta equals ora.IDFasciaEta
                         join fasciaOraria in db.FASCIA_ORARIA on ora.IDFasciaOraria equals fasciaOraria.IDFasciaOraria
                         where fasciaEta.IDFasciaEta == trovaFasciaEta(matricola)
                         && DateTime.Now.TimeOfDay < fasciaOraria.FineOrario
                         && DateTime.Now.TimeOfDay >= fasciaOraria.InizioOrario
                         select new { fasciaOraria.Nome };
            return orario.Count() > 0 ? true : false;
        }

        private bool checkIngressoAttivo(int matricola)
        {
            var accesso = from accessi in db.ACCESSO
                          where accessi.IDPersona == matricola
                          && accessi.DataOra.Date == DateTime.Now.Date
                          select new { accessi };
            if (accesso.Count() > 0)
            {
                return true;
            }
            return false;
        }

        private bool checkAbbonamento(int matricola)
        {
            var abbonamento = from abbonamenti in db.ABBONAMENTO
                              join tipoAbbonamento in db.TIPO_ABBONAMENTO on abbonamenti.IDTipoAbbonamento equals tipoAbbonamento.IDTipoAbbonamento
                              where abbonamenti.IDPersona == matricola
                              && abbonamenti.DataInizio.Date <= DateTime.Now.Date
                              && DateTime.Now <= abbonamenti.DataInizio.AddDays((int)tipoAbbonamento.MesiDurata * 30).Date
                              select new { abbonamenti.DataInizio };
            return abbonamento.Count() > 0 ? true : false;
        }

        private bool checkTessera(int matricola)
        {
            var tessereConAccessi = from acquisto in db.ACQUISTO
                                    join tessera in db.TESSERA on acquisto.IDAcquisto equals tessera.IDAcquisto
                                    join uso in db.USO_TESSERA on tessera.IDTessera equals uso.IDTessera into tmp
                                    from x in tmp.DefaultIfEmpty()
                                    where acquisto.IDPersona == matricola
                                    select new { tessera.IDTessera, tessera.IDTipoTessera, tessera.NumeroBiglietti, accessi = tmp.Count(k => k.DataUso != null) };
            var tessereDistinteConAccessi = tessereConAccessi.GroupBy(x => x.IDTessera).Select(y => y.First());
            var tessereValide = from tessere in tessereConAccessi
                                join tipoTessera in db.TIPO_TESSERA
                                on new { tessere.IDTipoTessera, tessere.NumeroBiglietti } equals new { tipoTessera.IDTipoTessera, tipoTessera.NumeroBiglietti }
                                where tessere.accessi < tipoTessera.NumeroBiglietti
                                select new { tessere.IDTessera };
            if (tessereValide.Count() > 0)
            {
                USO_TESSERA uso = new USO_TESSERA();
                uso.DataUso = DateTime.Now;
                uso.IDTessera = tessereValide.First().IDTessera;
                db.USO_TESSERA.InsertOnSubmit(uso);
                db.SubmitChanges();
                return true;
            }
            return false;
        }

        private bool checkBiglietto(int matricola)
        {
            var biglietto = from biglietti in db.BIGLIETTO
                            join acquisto in db.ACQUISTO
                            on biglietti.IDAcquisto equals acquisto.IDAcquisto
                            where acquisto.IDPersona == matricola
                            && biglietti.DataUso == null
                            select new { biglietti };
            if (biglietto.Count() > 0)
            {
                biglietto.First().biglietti.DataUso = DateTime.Now;
                db.SubmitChanges();
                return true;
            }
            return false;
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


        private void Btt_cerca_Click(object sender, RoutedEventArgs e)
        {
            int matricola;
            if (!int.TryParse(txt_matricola.Text, out matricola))
            {
                MessageBox.Show("Valore matricola non valido", "Alert", MessageBoxButton.OK);
            }
            else
            {
                var persone = from p in db.PERSONA
                              where p.IDPersona == matricola
                              select p;
                if (persone.Count() <= 0)
                {
                    MessageBox.Show("Matricola non trovata", "Alert", MessageBoxButton.OK);
                    reset();
                }
                else
                {
                    var persona = persone.First();
                    label_nome.Content = persona.Nome;
                    label_cognome.Content = persona.Cognome;
                    label_nascita.Content = persona.DataNascita;
                    label_matricola.Content = persona.IDPersona;
                    label_istruttore.Content = persona.Istruttore == '1' ? "Si" : "No";
                    btt_entra.IsEnabled = true;
                }
            }
        }

        private void Btt_statistiche_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Statistiche();
        }

        private void Btt_iscrizioneCorso_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new IscrizioneCorso();
        }

        private void Btt_naviga_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Naviga();
        }

        private void Btt_aggiungi_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Aggiungi();
        }
    }
}
