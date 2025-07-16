using System.Collections.Generic;

namespace FlowerShop.Models
{
    public class ToyDetailsViewModel
    {
        public Toy Toy { get; set; }
        public List<Toy> Recommendations { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}