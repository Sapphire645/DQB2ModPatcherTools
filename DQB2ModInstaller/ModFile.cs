
namespace DQB2ModInstaller
{

    internal class VersionLocation
    {
        private UInt64 version; //Stores number of files of LINKDATA.
        private ulong index;
        public UInt64 Version => version;
        public ulong Index => index;
        public VersionLocation(UInt64 version, ulong index)
        {
            this.version = version;
            this.index = index;
        }
    }
    internal class ModFile
    {
        public UInt64 _uncompressedSize;

        private VersionLocation[] location;
        private byte[] _data;

        public UInt64 Length => (UInt64)_data.Length;
        public UInt64 UncompressedSize => _uncompressedSize;
        public byte[] Data => _data;

        public ulong GetIndex(ulong version)
        {
            for (int i = 0; i < location.Length; i++)
            {
                if (location[i].Version == version)
                {
                    return location[i].Index;
                }
            }
            throw new Exception("Version not found");
        }

        public ModFile(VersionLocation[] location, byte[] bytes)
        {
            _uncompressedSize = BitConverter.ToUInt32(bytes, 8);

            _data = new byte[bytes.Length];
            Array.Copy(bytes, 0, _data, 0, bytes.Length);

            this.location = location;
        }
        public override string ToString()
        {
            string a = "";
            //foreach (byte b in _data) a += b.ToString("X2") + " ";
            return $"ModFile: UncompressedSize={_uncompressedSize}, DataLength={_data.Length}, \n\t data={a}";

        }
    }
}
