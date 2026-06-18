using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace demka.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        public string SupplierName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}