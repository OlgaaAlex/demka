using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace demka.Models
{
    public class OrderProduct
    {
        public int OrderProductId { get; set; }

        [Column("ORDER")]                    // ← именно так, как в БД
        public int? OrderId { get; set; }

        [Column("product")]
        public string Product { get; set; } = string.Empty;

        [Column("orderproductcount")]
        public int? OrderProductCount { get; set; }

        public virtual Order OrderNavigation { get; set; }
        public virtual Product ProductNavigation { get; set; }

        [NotMapped]
        public decimal Sum => (ProductNavigation?.ProductPrice ?? 0) * (OrderProductCount ?? 0);
    }
}