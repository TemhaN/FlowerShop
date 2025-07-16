using System.Globalization;
using System.IO;

namespace FlowerShop.Models
{
    public class SearchViewModel
    {
        public string MinPrice { get; set; } = "";
        public string MaxPrice { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IncludeBouquets { get; set; } = true;
        public bool IncludeFlowers { get; set; } = true;
        public bool IncludeToys { get; set; } = true;
        public string SortBy { get; set; } = "NameAsc";
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 12;
        public string? Query { get; set; }

        public decimal? ParsedMinPrice
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MinPrice)) return null;
                return decimal.TryParse(MinPrice.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : null;
            }
        }

        public decimal? ParsedMaxPrice
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MaxPrice)) return null;
                return decimal.TryParse(MaxPrice.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : null;
            }
        }
    }
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        private string _imagePath;
        public string ImagePath
        {
            get => string.IsNullOrEmpty(_imagePath) || !File.Exists($"wwwroot{_imagePath}")
                ? "/Images/placeholder.png"
                : _imagePath;
            set => _imagePath = value;
        }
        public string DetailsUrl { get; set; }
        public string Controller { get; set; }
    }

    public class HomeViewModel
    {
        public List<ProductViewModel> Products { get; set; }
        public SearchViewModel Search { get; set; }
        public bool HasMore { get; set; }
    }
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public int TotalBouquets { get; set; }
        public int TotalFlowers { get; set; }
        public int TotalToys { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalReviews { get; set; }
    }
}