﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootballStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }

        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}