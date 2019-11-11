using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootballStore.Models
{
    public class ShoppingCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Amount { get; set; }
        public Order Order { get; set; }
    }
}