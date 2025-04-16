using Server.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool State { get; set; }

        public IList<UserRoom> UserRooms { get; set; }

        public IList<Message> Messages { get; set; }
        public User()
        {
            UserRooms = new List<UserRoom>();
            Messages = new List<Message>();
        }

        public User(string name, string username, string password)
        {
            Name = name;
            Username = username;
            Password = password;
            State = false;
            UserRooms = new List<UserRoom>();
            Messages = new List<Message>();
        }
    }
}
