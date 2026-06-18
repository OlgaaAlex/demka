using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace demka.Models
{
    public class OrderProduct
    {
        public int OrderProductId { get; set; }

        public int? OrderId { get; set; }

        public string Product { get; set; }

        public int? OrderProductCount { get; set; }

        public virtual Order OrderNavigation { get; set; }

        public virtual Product ProductNavigation { get; set; }
    }
}
