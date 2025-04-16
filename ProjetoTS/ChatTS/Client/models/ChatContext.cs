using Server.models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    class ChatContext :DbContext
    {
        public ChatContext()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserRoom> UserRooms { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRoom>()
        .HasKey(ur => new { ur.IdUser, ur.IdRoom });
        }
    }
}
