using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace demka.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        public string AddressName { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
