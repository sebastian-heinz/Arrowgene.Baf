using System.Collections.Generic;
using System.IO;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.Asset
{
    public class DataArchive
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(DataArchive));
        private static readonly byte[] Password = {0x61, 0xf9, 0x53, 0x7c};
        private static readonly int NoNameOffset = -1;
        private static readonly int EntrySize = 16;
        public static readonly char BafDirectorySeparatorChar = '\\';
        public static readonly string BafMagic = "SDO";

        private FileInfo _saiFile;
        private FileInfo _sacFile;
        private IBuffer _sacBuffer;
        private bool _loaded;
        private readonly List<DataArchiveFile> _files;

        public DataArchive()
        {
            _files = new List<DataArchiveFile>();
            Reset();
        }

        private void Reset()
        {
            _files.Clear();
            _loaded = false;
            _sacBuffer = null;
            _sacFile = null;
            _saiFile = null;
        }

        public bool Load(string path)
        {
            Reset();

            string saiPath = path + ".sai";
            if (!File.Exists(saiPath))
            {
                Logger.Error($".sai file not found: {saiPath}");
                Reset();
                return false;
            }

            _saiFile = new FileInfo(saiPath);

            string sacPath = path + ".sac";
            if (!File.Exists(sacPath))
            {
                Logger.Error($".sac file not found: {sacPath}");
                Reset();
                return false;
            }

            _sacFile = new FileInfo(sacPath);

            byte[] saiData = File.ReadAllBytes(_saiFile.FullName);
            IBuffer saiBuffer = new StreamBuffer(saiData);
            saiBuffer.SetPositionStart();

            string magic = saiBuffer.ReadCString();
            if (BafMagic != magic)
            {
                Logger.Error($"Invalid Magic File Bytes: {magic}");
                Reset();
                return false;
            }

            int entries = saiBuffer.ReadInt32();
            int fileNameSize = saiBuffer.ReadInt32();
            int unk1 = saiBuffer.ReadInt32();
            byte[] dataAttribs = saiBuffer.ReadBytes(entries * EntrySize);
            dataAttribs = DecryptData(Password, dataAttribs);
            byte[] dataFileNames = saiBuffer.ReadBytes(fileNameSize);
            dataFileNames = DecryptData(Password, dataFileNames);

            IBuffer nameBuffer = new StreamBuffer(dataFileNames);
            IBuffer attributeBuffer = new StreamBuffer(dataAttribs);
            attributeBuffer.SetPositionStart();

            for (int i = 0; i < entries; i++)
            {
                DataArchiveFile item = new DataArchiveFile();
                item.Id = attributeBuffer.ReadInt32();
                item.Size = attributeBuffer.ReadInt32();
                item.Offset = attributeBuffer.ReadInt32();
                item.NameOffset = attributeBuffer.ReadInt32();

                nameBuffer.Position = item.NameOffset;
                string filePath = nameBuffer.ReadCString();
                filePath = filePath.Replace(BafDirectorySeparatorChar, Path.DirectorySeparatorChar);
                string fileName = Path.GetFileName(filePath);
                string directoryName = Path.GetDirectoryName(filePath);

                item.Name = fileName;
                item.Path = directoryName;
                
                _files.Add(item);
            }

            byte[] sacData = File.ReadAllBytes(_sacFile.FullName);
            _sacBuffer = new StreamBuffer(sacData);
            _sacBuffer.SetPositionStart();
            _loaded = true;

            // decrypted
            StreamBuffer decSaiBuffer = new StreamBuffer();
            decSaiBuffer.WriteCString(BafMagic);
            decSaiBuffer.WriteInt32(entries);
            decSaiBuffer.WriteInt32(dataFileNames.Length);
            decSaiBuffer.WriteInt32(0);
            decSaiBuffer.WriteBytes(dataAttribs);
            decSaiBuffer.WriteBytes(dataFileNames);
            File.WriteAllBytes(_saiFile.FullName + ".dec", decSaiBuffer.GetAllBytes());

            return true;
        }

        public bool AddFile(string rootDirectory, string filePath)
        {
            if (_sacBuffer == null)
            {
                _sacBuffer = new StreamBuffer();
            }

            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return false;
            }

            byte[] fileData = File.ReadAllBytes(fileInfo.FullName);
            int offset = _sacBuffer.Position;
            _sacBuffer.WriteBytes(fileData);

            string bafFilePath = filePath.Replace(rootDirectory, "");
            string fileName = Path.GetFileName(bafFilePath);
            string directoryName = Path.GetDirectoryName(bafFilePath);
            if (directoryName == null)
            {
                directoryName = "";
                bafFilePath = fileName;
            }
            else
            {
                directoryName = directoryName.Replace(Path.DirectorySeparatorChar, BafDirectorySeparatorChar);
                if (directoryName.StartsWith(BafDirectorySeparatorChar))
                {
                    directoryName = directoryName.Substring(1);
                }

                if (directoryName == string.Empty)
                {
                    bafFilePath = fileName;
                }
                else
                {
                    bafFilePath = directoryName + BafDirectorySeparatorChar + fileName;
                }
            }
            
            DataArchiveFile file = new DataArchiveFile();
            file.Id = GetPathIndex(bafFilePath);
            file.Size = fileData.Length;
            file.Name = fileName;
            file.Path = directoryName;
            file.Offset = offset;
            file.NameOffset = NoNameOffset;

            _files.Add(file);

            Logger.Info($"Add: {file.Id} {file.Path}{file.Name}");

            return true;
        }

        public bool AddFolder(string path)
        {
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

            int idx = 1;
            foreach (string filePath in files)
            {
                AddFile(path, filePath);
                Logger.Info($"Progress: {idx++}/{files.Length}");
            }

            return true;
        }

        public bool Save(string directory, string fileName)
        {
            if (!Directory.Exists(directory))
            {
                Logger.Error($"Directory not found: {directory}");
                Reset();
                return false;
            }

            string saiPath = directory + fileName + ".sai";
            if (File.Exists(saiPath))
            {
                Logger.Error($"Sai file already exists: {saiPath}");
                Reset();
                return false;
            }

            _saiFile = new FileInfo(saiPath);

            string sacPath = directory + fileName + ".sac";
            if (File.Exists(sacPath))
            {
                Logger.Error($"Sac file already exists: {sacPath}");
                Reset();
                return false;
            }

            _sacFile = new FileInfo(sacPath);

            if (_files.Count <= 0)
            {
                Logger.Error($"No Files add");
                Reset();
                return false;
            }

            if (_sacBuffer == null)
            {
                Logger.Error($"No Data available");
                Reset();
                return false;
            }

            IBuffer attributeBuffer = new StreamBuffer();
            IBuffer nameBuffer = new StreamBuffer();
            int entries = 0;

            _files.Sort((a, b) => a.Id.CompareTo(b.Id));
            foreach (DataArchiveFile file in _files)
            {
                file.NameOffset = nameBuffer.Position;
                string filePath = "";
                if (file.Path.Length > 0)
                {
                    filePath += file.Path + BafDirectorySeparatorChar;
                }

                filePath += file.Name;
                nameBuffer.WriteCString(filePath);
                attributeBuffer.WriteInt32(file.Id);
                attributeBuffer.WriteInt32(file.Size);
                attributeBuffer.WriteInt32(file.Offset);
                attributeBuffer.WriteInt32(file.NameOffset);
                entries++;
            }

            byte[] dataAttribs = attributeBuffer.GetAllBytes();
            byte[] dataFileNames = nameBuffer.GetAllBytes();

            dataAttribs = EncryptData(Password, dataAttribs);
            dataFileNames = EncryptData(Password, dataFileNames);

            IBuffer saiBuffer = new StreamBuffer();
            saiBuffer.WriteCString(BafMagic);
            saiBuffer.WriteInt32(entries);
            saiBuffer.WriteInt32(dataFileNames.Length);
            saiBuffer.WriteInt32(0);
            saiBuffer.WriteBytes(dataAttribs);
            saiBuffer.WriteBytes(dataFileNames);

            File.WriteAllBytes(_saiFile.FullName, saiBuffer.GetAllBytes());
            File.WriteAllBytes(_sacFile.FullName, _sacBuffer.GetAllBytes());

            // decrypted
            StreamBuffer decSaiBuffer = new StreamBuffer();
            decSaiBuffer.WriteCString(BafMagic);
            decSaiBuffer.WriteInt32(entries);
            decSaiBuffer.WriteInt32(dataFileNames.Length);
            decSaiBuffer.WriteInt32(0);
            decSaiBuffer.WriteBytes(attributeBuffer.GetAllBytes());
            decSaiBuffer.WriteBytes(nameBuffer.GetAllBytes());
            File.WriteAllBytes(_saiFile.FullName + ".dec", decSaiBuffer.GetAllBytes());

            return true;
        }

        public bool Extract(DataArchiveFile file, string destinationDirectory)
        {
            if (!_loaded)
            {
                Logger.Error($"Archive is not loaded");
                return false;
            }

            if (!Directory.Exists(destinationDirectory))
            {
                Logger.Error($"Directory not found: {destinationDirectory}");
                return false;
            }

            DirectoryInfo destination = new DirectoryInfo(destinationDirectory);
            byte[] fileData = _sacBuffer.GetBytes(file.Offset, file.Size);
            string dir = Path.Combine(destination.FullName, file.Path);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string path = Path.Combine(dir, file.Name);
            File.WriteAllBytes(path, fileData);

            Logger.Info($"Extract: {file.Id} {file.Path}{file.Name}");

            return true;
        }

        public bool ExtractAll(string destinationDirectory)
        {
            if (!_loaded)
            {
                Logger.Error($"Archive is not loaded");
                return false;
            }

            if (!Directory.Exists(destinationDirectory))
            {
                Logger.Error($"Directory not found: {destinationDirectory}");
                return false;
            }

            int idx = 1;
            foreach (DataArchiveFile file in _files)
            {
                Extract(file, destinationDirectory);
                Logger.Info($"Progress: {idx++}/{_files.Count}");
            }

            return true;
        }

        private byte[] DecryptData(byte[] key, byte[] data)
        {
            byte[] decrypted = new byte[data.Length];
            long lKey = (long) ((key[0] & 0xff) | ((key[1] & 0xff) << 8) | ((key[2] & 0xff) << 16) |
                                ((key[3] & 0xff) << 24));

            for (int i = 0; i < decrypted.Length; i++)
            {
                // ADDR:0x408704 RVA: 0x8704
                lKey = (lKey * 0x3d09) & 0xffffffffL;
                long tmpKey = lKey >> 0x10;
                decrypted[i] = (byte) (data[i] - (byte) (tmpKey & 0xff));
            }

            return decrypted;
        }

        private byte[] EncryptData(byte[] key, byte[] data)
        {
            byte[] decrypted = new byte[data.Length];
            long lKey = (long) ((key[0] & 0xff) | ((key[1] & 0xff) << 8) | ((key[2] & 0xff) << 16) |
                                ((key[3] & 0xff) << 24));

            for (int i = 0; i < decrypted.Length; i++)
            {
                // ADDR:0x408704 RVA: 0x8704
                lKey = (lKey * 0x3d09) & 0xffffffffL;
                long tmpKey = lKey >> 0x10;
                decrypted[i] = (byte) (data[i] + (byte) (tmpKey & 0xff));
            }

            return decrypted;
        }

        private int GetPathIndex(string path)
        {
            int index = 0;
            int offset = 0;

            foreach (var pathChar in path)
            {
                index += pathChar - 32 << offset;
                offset = offset < 20 ? offset + 5 : 0;
            }

            return index % 0xfff1;
        }
    }
}