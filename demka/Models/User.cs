using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace demka.Models
{
    public class User
    {
        public int Userid { get; set; }

        public int? Role { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public virtual Role RoleNavigation { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}