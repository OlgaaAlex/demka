using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace demka.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int? OrderStatus { get; set; }

        public int? OrderAddress { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? OrderDateIssue { get; set; }

        public int? OrderUser { get; set; }

        public int? OrderCode { get; set; }

        public virtual Status StatusNavigation { get; set; }

        public virtual Address AddressNavigation { get; set; }

        public virtual User UserNavigation { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
