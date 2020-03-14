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
    /// Logica di interazione per AggiungiPersona.xaml
    /// </summary>
    public partial class AggiungiPersona : Page
    {

        DataClassesWRDataContext db = new DataClassesWRDataContext();

        public AggiungiPersona()
        {
            InitializeComponent();
            date_data.BlackoutDates.Add(new CalendarDateRange(DateTime.Now.AddYears(-6), DateTime.Now));
            date_data.DisplayDateEnd = DateTime.Now;
        }

        private void Btt_aggiungi_Click(object sender, RoutedEventArgs e)
        {
            if(txt_nome.Text.Equals(String.Empty) || txt_cognome.Equals(String.Empty) || date_data.SelectedDate is null)
            {
                MessageBox.Show("Sono segnalati in rosso i dati obbligatori", "Inserimento non completo", MessageBoxButton.OK);
                txt_nome.BorderBrush = System.Windows.Media.Brushes.Red;
                txt_cognome.BorderBrush = System.Windows.Media.Brushes.Red;
                date_data.BorderBrush = System.Windows.Media.Brushes.Red;
            }
            else
            {
                bool accettato = true;

                int numero;
                accettato = int.TryParse(txt_cellulare.ToString(), out numero) || txt_cellulare.Text.Equals(String.Empty);
                if (!accettato)
                {
                    txt_cellulare.BorderBrush = System.Windows.Media.Brushes.Red;
                    MessageBox.Show("Controllare i campi segnalati", "Inserimento non corretto", MessageBoxButton.OK);
                } 
                else
                {
                    PERSONA persona = new PERSONA();
                    persona.Nome = txt_nome.Text;
                    persona.Cognome = txt_cognome.Text;
                    persona.DataNascita = date_data.SelectedDate.Value;
                    persona.Istruttore = check_istruttore.IsChecked.Value ? '1' : '0';
                    persona.Indirizzo = txt_indirizzo.Text.Equals(String.Empty) ? null : txt_indirizzo.Text;
                    if (!txt_cellulare.Text.Equals(String.Empty))
                    {
                        persona.Cellulare = numero;
                    }
                    db.PERSONA.InsertOnSubmit(persona);
                    db.SubmitChanges();
                    String messaggioMatricola = String.Format("Inserimento completato\nMatricola: "+ persona.IDPersona);
                    MessageBox.Show(messaggioMatricola, "Successo", MessageBoxButton.OK);
                    reset();
                }
            }
        }

        private void reset()
        {
            txt_cellulare.Text = String.Empty;
            txt_cognome.Text = String.Empty;
            txt_indirizzo.Text = String.Empty;
            txt_nome.Text = String.Empty;
            date_data.SelectedDate = null;
            check_istruttore.IsChecked = false;
            txt_nome.BorderBrush = System.Windows.Media.Brushes.Black;
            txt_cognome.BorderBrush = System.Windows.Media.Brushes.Black;
            date_data.BorderBrush = System.Windows.Media.Brushes.Black;
            txt_cellulare.BorderBrush = System.Windows.Media.Brushes.Black;
        }
    }
}
