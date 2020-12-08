using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Adrian Piwin

namespace SimplySorted.Models
{
    public class ItemDatabase : DbContext
    {

        public DbSet<Item> Items { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\ProjectsV13;Initial Catalog=ItemDatabase;Integrated Security=True;");
        }
    }
}
