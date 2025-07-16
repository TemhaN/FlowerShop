namespace FlowerShop.Models
{
    public class ProfileViewModel
    {
        public UserProfileDto UserProfile { get; set; }
        public List<FavoriteDto> Favorites { get; set; }
        public List<OrderDto> Orders { get; set; }
        public Cart Cart { get; set; }
    }
}