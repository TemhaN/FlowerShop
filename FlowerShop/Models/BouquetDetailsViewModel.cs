using System.Collections.Generic;

namespace FlowerShop.Models
{
    public class BouquetDetailsViewModel
    {
        public Bouquet Bouquet { get; set; }
        public List<Bouquet> Recommendations { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}