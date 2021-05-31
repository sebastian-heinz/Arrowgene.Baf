using Arrowgene.Buffers;

namespace Arrowgene.Baf.Server.Asset
{
    public class ItemDat
    {
        public ItemDat()
        {
        }

        public void Parse()
        {
            IBuffer buffer =
                new StreamBuffer("/Users/railgun/dev/Arrowgene.Baf/Arrowgene.Baf.Server/Files/Item.dat");
            buffer.SetPositionStart();

            int count = 0;
            int a = buffer.ReadInt32();
            int b = buffer.ReadInt32();
            while (buffer.Position < buffer.Size)
            {
                int itemId = buffer.ReadInt32();
                string unk = buffer.ReadCString();
                string unk1 = buffer.ReadCString();
                int b1 = buffer.ReadInt32();
                int b2 = buffer.ReadInt32();
                int b3 = buffer.ReadInt32();
                int b4 = buffer.ReadInt32();
                int b5 = buffer.ReadInt32();
                int b6 = buffer.ReadInt32();
                count++;
            }


            int end = 0;

        }
    }
}