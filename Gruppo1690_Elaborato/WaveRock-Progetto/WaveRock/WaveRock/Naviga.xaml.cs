using System;
using System.Collections.Generic;
using System.Data;
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
    /// Logica di interazione per Naviga.xaml
    /// </summary>
    public partial class Naviga : Page
    {
        private static readonly string connessione = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\WaveRockDatabase.mdf;Integrated Security=True;";

        public Naviga()
        {
            InitializeComponent();

            IDictionary<int, String> map = new Dictionary<int, String>();
            map.Add(1, "Lista delle persone registrate");
            map.Add(2, "Lista dei corsi attivi");
            map.Add(3, "Lista completa di tutte le lezioni in programma");
            map.Add(4, "Per ciascun cliente il numero di ingressi disponibili");
            map.Add(5, "Per ciascun cliente la lista degli abbonamenti attivi");
            map.Add(6, "Per ciascun cliente le lezioni programmate a cui dovrà partecipare");
            map.Add(7, "Lista accessi effettuati");
            map.Add(8, "Per ciascun corso attivo la lista dei partecipanti");
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
                    opMostraPersone();
                    break;
                case 2:
                    opMostraCorsiAttivi();
                    break;
                case 3:
                    opMostraLezionDaFare();
                    break;
                case 4:
                    opAccessiRimasti();
                    break;
                case 5:
                    opAbbonamentiAttivi();
                    break;
                case 6:
                    opLezioniRimastePerCliente();
                    break;
                case 7:
                    opListaAccessiEffettuati();
                    break;
                case 8:
                    opPartecipantiACorso();
                    break;
            }
        }

        private void opMostraPersone()
        {
            string stringaComando = @"select PERSONA.IDPersona, PERSONA.Nome, PERSONA.Cognome, 
                                    CONVERT(VARCHAR(10), PERSONA.DataNascita, 111) as DataNascita,
                                    PERSONA.Indirizzo, PERSONA.Cellulare, PERSONA.Istruttore
                                    from PERSONA";
            execute(stringaComando);
        }

        private void opMostraCorsiAttivi()
        {
            string stringaComando = @"select EDIZIONE_CORSO.IDEdizioneCorso, CONVERT(VARCHAR(10), EDIZIONE_CORSO.DataInizio, 111) as DataInizio, EDIZIONE_CORSO.NumeroPartecipanti as MassimiPartecipanti, EDIZIONE_CORSO.NumeroIscritti, CORSO.Nome, COUNT(LEZIONE.DataOra) as LezioniRimaste, PERSONA.Cognome as Istruttore
                                    from EDIZIONE_CORSO left join LEZIONE on EDIZIONE_CORSO.IDEdizioneCorso = LEZIONE.IDEdizioneCorso 
                                    join PERSONA on EDIZIONE_CORSO.IDPersona = PERSONA.IDPersona
                                    join CORSO on EDIZIONE_CORSO.IDCorso = CORSO.IDCorso
                                    and (LEZIONE.DataOra > GETDATE() or EDIZIONE_CORSO.DataInizio > GETDATE())
                                    group by EDIZIONE_CORSO.IDEdizioneCorso, EDIZIONE_CORSO.DataInizio, EDIZIONE_CORSO.NumeroPartecipanti, EDIZIONE_CORSO.NumeroIscritti, CORSO.Nome, PERSONA.Cognome";
            execute(stringaComando);
        }
        
        private void opMostraLezionDaFare()
        {
            string stringaComando = @"select CORSO.IDCorso, CORSO.Nome,CONVERT(VARCHAR(10), EDIZIONE_CORSO.DataInizio, 111) as DataInizio, LEZIONE.DataOra, LEZIONE.Durata, LUOGO.Nome as Luogo
                                      from LEZIONE,EDIZIONE_CORSO,CORSO,LUOGO
                                      where LEZIONE.IDEdizioneCorso = EDIZIONE_CORSO.IDEdizioneCorso
                                      and EDIZIONE_CORSO.IDCorso = CORSO.IDCorso
                                      and LEZIONE.IDLuogo = LUOGO.IDLuogo
                                      and LEZIONE.DataOra > GETDATE()";
            execute(stringaComando);
        }

        private void opAccessiRimasti()
        {
            string stringaComando = @"select UsoTessere.IDPersona, UsoTessere.Nome, UsoTessere.Cognome, IsNull(sum(UsoTessere.NumeroBiglietti * UsoTessere.NumeroPerTipo) - sum(UsoTessere.AccessiEseguiti),0) as AccessiRimastiTessere,
	                                        Biglietti.BigliettiRimasti, IsNUll(sum(UsoTessere.NumeroBiglietti * UsoTessere.NumeroPerTipo) - sum(UsoTessere.AccessiEseguiti),0) +
	                                        Biglietti.BigliettiRimasti as AccessiTotali
                                    from (
	                                    select PERSONA.IDPersona, PERSONA.Nome, PERSONA.Cognome, TESSERA.IDTipoTessera, TESSERA.NumeroBiglietti, 
	                                    COUNT(TESSERA.IDTessera) as NumeroPerTipo, COUNT(USO_TESSERA.DataUso) as AccessiEseguiti
	                                    from ACQUISTO join TESSERA on ACQUISTO.IDAcquisto = TESSERA.IDAcquisto
	                                    left join USO_TESSERA on TESSERA.IDTessera = USO_TESSERA.IDTessera
	                                    join TIPO_TESSERA on (TESSERA.NumeroBiglietti = TIPO_TESSERA.NumeroBiglietti 
	                                    and TESSERA.IDTipoTessera = TIPO_TESSERA.IDTipoTessera)
	                                    right join PERSONA on PERSONA.IDPersona = ACQUISTO.IDPersona
	                                    group by PERSONA.IDPersona, PERSONA.Nome, PERSONA.Cognome, TESSERA.IDTipoTessera, TESSERA.NumeroBiglietti
                                    ) as UsoTessere
                                    join 
                                    (
                                    select PERSONA.IDPersona, PERSONA.Nome, PERSONA.Cognome, COUNT(BIGLIETTO.IDBiglietto) - COUNT(BIGLIETTO.DataUso) as BigliettiRimasti
                                    from ACQUISTO join BIGLIETTO on ACQUISTO.IDAcquisto = BIGLIETTO.IDAcquisto
                                    right join PERSONA on ACQUISTO.IDPersona = PERSONA.IDPersona
                                    group by PERSONA.IDPersona, PERSONA.Nome, PERSONA.Cognome
                                    ) as Biglietti
                                    on UsoTessere.IDPersona = Biglietti.IDPersona
                                    group by UsoTessere.IDPersona, Biglietti.BigliettiRimasti,UsoTessere.Nome, UsoTessere.Cognome
                                    order by UsoTessere.IDPersona asc";
            execute(stringaComando);
        }

        private void opAbbonamentiAttivi()
        {
            string stringaComando = @"select PERSONA.IDPersona, PERSONA.Nome, PERSONA.Cognome, CONVERT(VARCHAR(10), ABBONAMENTO.DataInizio, 111) as DataInizio, 
                                    CONVERT(VARCHAR(10), dateadd(DAY, TIPO_ABBONAMENTO.MesiDurata*30,ABBONAMENTO.DataInizio), 111) as DataFine
                                    from PERSONA left join ABBONAMENTO on PERSONA.IDPersona = ABBONAMENTO.IDPersona
                                    join TIPO_ABBONAMENTO on ABBONAMENTO.IDTipoAbbonamento = TIPO_ABBONAMENTO.IDTipoAbbonamento
                                    where dateadd(DAY, TIPO_ABBONAMENTO.MesiDurata*30,ABBONAMENTO.DataInizio) > GETDATE()
                                    order by PERSONA.IDPersona asc";
            execute(stringaComando);
        }

        private void opLezioniRimastePerCliente()
        {
            string stringaComando = @"select PERSONA.IDPersona, PERSONA.Nome, PERSONA.Cognome, CORSO.Nome as TipologiaCorso, LEZIONE.DataOra, LEZIONE.Durata as Durata, LUOGO.Nome as Luogo
                                    from PERSONA join ISCRIZIONE on PERSONA.IDPersona = ISCRIZIONE.IDPersona
                                    join EDIZIONE_CORSO on ISCRIZIONE.IDEdizioneCorso = EDIZIONE_CORSO.IDEdizioneCorso
                                    join CORSO on EDIZIONE_CORSO.IDCorso = CORSO.IDCorso
                                    join LEZIONE on EDIZIONE_CORSO.IDEdizioneCorso = LEZIONE.IDEdizioneCorso
                                    join LUOGO on LEZIONE.IDLuogo = LUOGO.IDLuogo
                                    where LEZIONE.DataOra > GETDATE()
                                    order by PERSONA.IDPersona asc";
            execute(stringaComando);
        }

        private void opListaAccessiEffettuati()
        {
            string stringaComando = @"select PERSONA.IDPersona,PERSONA.Nome, PERSONA.Cognome, ACCESSO.DataOra
                                    from ACCESSO, PERSONA
                                    where ACCESSO.IDPersona = PERSONA.IDPersona
                                    order by ACCESSO.DataOra desc";
            execute(stringaComando);
        }
        private void opPartecipantiACorso()
        {
            string stringaComando = @"select EDIZIONE_CORSO.IDEdizioneCorso, CORSO.Nome, CONVERT(VARCHAR(10), EDIZIONE_CORSO.DataInizio, 111) as DataInizio, 
                                    PERSONA.IDPersona, PERSONA.Nome as NomePartecipante, PERSONA.Cognome as CognomePartecipante
                                    from PERSONA join ISCRIZIONE on PERSONA.IDPersona = ISCRIZIONE.IDPersona
                                    join EDIZIONE_CORSO on ISCRIZIONE.IDEdizioneCorso = EDIZIONE_CORSO.IDEdizioneCorso
                                    join CORSO on EDIZIONE_CORSO.IDCorso = CORSO.IDCorso
                                    where EDIZIONE_CORSO.DataInizio > GETDATE() or
                                    EDIZIONE_CORSO.IDEdizioneCorso in (
	                                    select EDIZIONE_CORSO.IDEdizioneCorso
	                                    from EDIZIONE_CORSO join LEZIONE on EDIZIONE_CORSO.IDEdizioneCorso = LEZIONE.IDEdizioneCorso	
	                                    where LEZIONE.DataOra > GETDATE()
	                                    group by EDIZIONE_CORSO.IDEdizioneCorso
                                    )
                                    order by EDIZIONE_CORSO.IDEdizioneCorso asc";
            execute(stringaComando);
        }

        private void execute(string query)
        {
            SqlDataAdapter da = new SqlDataAdapter(query, connessione);
            DataSet ds = new DataSet();
            da.Fill(ds);
            datagrid_risultato.ItemsSource = ds.Tables[0].DefaultView;
        }

    }
}
