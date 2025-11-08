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

namespace DQB2ModPacker
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "idxzrc file|*.idxzrc";
            if (dlg.ShowDialog() == false) return;

            viewModel.AppendFile(dlg.FileName);
            viewModel.ReorderFiles();
        }

        private void ButtonVersion_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Input("Enter version name");
            dialog.Owner = this;
            bool? result = dialog.ShowDialog();
            string userInput;
            uint number_of_files;
            if (result == true)
                userInput = dialog.Result;
            else
                return;


            dialog = new Input("Enter number of files");
            dialog.Owner = this;
            result = dialog.ShowDialog();
            if (result == true)
            {
                if (uint.TryParse(dialog.Result, out number_of_files))
                {
                    viewModel.AppendVersion(userInput, number_of_files);
                }
                else
                {
                    return;
                }
            }
            else
                return;
        }

        private void ButtonVersionSelect_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Input("Enter version name");
            dialog.Owner = this;
            bool? result = dialog.ShowDialog();
            string userInput;
            uint number_of_files;
            if (result == true)
                userInput = dialog.Result;
            else
                return;


            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "LINKDATA file|LINKDATA*.idx";
            if (dlg.ShowDialog() == false) return;

            FileInfo fileInfo = new FileInfo(dlg.FileName);
            number_of_files = (uint)(fileInfo.Length / 32);

            viewModel.AppendVersion(userInput, number_of_files);
            
        }

        private void Delete_file(object sender, MouseButtonEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null)
            {
                viewModel.DeleteFile(listBox.SelectedIndex);
            }
        }

        private void PackAlgorithm(object sender, RoutedEventArgs e)
        {
            //Size of file
            viewModel.ReorderFiles();
            viewModel.PACKFILE();
        }
    }
}
