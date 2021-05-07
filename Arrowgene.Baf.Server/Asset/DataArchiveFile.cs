namespace Arrowgene.Baf.Server.Asset
{
    public class DataArchiveFile
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public int Size { get; set; }

        public int Offset { get; set; }

        public int Id { get; set; }

        public int NameOffset { get; set; }
    }
}