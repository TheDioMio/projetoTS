using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    
    class Program
    {
       
        static void Main(string[] args)
        {

            using (var db = new ChatContext())
            {
                var User = new User { Name = "Igor",  Username= "asd", Password="123" };
                db.Users.Add(User);

          
                db.SaveChanges();
            }

        }
    }
}
