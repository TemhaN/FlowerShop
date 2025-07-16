namespace FlowerShop.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }

    public class Bouquet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        private string _imagePath;
        public string ImagePath
        {
            get => string.IsNullOrEmpty(_imagePath) || !File.Exists(Path.Combine("wwwroot", _imagePath.TrimStart('/')))
                ? "/Images/placeholder.png"
                : _imagePath;
            set => _imagePath = value;
        }
    }

    public class Flower
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        private string _imagePath;
        public string ImagePath
        {
            get => string.IsNullOrEmpty(_imagePath) || !File.Exists(Path.Combine("wwwroot", _imagePath.TrimStart('/')))
                ? "/Images/placeholder.png"
                : _imagePath;
            set => _imagePath = value;
        }
    }

    public class Toy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        private string _imagePath;
        public string ImagePath
        {
            get => string.IsNullOrEmpty(_imagePath) || !File.Exists(Path.Combine("wwwroot", _imagePath.TrimStart('/')))
                ? "/Images/placeholder.png"
                : _imagePath;
            set => _imagePath = value;
        }
    }
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int? BouquetId { get; set; }
        public Bouquet Bouquet { get; set; }
        public int? FlowerId { get; set; }
        public Flower Flower { get; set; }
        public int? ToyId { get; set; }
        public Toy Toy { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<CartItem> CartItems { get; set; }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int? BouquetId { get; set; }
        public Bouquet Bouquet { get; set; }
        public int? FlowerId { get; set; }
        public Flower Flower { get; set; }
        public int? ToyId { get; set; }
        public Toy Toy { get; set; }
        public int Quantity { get; set; }
    }
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? BouquetId { get; set; }
        public int? FlowerId { get; set; }
        public int? ToyId { get; set; }
        public User User { get; set; }
        public Bouquet Bouquet { get; set; }
        public Flower Flower { get; set; }
        public Toy Toy { get; set; }
    }
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int? BouquetId { get; set; }
        public Bouquet Bouquet { get; set; }
        public int? FlowerId { get; set; }
        public Flower Flower { get; set; }
        public int? ToyId { get; set; }
        public Toy Toy { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; } // 1-5
        public DateTime CreatedAt { get; set; }
    }
}