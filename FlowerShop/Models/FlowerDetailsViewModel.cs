using System.Collections.Generic;

namespace FlowerShop.Models
{
    public class FlowerDetailsViewModel
    {
        public Flower Flower { get; set; }
        public List<Flower> Recommendations { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}