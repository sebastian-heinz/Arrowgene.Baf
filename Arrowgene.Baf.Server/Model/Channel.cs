using System.Collections.Generic;

namespace Arrowgene.Baf.Server.Model
{
    public class Channel
    {
        public const int MaxRooms = 100;
        public const int MaxChannelLoad = 100;

        private readonly Room[] _rooms;
        private readonly object _channelLock;

        public Channel(short tab, short number, string name)
        {
            Tab = tab;
            Number = number;
            Name = name;
            MaxLoad = MaxChannelLoad;
            CurrentLoad = 0;
            _channelLock = new object();
            _rooms = new Room[MaxRooms];
        }

        public int MaxLoad { get; set; }
        public int CurrentLoad { get; set; }
        public string Name { get; set; }
        public short Tab { get; set; }
        public short Number { get; set; }

        public Room CreateRoom(string name, TeamType team, KeyType key, bool allowSpectators, string password = null)
        {
            Room room = null;
            lock (_channelLock)
            {
                for (int i = 0; i < MaxRooms; i++)
                {
                    if (_rooms[i] == null)
                    {
                        room = new Room();
                        _rooms[i] = room;
                        room.Id = i;
                        break;
                    }
                }
            }

            if (room == null)
            {
                return null;
            }

            room.Name = name;
            room.Key = key;
            room.Team = team;
            room.Status = RoomStatusType.Wait;
            room.AllowSpectators = allowSpectators;
            room.SlotsAvailable = 6;
            if (password != null)
            {
                room.Password = password;
                room.HasPassword = true;
            }

            return room;
        }

        public Room GetRoom(int roomId)
        {
            if (roomId >= MaxRooms | roomId < 0)
            {
                return null;
            }

            lock (_channelLock)
            {
                return _rooms[roomId];
            }
        }

        public List<Room> GetRooms()
        {
            List<Room> rooms = new List<Room>();
            lock (_channelLock)
            {
                for (int i = 0; i < MaxRooms; i++)
                {
                    Room room = _rooms[i];
                    if (room == null)
                    {
                        continue;
                    }

                    rooms.Add(room);
                }
            }

            return rooms;
        }
    }
}