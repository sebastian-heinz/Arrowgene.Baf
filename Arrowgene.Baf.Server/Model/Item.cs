namespace Arrowgene.Baf.Server.Model
{
    public class Item
    {
        public Item()
        {
            Id = -1;
            ShopItem = null;
            Quantity = 0;
        }

        public int Id { get; set; }
        public ShopItem ShopItem { get; set; }
        public int Quantity { get; set; }
    }
}