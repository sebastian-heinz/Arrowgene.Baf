namespace Arrowgene.Baf.Server.Model
{
    public class Character
    {
        public Character()
        {
            Id = 1337;
            Name = null;
            Gender = GenderType.Female;
            Level = 1;
            MCoins = 1000000;
            GCoins = 1000000;
            Head = null;
            Hair = null;
            Chest = null;
            Pants = null;
            Hands = null;
            Shoes = null;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public GenderType Gender { get; set; }
        public int Level { get; set; }
        public int MCoins { get; set; }
        public int GCoins { get; set; }
        public Item Head { get; set; }
        public Item Hair { get; set; }
        public Item Chest { get; set; }
        public Item Pants { get; set; }
        public Item Hands { get; set; }
        public Item Shoes { get; set; }

        public int GetShopItemId(ItemType itemType)
        {
            Item item = null;
            switch (itemType)
            {
                case ItemType.Head:
                {
                    item = Head;
                    break;
                }
                case ItemType.Hair:
                {
                    item = Hair;
                    break;
                }
                case ItemType.Chest:
                {
                    item = Chest;
                    break;
                }
                case ItemType.Pants:
                {
                    item = Pants;
                    break;
                }
                case ItemType.Hands:
                {
                    item = Hands;
                    break;
                }
                case ItemType.Shoes:
                {
                    item = Shoes;
                    break;
                }
            }

            if (item != null)
            {
                return item.ShopItem.Id;
            }

            return ShopItem.DefaultId;
        }
    }
}