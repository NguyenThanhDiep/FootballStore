using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FootballStore.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        public string Name { get; set; }
    }

    public class Role
    {
        public Role()
        {
            Users = new List<User>();
            UsersName = new List<string>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public List<string> UsersName { get; set; }
    }
}