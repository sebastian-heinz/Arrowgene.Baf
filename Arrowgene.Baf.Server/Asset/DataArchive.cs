using System;
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

        private FileInfo _saiFile;
        private FileInfo _sacFile;
        private IBuffer _sacBuffer;
        private bool _loaded;
        private List<DataArchiveFile> _files;

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

            saiBuffer.ReadInt32();
            int entrySize = saiBuffer.ReadInt32() * 16;
            int fileNameSize = saiBuffer.ReadInt32();
            saiBuffer.ReadInt32();
            byte[] dataAttribs = saiBuffer.ReadBytes(entrySize);
            dataAttribs = DecryptData(Password, dataAttribs);
            byte[] dataFileNames = saiBuffer.ReadBytes(fileNameSize);
            dataFileNames = DecryptData(Password, dataFileNames);

            string tmp = "";
            IBuffer attributeBuffer = new StreamBuffer(dataAttribs);
            attributeBuffer.SetPositionStart();
            for (int a = 0; a < attributeBuffer.Size / 16; a++)
            {
                DataArchiveFile item = new DataArchiveFile();
                item.Id = attributeBuffer.ReadInt32();
                item.Size = attributeBuffer.ReadInt32();
                item.Offset = attributeBuffer.ReadInt32();
                item.NameOffset = attributeBuffer.ReadInt32();
                for (int i = item.NameOffset; i < dataFileNames.Length; i++)
                {
                    if (dataFileNames[i] != 0)
                        tmp += (char) dataFileNames[i];
                    else
                    {
                        int lastSlash = tmp.LastIndexOf("\\", StringComparison.Ordinal);
                        if (lastSlash >= 0)
                        {
                            item.Path = tmp.Substring(0, lastSlash + 1);
                            item.Name = tmp.Substring(lastSlash + 1);
                        }
                        else
                        {
                            item.Path = "";
                            item.Name = tmp;
                        }

                        tmp = "";
                        break;
                    }
                }

                _files.Add(item);
            }

            byte[] sacData = File.ReadAllBytes(_sacFile.FullName);
            _sacBuffer = new StreamBuffer(sacData);
            _sacBuffer.SetPositionStart();
            _loaded = true;

            return true;
        }

        public bool Save(DataArchiveFile file, string destinationDirectory)
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

            return true;
        }

        public bool SaveAll(string destinationDirectory)
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
                Logger.Info($"Saving: ({idx++}/{_files.Count}) {file.Path}{file.Name}");
                Save(file, destinationDirectory);
            }

            return true;
        }

        private byte[] DecryptData(byte[] key, byte[] data)
        {
            byte[] decrypted = new byte[data.Length];
            long lKey = (long) ((key[0] & 0xff) | ((key[1] & 0xff) << 8) | ((key[2] & 0xff) << 16) |
                                ((key[3] & 0xff) << 24));
            long ltmpKey;

            for (int i = 0; i < decrypted.Length; i++)
            {
                lKey = (lKey * 0x3d09) & 0xffffffffL;
                ltmpKey = lKey >> 0x10;
                decrypted[i] = (byte) (data[i] - (byte) (ltmpKey & 0xff));
            }

            return decrypted;
        }
    }
}