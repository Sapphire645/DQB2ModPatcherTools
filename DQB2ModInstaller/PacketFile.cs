using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DQB2ModInstaller
{
    internal class PacketFile
    {

        private static byte[] ReadPacketFile(string linkdataPath)
        {
            //Checks
            if (!System.IO.File.Exists(linkdataPath)) return null;
            //Read
            byte[] bin = System.IO.File.ReadAllBytes(linkdataPath);
            return bin;
        }

        private static int VERSION = 1;
        public static ModFile[] UnpacketFile(String path)
        {
            byte[] bytes = ReadPacketFile(path);

            ulong program_version = BitConverter.ToUInt64(bytes, 0);

            if(VERSION < (int)program_version)
            {
                var res = MessageBox.Show("WARNING: This mod was made for a future release of this program " +
                    $"(Mod version = {program_version}, program version = {VERSION}). This might crash, but won't corrupt your game. Proceed?","MOD VERSION INCOMPATIBILITY", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(res == MessageBoxResult.No)
                {
                    return null;
                }
            }

            ulong fileNumber = BitConverter.ToUInt64(bytes, 8);
            ulong linkdata_number_of_versions = BitConverter.ToUInt64(bytes, 16);

            ModFile[] FILES = new ModFile[fileNumber];
            VersionLocation[][] versions = new VersionLocation[fileNumber][];
            for (ulong i = 0; i < fileNumber; i++)
            {
                versions[i] = new VersionLocation[linkdata_number_of_versions];
            }

            int offset = 24;
            //For each version...
            for (ulong i_ver = 0; i_ver < linkdata_number_of_versions; i_ver++)
            {
                ulong linkdata_size_in_version = BitConverter.ToUInt64(bytes, offset);
                offset = offset + 8;
                Console.WriteLine("VERSION " + linkdata_size_in_version);
                //I get the file Indexes.
                for (ulong k_fil = 0; k_fil < fileNumber; k_fil++)
                {
                    ulong indexOfFile = BitConverter.ToUInt64(bytes, offset);
                    versions[k_fil][i_ver] = new VersionLocation(linkdata_size_in_version, indexOfFile);
                    offset = offset + 8;
                    Console.WriteLine("\tINDEX " + indexOfFile);
                }
            }

            for (ulong i = 0; i < fileNumber; i++)
            {
                Console.WriteLine("File " + i);
                ulong lenght = BitConverter.ToUInt64(bytes, offset);
                offset = offset + 8;
                byte[] dataa = new byte[lenght];

                //Copy file
                Array.Copy(bytes, offset, dataa, 0, (int)lenght);
                offset = offset + (int)lenght;
                FILES[i] = new ModFile(versions[i], dataa);
                Console.WriteLine(FILES[i].ToString());
            }
            return FILES;
        }
    }
}
