using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace demka.Models
{
    public class Product
    {
        public string ProductArticul { get; set; }

        public string ProductName { get; set; }

        public string ProductUnit { get; set; }

        public decimal ProductPrice { get; set; }

        public int? Supplier { get; set; }

        public int? Manufacturer { get; set; }

        public int? Category { get; set; }

        public int? Discount { get; set; }

        public int? CountInStock { get; set; }

        public string Description { get; set; }

        public byte[] PhotoPath { get; set; }

        public virtual Supplier SupplierNavigation { get; set; }

        public virtual Manufacturer ManufacturerNavigation { get; set; }

        public virtual Category CategoryNavigation { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }



        public bool IsDiscountGreaterThan15
            => Discount.HasValue && Discount.Value > 15;
    }
}