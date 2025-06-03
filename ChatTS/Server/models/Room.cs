using Server.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    class Room
    {
        public Room()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public IList<UserRoom> UserRooms { get; set; }
        public IList<Message> Messages { get; set; }

        public Room(string name)
        {
            Name = name;
            UserRooms = new List<UserRoom>();
            Messages = new List<Message>();
        }
    }
}
