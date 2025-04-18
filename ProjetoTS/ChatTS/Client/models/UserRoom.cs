using Server.Models;
using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace Server.models
{
    class UserRoom
    {
        public UserRoom()
        {
        }

        //public int Id { get; set; }
        public int IdUser { get; set; }
        public int IdRoom { get; set; }
        public User User { get; set; }
        public Room Room { get; set; }

        public string UserType { get; set; } // se vai ser Guest ou Admin
        public DateTime DateCreated { get; set; }
        public string UserState { get; set; } // vamos guardar esta variavel, para se quisermos excluir um membro de uma sala por exemplo

        //public UserRoom(int idUser, int idRoom, string userType, string userState)
        public UserRoom(int idUser, int idRoom, string userType, string userState)
        {
            IdUser = idUser;
            IdRoom = idRoom;
            UserType = userType;
            DateCreated = DateTime.Now;
            UserState = userState;
        }
    }
}
