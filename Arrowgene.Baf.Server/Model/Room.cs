namespace Arrowgene.Baf.Server.Model
{
    public class Room
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int SlotsAvailable { get; set; }
        public TeamType Team { get; set; }
        public KeyType Key { get; set; }
        public bool HasPassword { get; set; }
        public bool AllowSpectators { get; set; }
        public string Password { get; set; }
        public RoomStatusType Status { get; set; }
        
        
    }
}