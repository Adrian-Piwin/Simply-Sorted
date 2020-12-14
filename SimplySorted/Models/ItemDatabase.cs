using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

// Adrian Piwin

namespace SimplySorted.Models
{
    public class ItemDatabase : IdentityDbContext
    {

        public DbSet<Item> Items { get; set; }


        public ItemDatabase(DbContextOptions<ItemDatabase> options) : base(options)
        {

        }
    }
}
