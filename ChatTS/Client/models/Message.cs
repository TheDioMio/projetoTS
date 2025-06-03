using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    class Message
    {
        public Message()
        {
        }

        public int Id { get; set; }
        public int IdUser { get; set; }
        public User User { get; set; }
        public int IdRoom { get; set; }
        public Room Room { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }

        public Message(int idUser, int idRoom, string text)
        {
            IdUser = idUser;
            IdRoom = idRoom;
            Date = DateTime.Now;
            Text = text;
        }
    }
}
