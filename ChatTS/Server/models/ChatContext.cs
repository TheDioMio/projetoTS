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

            modelBuilder.Entity<Message>()
                .HasRequired(m => m.Room)
                .WithMany(r => r.Messages)
                .HasForeignKey(m => m.IdRoom)
                .WillCascadeOnDelete(true); // Permite exclusão em cascata

            modelBuilder.Entity<UserRoom>()
                .HasRequired(ur => ur.Room)
                .WithMany(r => r.UserRooms)
                .HasForeignKey(ur => ur.IdRoom)
                .WillCascadeOnDelete(true); // Permite exclusão em cascata


        }
    }
}
