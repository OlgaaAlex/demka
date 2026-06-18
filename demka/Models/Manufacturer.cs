using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace demka.Models
{
    public class Manufacturer
    {
        public int ManufacturerId { get; set; }

        public string ManufacturerName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
