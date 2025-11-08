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

namespace DQB2ModInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ViewModel viewModel;
        public MainWindow()
        {
            viewModel = new ViewModel(this);
            DataContext = viewModel;
            InitializeComponent();
        }

        private void LinkdataPathUpdate(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "LINKDATA|LINKDATA*.IDX";
            if (dlg.ShowDialog() == false) return;

            viewModel.LinkdataPath = dlg.FileName;
        }
        
        private void PacketPathUpdate(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "mod file (.dqb2.bin)|*.dqb2.bin";
            if (dlg.ShowDialog() == false) return;

            viewModel.PacketPath = dlg.FileName;
        }

        private void PatchAlg(object sender, RoutedEventArgs e)
        {
            if(viewModel.PatchAlg())
                Patched.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String text = File.ReadAllText("information.txt");
            MessageBox.Show(text,"INFORMATION", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
