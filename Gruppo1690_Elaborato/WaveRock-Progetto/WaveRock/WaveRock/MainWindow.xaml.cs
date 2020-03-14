using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var path = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            path = path + "\\WaveRock\\WaveRock";

            //var path = System.IO.Directory.GetCurrentDirectory();

            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            InitializeComponent();
        }

        private void Btt_accedi_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new Accesso();
        }
    }
}
