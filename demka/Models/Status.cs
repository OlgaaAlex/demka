using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace demka.Models
{
    public class Status
    {
        public int StatusId { get; set; }

        public string StatusName { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
