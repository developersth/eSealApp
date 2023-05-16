using System;
using System.Collections.Generic;

namespace backend.Models
{
    public partial class Users
    {
        public int Id { get; set; }
        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }
        public bool? IsActive { get; set; }
        public int? RoleId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
    public class Roles
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
