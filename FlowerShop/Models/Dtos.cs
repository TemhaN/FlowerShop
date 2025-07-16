namespace FlowerShop.Models
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? Password { get; set; }
    }

    public class BouquetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImagePath { get; set; }
    }

    public class FlowerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImagePath { get; set; }
    }

    public class ToyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImagePath { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int? BouquetId { get; set; }
        public int? FlowerId { get; set; }
        public int? ToyId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ItemName { get; set; }
    }

    public class CartDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemDto> CartItems { get; set; }
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int? BouquetId { get; set; }
        public int? FlowerId { get; set; }
        public int? ToyId { get; set; }
        public int Quantity { get; set; }
    }
    public class ReviewDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public int? BouquetId { get; set; }
        public int? FlowerId { get; set; }
        public int? ToyId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class FavoriteDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? BouquetId { get; set; }
        public string? BouquetName { get; set; }
        private string? _bouquetImagePath;
        public string BouquetImagePath
        {
            get => string.IsNullOrEmpty(_bouquetImagePath) || !File.Exists(Path.Combine("wwwroot", _bouquetImagePath.TrimStart('/')))
                ? "/Images/placeholder.png"
                : _bouquetImagePath;
            set => _bouquetImagePath = value;
        }
        public decimal? BouquetPrice { get; set; }
        public int? FlowerId { get; set; }
        public string? FlowerName { get; set; }
        private string? _flowerImagePath;
        public string FlowerImagePath
        {
            get => string.IsNullOrEmpty(_flowerImagePath) || !File.Exists(Path.Combine("wwwroot", _flowerImagePath.TrimStart('/')))
                ? "/Images/placeholder.png"
                : _flowerImagePath;
            set => _flowerImagePath = value;
        }
        public decimal? FlowerPrice { get; set; }
        public int? ToyId { get; set; }
        public string? ToyName { get; set; }
        private string? _toyImagePath;
        public string ToyImagePath
        {
            get => string.IsNullOrEmpty(_toyImagePath) || !File.Exists(Path.Combine("wwwroot", _toyImagePath.TrimStart('/')))
                ? "/Images/placeholder.png"
                : _toyImagePath;
            set => _toyImagePath = value;
        }
        public decimal? ToyPrice { get; set; }
    }

}