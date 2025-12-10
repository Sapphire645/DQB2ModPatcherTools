using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for IDXFile.xaml
    /// </summary>
    public partial class IDXFile : UserControl
    {
        private IDXFileViewModel viewModel;
        public IDXFile(String file, int count)
        {
            viewModel = new IDXFileViewModel(file, count);
            DataContext = viewModel;
            InitializeComponent();
        }

        internal void UpdateList(int count)
        {
            viewModel.UpdateList(count);
        }

        private void EditNumber(object sender, MouseButtonEventArgs e)
        {
            ListBox? listBox = sender as ListBox;
            if (listBox != null)
            {
                viewModel.UpdateNumber(listBox.SelectedIndex);
            }
        }

        public long GetFileSize()
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(viewModel.FilePath);
            return fi.Length;
        }
        public ulong GetVersionIndex(int index)
        {
            return viewModel.GetVersionIndex(index);
        }
        public byte[] GetFileBytes()
        {
            return System.IO.File.ReadAllBytes(viewModel.FilePath);
        }
    }
    internal class IDXFileViewModel : INotifyPropertyChanged
    {
        public String FilePath { get; set; }

        public ObservableCollection<ulong> VersionsIndex { get; set; } = new ObservableCollection<ulong>();
        public IDXFileViewModel(String filePath, int vcount)
        {
            FilePath = filePath;
            UpdateList(vcount);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void UpdateList(int versionCount)
        {
            for (int i = 0; i < versionCount; i++)
            {
                VersionsIndex.Add(0);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VersionsIndex)));
        }

        public void UpdateNumber(int index)
        {
            var dialog = new Input("Enter version name");
            bool? result = dialog.ShowDialog();
            ulong number_of_files;
            if (result == true)
            {
                if (ulong.TryParse(dialog.Result, out number_of_files))
                {
                    VersionsIndex[index] = number_of_files;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VersionsIndex)));
                }
                else
                {
                    return;
                }
            }
            else
                return;
            
        }
        public ulong GetVersionIndex(int index)
        {
            return VersionsIndex[index];
        }
    }
}
