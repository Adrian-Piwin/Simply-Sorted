using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplySorted.Models
{
    public class Item
    {
        public int id { get; set; }

        public string ownershipId { get; set; }

        public string title { get; set; }

        public string category { get; set; }

        public string description { get; set; }
    }
}
