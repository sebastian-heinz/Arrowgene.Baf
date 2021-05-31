namespace Arrowgene.Baf.Server.Model
{
    public class Character
    {
        public Character()
        {
            Name = null;
            Gender = GenderType.Female;
            Level = 1;
            MCoins = 1000000;
            GCoins = 1000000;
        }

        public string Name { get; set; }
        public GenderType Gender { get; set; }
        public int Level { get; set; }
        public int MCoins { get; set; }
        public int GCoins { get; set; }
    }
}