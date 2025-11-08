using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DQB2ModInstaller
{
    internal static class LINKDATA
    {

        private static byte[] ReadLINKDATA_BIN(string linkdataPath)
        {
            //Checks
            if (!System.IO.File.Exists(linkdataPath)) return null;

            String path = linkdataPath.Substring(0, linkdataPath.Length - 3) + "BIN";
            if (!System.IO.File.Exists(path)) return null;

            //Read
            byte[] bin = System.IO.File.ReadAllBytes(path);
            return bin;
        }

        private static byte[] ReadLINKDATA_IDX(string linkdataPath)
        {
            //Checks
            if (!System.IO.File.Exists(linkdataPath)) return null;
            //Read
            byte[] bin = System.IO.File.ReadAllBytes(linkdataPath);
            return bin;
        }

        private static void WriteLINKDATA(string linkdataPath, byte[] idx, byte[] bin)
        {
            //Checks
            if (!System.IO.File.Exists(linkdataPath)) return;

            String path = linkdataPath.Substring(0, linkdataPath.Length - 3) + "BIN";
            if (!System.IO.File.Exists(path)) return;

            //Write
            System.IO.File.WriteAllBytes(path, bin);
            System.IO.File.WriteAllBytes(linkdataPath, idx);
        }

        //The entreis must be sorted 
        public static bool SaveToLINKDATA(string linkdataPath, ModFile[] entries)
        {
            //Read
            byte[] link_bin = LINKDATA.ReadLINKDATA_BIN(linkdataPath);
            byte[] link_idx = LINKDATA.ReadLINKDATA_IDX(linkdataPath);


            for(int i = 0; i < entries.Length; i++)
            {
                //My current entry
                ModFile entryCurrent = entries[i];

                //The indices to update to. If last then to the end of file.
                ulong LastIndex = (ulong)(link_idx.Length / 32);
                ulong ThisIndex = 0;
                try
                {
                    //LastIndex = entries[i + 1].GetIndex(LastIndex);
                    ThisIndex = entryCurrent.GetIndex(LastIndex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR. Mod not compatible " + ex, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                if (i + 1 < entries.Length)
                {
                    try
                    {
                        //LastIndex = entries[i + 1].GetIndex(LastIndex);
                        ThisIndex = entryCurrent.GetIndex(LastIndex);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ERROR. Mod not compatible " + ex, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                //Compressed value
                var OldCompressed = BitConverter.ToUInt64(link_idx, (int)(ThisIndex * 32 + 16));
                var OldOffset = BitConverter.ToUInt64(link_idx, (int)ThisIndex * 32 + 0);

                // offset = after - original
                // original file
                uint offset = ((uint)OldCompressed + 0x80) / 0x100 * 0x100;
                // after file
                offset = ((uint)entryCurrent.Length + 0x80) / 0x100 * 0x100 - offset;

                //write up until old offset
                Byte[] bin = new Byte[link_bin.Length + offset];
                Array.Copy(link_bin, 0, bin, 0, (int)OldOffset);
                //write up until old offset idx

                //Must save this
                byte[] compr = new byte[4];
                Array.Copy(link_idx, (int)(ThisIndex * 32 + 24), compr,0,4);

                Array.Fill<Byte>(link_idx, 0, (int)(ThisIndex * 32 + 8), 24);

                //write this idx
                Array.Copy(BitConverter.GetBytes(entryCurrent.UncompressedSize), 0, link_idx, (int)(ThisIndex * 32 + 8), 4);
                Array.Copy(BitConverter.GetBytes(entryCurrent.Length), 0, link_idx, (int)(ThisIndex * 32 + 16), 4);
                Array.Copy(compr, 0, link_idx, (int)(ThisIndex * 32 + 24), 4);

                //Copy data
                Array.Copy(entryCurrent.Data, 0, bin, (int)OldOffset, (int)entryCurrent.Length);
                //Copy rest of old data
                for (uint index = (uint)(ThisIndex + 1); index < LastIndex; index++)
                {
                    var IDXofset = BitConverter.ToUInt64(link_idx, (int)(index * 32 + 0));
                    var IDXcompressed = BitConverter.ToUInt64(link_idx, (int)(index * 32 + 16));
                    UInt64 address = IDXofset + (UInt64)offset;
                    Array.Copy(BitConverter.GetBytes((UInt64)address), 0, link_idx, index * 32, 8);
                    Array.Copy(link_bin, (int)IDXofset, bin, (int)address, (int)IDXcompressed);
                }
                //Update the link bin for next iteration
                //Should try to not have it write all but......
                link_bin = bin;
            }

            WriteLINKDATA(linkdataPath, link_idx, link_bin);
            return true;
        }
    }
}
