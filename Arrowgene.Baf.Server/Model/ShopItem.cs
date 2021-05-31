namespace Arrowgene.Baf.Server.Model
{
    public class ShopItem
    {
        public static int DefaultId = 0x7FFFFFFF;
        
        public ShopItem()
        {
            Id = -1;
            Gem30 = -1;
            Money7 = -1;
            Money30 = -1;
            Money = -1;
            Level = -1;
            Name = null;
            Model = null;
            Type = ItemType.Chest;
            Gender = GenderType.Female;
            IsNew = false;
            IsHot = false;
            OnlyMarried = false;
            OnlyGift = false;
        }

        public int Id { get; set; }
        public int Gem30 { get; set; }
        public int Money7 { get; set; }
        public int Money30 { get; set; }
        public int Money { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public ItemType Type { get; set; }
        public GenderType Gender { get; set; }
        public bool IsNew { get; set; }
        public bool IsHot { get; set; }
        public bool OnlyMarried { get; set; }
        public bool OnlyGift { get; set; }
    }
}