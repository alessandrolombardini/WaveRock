using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
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
    /// Logica di interazione per Statistiche.xaml
    /// </summary>
    public partial class Statistiche : Page
    {
        private static readonly string connessione = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\WaveRockDatabase.mdf;Integrated Security=True;";

        public Statistiche()
        {
            InitializeComponent();

            IDictionary<int, String> map = new Dictionary<int,String>();
            map.Add(1, "Tassi di partecipazione alle diverse tipologie di corso");
            map.Add(2, "Per ciascuna tipologia di abbonamento le vendite dell'ultimo mese");
            map.Add(3, "Per ciascuna fascia di età il numero di biglietti acquistati");
            map.Add(4, "Per ciascun cliente le spese effettuate nell'ultimo anno");
            combo_scelta.ItemsSource = map;
            combo_scelta.DisplayMemberPath = "Value";
            combo_scelta.SelectedValuePath = "Key";
        }

        private void Btt_back_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Accesso();
        }

        private void Combo_scelta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Int32.Parse(combo_scelta.SelectedValue.ToString()))
            {
                case 1:
                    opPartecipazioniEIncassiEdizioneCorso();
                    break;
                case 2:
                    opIncassoAbbonamenti();
                    break;
                case 3:
                    opBigliettiPerFasciaDiEta();
                    break;
                case 4:
                    opSpesaClienti();
                    break;
            }
        }

        private void opPartecipazioniEIncassiEdizioneCorso()
        {
            string stringaComando = @"select CORSO.Nome, 
                                    COUNT(DISTINCT(EDIZIONE_CORSO.IDEdizioneCorso)) as NumeroEdizioni, 
                                    round(cast(sum(EDIZIONE_CORSO.NumeroIscritti) as float) / cast(sum(EDIZIONE_CORSO.NumeroPartecipanti) as float),2) as RateoPartecipazione, 
                                    (sum(EDIZIONE_CORSO.NumeroIscritti) * CORSO.Prezzo)/COUNT(DISTINCT(EDIZIONE_CORSO.IDEdizioneCorso)) as IncassoMedioEdizione, 
                                    sum(EDIZIONE_CORSO.NumeroIscritti) as IscrittiTotali,
                                    sum(NumeroPartecipanti) as TotalePosti
                                    from CORSO join EDIZIONE_CORSO on EDIZIONE_CORSO.IDCorso = CORSO.IDCorso 
                                    where EDIZIONE_CORSO.DataInizio < GETDATE() 
                                    group by CORSO.IDCorso, CORSO.Nome, Corso.Prezzo, EDIZIONE_CORSO.NumeroIscritti
                                    order by RateoPartecipazione ASC";
            SqlDataAdapter da = new SqlDataAdapter(stringaComando, connessione);
            DataSet ds = new DataSet();
            da.Fill(ds);
            datagrid_risultato.ItemsSource = ds.Tables[0].DefaultView;
        }

        private void opIncassoAbbonamenti()
        {
            string stringaComando = @"select TIPO_ABBONAMENTO.IDTipoAbbonamento,FASCIA_ETA.InizioEta, FASCIA_ETA.FineEta, TIPO_ABBONAMENTO.MesiDurata, TIPO_ABBONAMENTO.Prezzo as PrezzoSingolo, 
	                                   COUNT(*) as NumeroAcquisti, SUM(Prezzo) as TotaleIncasso
                                       from ABBONAMENTO join TIPO_ABBONAMENTO on ABBONAMENTO.IDTipoAbbonamento = TIPO_ABBONAMENTO.IDTipoAbbonamento
	                                   join FASCIA_ETA on FASCIA_ETA.IDFasciaEta = TIPO_ABBONAMENTO.IDFasciaEta
                                       where DATEDIFF(day,GETDATE(), ABBONAMENTO.DataOraPagamento) < 30
                                       group by TIPO_ABBONAMENTO.IDTipoAbbonamento,TIPO_ABBONAMENTO.MesiDurata, FASCIA_ETA.InizioEta, FASCIA_ETA.FineEta, TIPO_ABBONAMENTO.Prezzo
                                       order by TotaleIncasso desc";
            SqlDataAdapter da = new SqlDataAdapter(stringaComando, connessione);
            DataSet ds = new DataSet();
            da.Fill(ds);
            datagrid_risultato.ItemsSource = ds.Tables[0].DefaultView;
        }

        private void opBigliettiPerFasciaDiEta()
        {
            string stringaComando = @"select FASCIA_ETA.IDFasciaEta, FASCIA_ETA.InizioEta, FASCIA_ETA.FineEta,COUNT(BIGLIETTO.IDBIGLIETTO) as BigliettiAcquistati
                                    from BIGLIETTO join TIPO_BIGLIETTO on BIGLIETTO.IDTipoBiglietto = TIPO_BIGLIETTO.IDTipoBiglietto
                                    right join FASCIA_ETA on TIPO_BIGLIETTO.IDTipoBiglietto = FASCIA_ETA.IDFasciaEta
                                    group by FASCIA_ETA.IDFasciaEta, FASCIA_ETA.InizioEta, FASCIA_ETA.FineEta
                                    order by BigliettiAcquistati desc";
            SqlDataAdapter da = new SqlDataAdapter(stringaComando, connessione);
            DataSet ds = new DataSet();
            da.Fill(ds);
            datagrid_risultato.ItemsSource = ds.Tables[0].DefaultView;
        }

        private void opSpesaClienti()
        {
            string stringaComando = @"select PERSONA.IDPersona, PERSONA.Nome, PERSONA.Cognome,  IsNull(Tessere.SpesaTessere,0) as SpesaTessere, IsNull(Biglietti.SpesaBiglietti,0) as SpesaBiglietti, 
                                            IsNull(Abbonamenti.SpesaAbbonamenti,0) as SpesaAbbonamenti, IsNull(Corsi.SpesaCorsi,0) as SpesaCorsi,
                                            IsNull(Tessere.SpesaTessere,0) + IsNull(Biglietti.SpesaBiglietti,0) + IsNull(Abbonamenti.SpesaAbbonamenti,0) + IsNull(Corsi.SpesaCorsi,0) as SpesaTotale
                                    from PERSONA left join 
                                    (select ACQUISTO.IDPersona, sum(TIPO_TESSERA.Prezzo) as SpesaTessere
                                    from ACQUISTO join TESSERA on ACQUISTO.IDAcquisto = TESSERA.IDAcquisto
	                                        join TIPO_TESSERA on TESSERA.IDTipoTessera = TIPO_TESSERA.IDTipoTessera and TESSERA.NumeroBiglietti = TIPO_TESSERA.NumeroBiglietti
                                    where ACQUISTO.DataOraAcquisto > dateadd(YEAR, -1,GETDATE())
                                    group by ACQUISTO.IDPersona) as Tessere
                                    on PERSONA.IDPersona = Tessere.IDPersona
                                    left join 
                                    (select ACQUISTO.IDPersona, sum(TIPO_BIGLIETTO.Prezzo) as SpesaBiglietti
                                    from ACQUISTO join BIGLIETTO on ACQUISTO.IDAcquisto = BIGLIETTO.IDAcquisto
	                                        join TIPO_BIGLIETTO on BIGLIETTO.IDTipoBiglietto = TIPO_BIGLIETTO.IDTipoBiglietto
                                    where ACQUISTO.DataOraAcquisto > dateadd(YEAR, -1,GETDATE())
                                    group by ACQUISTO.IDPersona) as Biglietti
                                    on PERSONA.IDPersona = Biglietti.IDPersona
                                    left join 
                                    (select ABBONAMENTO.IDPersona, sum(TIPO_ABBONAMENTO.Prezzo) as SpesaAbbonamenti
                                    from ABBONAMENTO join TIPO_ABBONAMENTO on ABBONAMENTO.IDTipoAbbonamento = TIPO_ABBONAMENTO.IDTipoAbbonamento
                                    where ABBONAMENTO.DataOraPagamento > dateadd(YEAR, -1,GETDATE())
                                    group by ABBONAMENTO.IDPersona) as Abbonamenti
                                    on PERSONA.IDPersona = Abbonamenti.IDPersona
                                    left join
                                    (select ISCRIZIONE.IDPersona, sum(CORSO.Prezzo) as SpesaCorsi
                                    from CORSO join EDIZIONE_CORSO on EDIZIONE_CORSO.IDCorso = CORSO.IDCorso
                                    join ISCRIZIONE on ISCRIZIONE.IDEdizioneCorso = EDIZIONE_CORSO.IDEdizioneCorso
                                    where ISCRIZIONE.DataPagamento > dateadd(YEAR, -1,GETDATE())
                                    group by ISCRIZIONE.IDPersona) as Corsi
                                    on PERSONA.IDPersona = Corsi.IDPersona
                                    order by SpesaTotale desc";
            SqlDataAdapter da = new SqlDataAdapter(stringaComando, connessione);
            DataSet ds = new DataSet();
            da.Fill(ds);
            datagrid_risultato.ItemsSource = ds.Tables[0].DefaultView;
        }

        private void Datagrid_risultato_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
