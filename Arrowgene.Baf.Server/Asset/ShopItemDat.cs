using Arrowgene.Buffers;

namespace Arrowgene.Baf.Server.Asset
{
    public class ShopItemDat
    {
        public ShopItemDat()
        {
        }

        public void Parse()
        {
            IBuffer buffer =
                new StreamBuffer("/Users/railgun/dev/Arrowgene.Baf/Arrowgene.Baf.Server/Files/ShopItem.dat");
            buffer.SetPositionStart();
            
            int count = 0;
            int a = buffer.ReadInt32();
            int b = buffer.ReadInt32();
            while (buffer.Position < buffer.Size)
            {
                int itemId = buffer.ReadInt32();
                int b1 = buffer.ReadInt32();
                int b2 = buffer.ReadInt32();
                int b3 = buffer.ReadInt32();
                int b5 = buffer.ReadInt32();
                
                int b6 = buffer.ReadInt32();
                int b7 = buffer.ReadInt32();
                int b8 = buffer.ReadInt32();
                int b9 = buffer.ReadInt32();
                int b10 = buffer.ReadInt32();
                
                int b11 = buffer.ReadInt32();
                int b12 = buffer.ReadInt32();
                int b13 = buffer.ReadInt32();
                int b14 = buffer.ReadInt32();
                int b15 = buffer.ReadInt32();
                
                int b16 = buffer.ReadInt32();
                int b17 = buffer.ReadInt32();
                int b18 = buffer.ReadInt32();
                int b19 = buffer.ReadInt32();
                
                count++;
            }

            int iss = 0;

        }
    }
}