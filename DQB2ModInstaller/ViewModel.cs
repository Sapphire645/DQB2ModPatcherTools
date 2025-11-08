using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DQB2ModInstaller
{
    internal class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private String _linkdataPath;
        public Visibility LinkdataWritten => String.IsNullOrEmpty(_linkdataPath) ? Visibility.Visible : Visibility.Collapsed;
        private bool _linkdataExists;
        public String LinkdataPath
        {
            get => _linkdataPath;
            set
            {
                if (_linkdataPath != value)
                {
                    _linkdataPath = value;
                    _linkdataExists = File.Exists(_linkdataPath);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LinkdataPath)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LinkdataWritten)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LinkdataError)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LINKDATA_Patchable)));
                }
            }
        }

        public bool LINKDATA_Patchable => _linkdataExists && _packetExists;
        public Brush LinkdataError => !_linkdataExists ? (System.Windows.Media.Brush)System.Windows.Application.Current.Resources["MediumOrangeBrush"] : Brushes.White;
        public Brush PacketError => !_packetExists ? (System.Windows.Media.Brush)System.Windows.Application.Current.Resources["MediumOrangeBrush"] : Brushes.White; 

        private String _packetPath;
        public Visibility PacketWritten => String.IsNullOrEmpty(_packetPath) ? Visibility.Visible : Visibility.Collapsed;
        private bool _packetExists;
        public Visibility Fo => String.IsNullOrEmpty(_packetPath) ? Visibility.Visible : Visibility.Collapsed;
        public String PacketPath
        {
            get => _packetPath;
            set
            {
                if (_packetPath != value)
                {
                    _packetPath = value;
                    _packetExists = File.Exists(_packetPath);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PacketPath)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PacketWritten)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PacketError)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LINKDATA_Patchable)));
                }
            }
        }

        private MainWindow mainWindow;
        public ViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public bool PatchAlg()
        {
            ModFile[] res;
            try
            {
                res = PacketFile.UnpacketFile(_packetPath);
                if (res == null)
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR. This mod is either not a mod, or is not compatible with this version. " + ex, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return LINKDATA.SaveToLINKDATA(_linkdataPath, res);
        }
    }
}
