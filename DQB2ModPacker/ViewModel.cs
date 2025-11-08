using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQB2ModPacker
{
    internal class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private MainWindow mainWindow;

        private ObservableCollection<IDXFile> _files = new ObservableCollection<IDXFile>();
        public ObservableCollection<IDXFile> Files
        {
            get => _files;
        }
        public ViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void AppendFile(string path)
        {
            _files.Add(new IDXFile(path, Versions.Count));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Files)));
        }

        public ObservableCollection<String> Versions { get; set; } = new ObservableCollection<String>();
        private List<uint> versionNumbers = new List<uint>();
        public void AppendVersion(string ver, uint number)
        {
            Versions.Add(ver+" "+number);
            versionNumbers.Add(number);
            for (int i = 0; i < _files.Count; i++)
            {
                _files[i].UpdateList(Versions.Count);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Versions)));

        }

        public void DeleteFile(int index)
        {
            if (index == -1) return;
                       _files.RemoveAt(index);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Files)));
        }

        public void ReorderFiles()
        {

            var sorted = Files.OrderBy(p => p.GetVersionIndex(0)).ToList();

            // Rebuild the collection in sorted order
            Files.Clear();
            foreach (var person in sorted)
                Files.Add(person);
        }
        public void PACKFILE() {

            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "idxzrc file|*.dqb2.bin";
            if (dlg.ShowDialog() == false) return;


            uint fileSize = (uint)(8 + 8 + 8 +
                (8 * versionNumbers.Count) + //VERSIONS
                (8 * versionNumbers.Count * _files.Count) + //FOR EACH VERSION ONE NUMBER
                + (8 * _files.Count)); //FOR EACH FILE ONE SIZE
            //Add the sizes.
            for (int i = 0; i < _files.Count; i++)
            {
                fileSize += (uint)_files[i].GetFileSize();
            }
            byte[] bin = new byte[fileSize];

            Array.Copy(BitConverter.GetBytes(versionNumber), 0, bin, 0, 8);
            Array.Copy(BitConverter.GetBytes((ulong)_files.Count), 0, bin, 8, 8);
            Array.Copy(BitConverter.GetBytes((ulong)versionNumbers.Count), 0, bin, 16, 8);
            int offset = 24;
            //For each version...
            for (int i_ver = 0; i_ver < versionNumbers.Count; i_ver++)
            {
                Array.Copy(BitConverter.GetBytes((ulong)versionNumbers[i_ver]), 0, bin, offset, 8);
                offset = offset + 8;

                //I get the file Indexes.
                for (int k_fil = 0; k_fil < _files.Count; k_fil++)
                {
                    ulong indexOfFile = _files[k_fil].GetVersionIndex(i_ver);
                    Array.Copy(BitConverter.GetBytes(indexOfFile), 0, bin, offset, 8);
                    offset = offset + 8;
                }
            }
            //For each file...
            for (int i = 0; i < _files.Count; i++)
            {
                ulong lenght = (ulong)_files[i].GetFileSize();
                Array.Copy(BitConverter.GetBytes(lenght), 0, bin, offset, 8);
                offset = offset + 8;
                //Copy file
                Array.Copy(_files[i].GetFileBytes(), 0, bin, offset, (int)lenght);
                offset = offset + (int)lenght;
            }

            System.IO.File.WriteAllBytes(dlg.FileName, bin);
        }


        private ulong versionNumber = 1;
    }
}
