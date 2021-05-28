using System.Collections.Generic;

namespace Arrowgene.Baf.Server.Model
{
    public class Channel
    {
        private List<Room> _rooms;

        public void CreateRoom()
        {
            _rooms = new List<Room>();
        }
        
        
    }
}