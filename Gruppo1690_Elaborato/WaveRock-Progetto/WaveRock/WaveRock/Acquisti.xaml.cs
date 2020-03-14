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
    /// Logica di interazione per Acquisti.xaml
    /// </summary>
    public partial class Acquisti : Page
    {

        DataClassesWRDataContext db = new DataClassesWRDataContext();

        public Acquisti()
        {
            InitializeComponent();
        }

        private void Btt_back_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Accesso();
        }

        /// <summary>
        /// Richiesta di esecuzione dell'acquisto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btt_acquista_Click(object sender, RoutedEventArgs e)
        {
            if(combo_quantitaBiglietti.SelectedItem is null && 
                (combo_quantitaTessere.SelectedItem is null || combo_tipoTessere.SelectedItem is null))
            {
                MessageBox.Show("Valori d'acquisto non validi", "Alert", MessageBoxButton.OK);
            }
            else
            {
                // Aggiungo l'acquisto
                ACQUISTO acquisto = new ACQUISTO();
                acquisto.DataOraAcquisto = DateTime.Now;
                acquisto.IDPersona =int.Parse(txt_matricola.Text);
                db.ACQUISTO.InsertOnSubmit(acquisto);
                db.SubmitChanges();

                var persone = from persona in db.ACQUISTO
                              select persona;

                // Ricerco la fascia di eta e aggiungo biglietti e/o tessere
                int fasciaEta = trovaFasciaEta(int.Parse(txt_matricola.Text));

                if (!(combo_quantitaBiglietti.SelectedItem is null))
                {
                    int numeroBiglietti = Int32.Parse(combo_quantitaBiglietti.Text);
                    for (int i = 0; i < numeroBiglietti; i++)
                    {
                        BIGLIETTO biglietto = new BIGLIETTO();
                        biglietto.IDAcquisto = acquisto.IDAcquisto;
                        biglietto.IDTipoBiglietto = fasciaEta;
                        db.BIGLIETTO.InsertOnSubmit(biglietto);
                        db.SubmitChanges();
                    }
                }

                if (!(combo_quantitaTessere.SelectedItem is null || combo_tipoTessere.SelectedItem is null))
                {
                    int numeroTessere = Int32.Parse(combo_quantitaTessere.Text);
                    for (int i = 0; i < numeroTessere; i++)
                    {
                        TESSERA tessera = new TESSERA();
                        tessera.IDAcquisto = acquisto.IDAcquisto;
                        tessera.IDTipoTessera = fasciaEta;
                        tessera.NumeroBiglietti = Int32.Parse(combo_tipoTessere.SelectedValue.ToString());
                        db.TESSERA.InsertOnSubmit(tessera);
                        db.SubmitChanges();
                    }
                }
                MessageBox.Show("Acquisto completato con successo", "Completato", MessageBoxButton.OK);
                reset();
            }
        }

        private void reset()
        {
            txt_matricola.Text = String.Empty;
            checkbox_trovata.IsChecked = false;
            combo_quantitaBiglietti.SelectedItem = null;
            combo_quantitaTessere.SelectedItem = null;
            combo_tipoTessere.SelectedItem = null;
            label_prezzo.Content = String.Empty;
            label_totale.Content = String.Empty;
            label_totaleBiglietti.Content = String.Empty;
            label_totaleTessere.Content = String.Empty;
        }

        /// <summary>
        /// Ricerca la matrica e aggiorna i campi in funzione di ciò che essa può acquistare
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    var tipoTessera = from tessere in db.TIPO_TESSERA
                                      where tessere.IDTipoTessera == fasciaEta
                                      select new { NumeroEPrezzo=tessere.NumeroBiglietti+" (Prezzo: "+tessere.Prezzo+")"      ,tessere.NumeroBiglietti };
                    combo_tipoTessere.ItemsSource = tipoTessera.ToList();
                    combo_quantitaBiglietti.ItemsSource = Enumerable.Range(0, 20);
                    combo_quantitaTessere.ItemsSource = Enumerable.Range(0, 20);
                    var prezzoBiglietto = (from b in db.TIPO_BIGLIETTO
                                          where b.IDTipoBiglietto == trovaFasciaEta(matricola)
                                          select new { b.Prezzo}).First();
                    label_prezzo.Content = prezzoBiglietto.Prezzo;
                    checkbox_trovata.IsChecked = true;
                }
            }
        }

        private int trovaFasciaEta(int matricola)
        {
            var persona = (from persone in db.PERSONA
                          where persone.IDPersona == matricola
                          select persone).First();
            int eta = anni(persona.DataNascita);
            var fascia = (from fasce in db.FASCIA_ETA
                           where fasce.InizioEta<=eta && fasce.FineEta>eta
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

        private void Combo_quantitaBiglietti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ricalcola();
        }

        private void Combo_tipoTessere_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ricalcola();
        }

        private void Combo_quantitaTessere_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ricalcola();
        }

        private void ricalcola()
        {
            if (!txt_matricola.Text.Equals(String.Empty))
            {
                double totaleBiglietti = 0;
                double totaleTessere = 0;

                if (!(combo_quantitaBiglietti.SelectedItem is null))
                {
                    totaleBiglietti = Double.Parse(label_prezzo.Content.ToString()) * Int32.Parse(combo_quantitaBiglietti.SelectedValue.ToString());
                }
                if(!(combo_quantitaTessere.SelectedItem is null || combo_tipoTessere.SelectedItem is null))
                {
                    var prezzoTessera = (from tessera in db.TIPO_TESSERA
                                        where tessera.NumeroBiglietti == Int32.Parse(combo_tipoTessere.SelectedValue.ToString())
                                        && tessera.IDTipoTessera == trovaFasciaEta(Int32.Parse(txt_matricola.Text))
                                        select new { tessera.Prezzo }).First().Prezzo;
                    totaleTessere = Double.Parse(combo_quantitaTessere.SelectedValue.ToString()) * (float)prezzoTessera;
                }
                label_totaleBiglietti.Content = totaleBiglietti;
                label_totaleTessere.Content = totaleTessere;
                label_totale.Content = totaleTessere + totaleBiglietti;
            }
        }

        private void Txt_matricola_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkbox_trovata.IsChecked = false;
            combo_quantitaBiglietti.SelectedItem = null;
            combo_quantitaTessere.SelectedItem = null;
            combo_tipoTessere.SelectedItem = null;
            combo_quantitaBiglietti.ItemsSource = null;
            combo_quantitaTessere.ItemsSource = null;
            combo_tipoTessere.ItemsSource = null;
            label_prezzo.Content = String.Empty;
            label_totale.Content = String.Empty;
            label_totaleBiglietti.Content = String.Empty;
            label_totaleTessere.Content = String.Empty;
        }
    }
}
