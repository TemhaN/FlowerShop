using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace FlowerShop.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Bouquet> Bouquets { get; set; }
        public DbSet<Flower> Flowers { get; set; }
        public DbSet<Toy> Toys { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Bouquet)
                .WithMany()
                .HasForeignKey(r => r.BouquetId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Flower)
                .WithMany()
                .HasForeignKey(r => r.FlowerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Toy)
                .WithMany()
                .HasForeignKey(r => r.ToyId)
                .OnDelete(DeleteBehavior.SetNull);

            // Пример посева данных для отзывов
            modelBuilder.Entity<Review>().HasData(
                new Review { Id = 1, UserId = 2, BouquetId = 1, Comment = "Красивый букет, свежие цветы!", Rating = 5, CreatedAt = new DateTime(2025, 6, 1, 10, 0, 0) },
                new Review { Id = 2, UserId = 3, FlowerId = 1, Comment = "Роза шикарная, долго стояла.", Rating = 4, CreatedAt = new DateTime(2025, 6, 2, 12, 0, 0) },
                new Review { Id = 3, UserId = 2, ToyId = 1, Comment = "Медведь очень мягкий, ребенку понравился.", Rating = 5, CreatedAt = new DateTime(2025, 6, 3, 14, 0, 0) },
                new Review { Id = 4, UserId = 3, BouquetId = 2, Comment = "Яркий и свежий букет, поднял настроение!", Rating = 5, CreatedAt = new DateTime(2025, 6, 4, 9, 0, 0) },
                new Review { Id = 5, UserId = 4, FlowerId = 3, Comment = "Лилии пахнут восхитительно.", Rating = 4, CreatedAt = new DateTime(2025, 6, 5, 11, 0, 0) },
                new Review { Id = 6, UserId = 5, ToyId = 2, Comment = "Кролик такой милый, мягкий!", Rating = 5, CreatedAt = new DateTime(2025, 6, 6, 13, 0, 0) },
                new Review { Id = 7, UserId = 6, BouquetId = 4, Comment = "Элегантный букет, идеально для свадьбы.", Rating = 5, CreatedAt = new DateTime(2025, 6, 7, 15, 0, 0) },
                new Review { Id = 8, UserId = 7, FlowerId = 5, Comment = "Орхидея выглядит экзотично, но быстро завяла.", Rating = 3, CreatedAt = new DateTime(2025, 6, 8, 10, 0, 0) },
                new Review { Id = 9, UserId = 8, ToyId = 4, Comment = "Единорог стал любимой игрушкой дочки!", Rating = 5, CreatedAt = new DateTime(2025, 6, 9, 12, 0, 0) },
                new Review { Id = 10, UserId = 2, BouquetId = 6, Comment = "Романтичный букет, идеально для свидания.", Rating = 4, CreatedAt = new DateTime(2025, 6, 10, 14, 0, 0) },
                new Review { Id = 11, UserId = 3, BouquetId = 3, Comment = "Букет превзошёл ожидания, очень яркий!", Rating = 5, CreatedAt = new DateTime(2025, 6, 11, 9, 0, 0) },
                new Review { Id = 12, UserId = 4, FlowerId = 2, Comment = "Тюльпаны свежие, но быстро увяли.", Rating = 3, CreatedAt = new DateTime(2025, 6, 12, 11, 0, 0) },
                new Review { Id = 13, UserId = 5, ToyId = 3, Comment = "Панда такая мягкая, отличный подарок!", Rating = 5, CreatedAt = new DateTime(2025, 6, 13, 13, 0, 0) },
                new Review { Id = 14, UserId = 6, BouquetId = 5, Comment = "Букет шикарный, но доставка задержалась.", Rating = 4, CreatedAt = new DateTime(2025, 6, 14, 15, 0, 0) },
                new Review { Id = 15, UserId = 7, FlowerId = 4, Comment = "Хризантемы долго стояли, рекомендую!", Rating = 5, CreatedAt = new DateTime(2025, 6, 15, 10, 0, 0) },
                new Review { Id = 16, UserId = 8, ToyId = 1, Comment = "Медведь просто огромный, супер!", Rating = 5, CreatedAt = new DateTime(2025, 6, 16, 12, 0, 0) },
                new Review { Id = 17, UserId = 2, BouquetId = 1, Comment = "Снова заказал этот букет, всё идеально!", Rating = 5, CreatedAt = new DateTime(2025, 6, 17, 14, 0, 0) },
                new Review { Id = 18, UserId = 3, FlowerId = 1, Comment = "Розы как всегда на высоте!", Rating = 4, CreatedAt = new DateTime(2025, 6, 18, 9, 0, 0) },
                new Review { Id = 19, UserId = 4, ToyId = 2, Comment = "Кролик очень понравился племяннице!", Rating = 5, CreatedAt = new DateTime(2025, 6, 19, 11, 0, 0) },
                new Review { Id = 20, UserId = 5, BouquetId = 2, Comment = "Букет яркий, но некоторые цветы завяли быстро.", Rating = 3, CreatedAt = new DateTime(2025, 6, 20, 13, 0, 0) },
                new Review { Id = 21, UserId = 6, FlowerId = 3, Comment = "Лилии великолепны, аромат потрясающий!", Rating = 5, CreatedAt = new DateTime(2025, 6, 21, 15, 0, 0) },
                new Review { Id = 22, UserId = 7, ToyId = 4, Comment = "Единорог такой милый, дочка в восторге!", Rating = 5, CreatedAt = new DateTime(2025, 6, 22, 10, 0, 0) },
                new Review { Id = 23, UserId = 8, BouquetId = 4, Comment = "Букет идеально подошёл для юбилея!", Rating = 5, CreatedAt = new DateTime(2025, 6, 23, 12, 0, 0) },
                new Review { Id = 24, UserId = 2, FlowerId = 5, Comment = "Орхидея красивая, но уход сложный.", Rating = 3, CreatedAt = new DateTime(2025, 6, 24, 14, 0, 0) },
                new Review { Id = 25, UserId = 3, ToyId = 3, Comment = "Панда стала любимой игрушкой сына!", Rating = 5, CreatedAt = new DateTime(2025, 6, 25, 9, 0, 0) },
                new Review { Id = 26, UserId = 4, BouquetId = 6, Comment = "Букет для свидания был идеальным!", Rating = 4, CreatedAt = new DateTime(2025, 6, 26, 11, 0, 0) },
                new Review { Id = 27, UserId = 5, FlowerId = 2, Comment = "Тюльпаны свежие, яркие цвета!", Rating = 5, CreatedAt = new DateTime(2025, 6, 27, 13, 0, 0) },
                new Review { Id = 28, UserId = 6, ToyId = 1, Comment = "Медведь огромный, качество отличное!", Rating = 5, CreatedAt = new DateTime(2025, 6, 28, 15, 0, 0) },
                new Review { Id = 29, UserId = 7, BouquetId = 3, Comment = "Букет яркий, доставка быстрая!", Rating = 5, CreatedAt = new DateTime(2025, 6, 29, 10, 0, 0) },
                new Review { Id = 30, UserId = 8, FlowerId = 4, Comment = "Хризантемы долго радовали глаз!", Rating = 5, CreatedAt = new DateTime(2025, 6, 30, 12, 0, 0) },
                new Review { Id = 31, UserId = 2, ToyId = 2, Comment = "Кролик мягкий, идеально для подарка!", Rating = 5, CreatedAt = new DateTime(2025, 7, 1, 14, 0, 0) },
                new Review { Id = 32, UserId = 3, BouquetId = 5, Comment = "Букет шикарный, но цена высокая.", Rating = 4, CreatedAt = new DateTime(2025, 7, 2, 9, 0, 0) },
                new Review { Id = 33, UserId = 4, FlowerId = 1, Comment = "Розы прекрасные, аромат чудесный!", Rating = 5, CreatedAt = new DateTime(2025, 7, 3, 11, 0, 0) },
                new Review { Id = 34, UserId = 5, ToyId = 4, Comment = "Единорог просто волшебный!", Rating = 5, CreatedAt = new DateTime(2025, 7, 4, 13, 0, 0) },
                new Review { Id = 35, UserId = 6, BouquetId = 1, Comment = "Букет свежий, красиво оформлен!", Rating = 5, CreatedAt = new DateTime(2025, 7, 5, 15, 0, 0) },
                new Review { Id = 36, UserId = 7, FlowerId = 3, Comment = "Лилии пахнут невероятно!", Rating = 4, CreatedAt = new DateTime(2025, 7, 6, 10, 0, 0) },
                new Review { Id = 37, UserId = 8, ToyId = 3, Comment = "Панда такая милая, качество супер!", Rating = 5, CreatedAt = new DateTime(2025, 7, 7, 12, 0, 0) },
                new Review { Id = 38, UserId = 2, BouquetId = 2, Comment = "Букет яркий, поднял настроение!", Rating = 5, CreatedAt = new DateTime(2025, 7, 8, 14, 0, 0) },
                new Review { Id = 39, UserId = 3, FlowerId = 5, Comment = "Орхидея красивая, но быстро завяла.", Rating = 3, CreatedAt = new DateTime(2025, 7, 9, 9, 0, 0) },
                new Review { Id = 40, UserId = 4, ToyId = 1, Comment = "Медведь мягкий, ребёнок в восторге!", Rating = 5, CreatedAt = new DateTime(2025, 7, 10, 11, 0, 0) }
            );
            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "$2a$12$lFPZmwhnNM4ym8cOSV0Na.930m0mqcNX/Cy26x06Br996bRu4YXPW",
                    Role = "Admin"
                },
                new User
                {
                    Id = 2,
                    Username = "alina_kz",
                    PasswordHash = "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy",
                    Role = "User"
                },
                new User
                {
                    Id = 3,
                    Username = "dmitriy88",
                    PasswordHash = "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy",
                    Role = "User"
                },
                new User
                {
                    Id = 4,
                    Username = "sofiya_love",
                    PasswordHash = "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy",
                    Role = "User"
                },
                new User
                {
                    Id = 5,
                    Username = "nurzhan_k",
                    PasswordHash = "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy",
                    Role = "User"
                },
                new User
                {
                    Id = 6,
                    Username = "ayana_flowers",
                    PasswordHash = "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy",
                    Role = "User"
                },
                new User
                {
                    Id = 7,
                    Username = "ermek95",
                    PasswordHash = "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy",
                    Role = "User"
                },
                new User
                {
                    Id = 8,
                    Username = "zarina_smile",
                    PasswordHash = "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy",
                    Role = "User"
                }
            );

            // Seed Bouquets
            modelBuilder.Entity<Bouquet>().HasData(
                new Bouquet { Id = 1, Name = "Романтическое счастье", Description = "Роскошный букет из алых роз и нежных лилий, украшенный пышной зеленью и атласной лентой. Идеален для выражения глубоких чувств и романтических моментов.", Price = 14395.20m, Stock = 50, ImagePath = "/images/bouquets/1.jpg" },
                new Bouquet { Id = 2, Name = "Весеннее вдохновение", Description = "Яркий букет из разноцветных тюльпанов и маргариток, создающий атмосферу свежести и радости. Подходит для весенних праздников и подарков близким.", Price = 9595.20m, Stock = 30, ImagePath = "/images/bouquets/2.jpg" },
                new Bouquet { Id = 3, Name = "Закатное сияние", Description = "Солнечный букет из подсолнухов и красных роз, наполненный теплом и энергией. Отличный выбор для ярких и запоминающихся моментов.", Price = 16795.20m, Stock = 40, ImagePath = "/images/bouquets/3.jpg" },
                new Bouquet { Id = 4, Name = "Классическая элегантность", Description = "Изысканный букет из белых лилий и роз, символизирующий чистоту и утонченность. Подходит для свадеб и торжественных событий.", Price = 11995.20m, Stock = 60, ImagePath = "/images/bouquets/4.jpg" },
                new Bouquet { Id = 5, Name = "Яркая гармония", Description = "Пестрый букет из разноцветных цветов, включая герберы и хризантемы, создающий радостное настроение. Для любого праздника.", Price = 11035.20m, Stock = 45, ImagePath = "/images/bouquets/5.jpg" },
                new Bouquet { Id = 6, Name = "Вечная любовь", Description = "Романтичный букет из красных и белых роз, символизирующий страсть и нежность. Украшен атласной лентой и зеленью.", Price = 19195.20m, Stock = 35, ImagePath = "/images/bouquets/6.jpg" },
                new Bouquet { Id = 7, Name = "Летний бриз", Description = "Легкий букет из маргариток и гвоздик, напоминающий о летних прогулках. Простота и свежесть в каждом цветке.", Price = 9115.20m, Stock = 50, ImagePath = "/images/bouquets/7.jpg" },
                new Bouquet { Id = 8, Name = "Пастельная мечта", Description = "Нежный букет из пастельных роз и тюльпанов, создающий романтичную и мечтательную атмосферу. Для особых случаев.", Price = 13435.20m, Stock = 25, ImagePath = "/images/bouquets/8.jpg" },
                new Bouquet { Id = 9, Name = "Королевская роскошь", Description = "Экзотический букет из орхидей и лилий, символизирующий утонченность и величие. Идеален для торжественных мероприятий.", Price = 23995.20m, Stock = 20, ImagePath = "/images/bouquets/9.jpg" },
                new Bouquet { Id = 10, Name = "Садовая радость", Description = "Яркий букет из садовых цветов, создающий ощущение прогулки по цветущему саду. Отличный подарок для близких.", Price = 10555.20m, Stock = 55, ImagePath = "/images/bouquets/10.jpg" },
                new Bouquet { Id = 11, Name = "Звездное сияние", Description = "Изысканный букет из белых орхидей и голубых гортензий, напоминающий звездное небо. Для романтических и торжественных событий.", Price = 21595.20m, Stock = 30, ImagePath = "/images/bouquets/11.jpg" },
                new Bouquet { Id = 12, Name = "Радужный микс", Description = "Яркий и разнообразный букет из разноцветных роз, гербер и тюльпанов. Подходит для поздравлений и праздников.", Price = 12475.20m, Stock = 40, ImagePath = "/images/bouquets/12.jpg" },
                new Bouquet { Id = 13, Name = "Нежное утро", Description = "Легкий букет из белых хризантем и розовых пионов, создающий атмосферу утренней свежести. Для нежных подарков.", Price = 13915.20m, Stock = 35, ImagePath = "/images/bouquets/13.jpg" },
                new Bouquet { Id = 14, Name = "Осенний вальс", Description = "Теплый букет из оранжевых роз и желтых хризантем, напоминающий осенние краски. Для уютных моментов.", Price = 11515.20m, Stock = 50, ImagePath = "/images/bouquets/14.jpg" },
                new Bouquet { Id = 15, Name = "Цветочная фантазия", Description = "Роскошный букет из экзотических цветов, включая орхидеи и лилии, с яркими акцентами зелени. Для особых событий.", Price = 26395.20m, Stock = 20, ImagePath = "/images/bouquets/15.jpg" }
            );

            // Seed Flowers
            modelBuilder.Entity<Flower>().HasData(
                new Flower { Id = 1, Name = "Алая роза", Description = "Классическая алая роза, символ страсти и любви. Идеальна для романтических букетов и подарков.", Price = 1435.20m, Stock = 100, ImagePath = "/images/flowers/1.jpg" },
                new Flower { Id = 2, Name = "Желтый тюльпан", Description = "Яркий желтый тюльпан, дарящий радость и солнечное настроение. Подходит для весенних композиций.", Price = 955.20m, Stock = 150, ImagePath = "/images/flowers/2.jpg" },
                new Flower { Id = 3, Name = "Белая лилия", Description = "Элегантная белая лилия, символизирующая чистоту и утонченность. Для торжественных букетов.", Price = 1675.20m, Stock = 80, ImagePath = "/images/flowers/3.jpg" },
                new Flower { Id = 4, Name = "Подсолнух", Description = "Яркий подсолнух, наполненный солнечной энергией. Отличный выбор для позитивных букетов.", Price = 1195.20m, Stock = 120, ImagePath = "/images/flowers/4.jpg" },
                new Flower { Id = 5, Name = "Орхидея", Description = "Экзотическая пурпурная орхидея, добавляющая изысканности любому букету. Для особых случаев.", Price = 2395.20m, Stock = 50, ImagePath = "/images/flowers/5.jpg" },
                new Flower { Id = 6, Name = "Гвоздика", Description = "Нежная розовая гвоздика, символизирующая благодарность и нежность. Универсальный выбор.", Price = 859.20m, Stock = 200, ImagePath = "/images/flowers/6.jpg" },
                new Flower { Id = 7, Name = "Маргаритка", Description = "Простая белая маргаритка, дарящая легкость и свежесть. Подходит для повседневных букетов.", Price = 619.20m, Stock = 180, ImagePath = "/images/flowers/7.jpg" },
                new Flower { Id = 8, Name = "Пион", Description = "Пышный розовый пион, символизирующий процветание и романтику. Для роскошных композиций.", Price = 1915.20m, Stock = 70, ImagePath = "/images/flowers/8.jpg" },
                new Flower { Id = 9, Name = "Гортензия", Description = "Синяя гортензия, добавляющая объем и нежность в букеты. Для утонченных подарков.", Price = 2155.20m, Stock = 60, ImagePath = "/images/flowers/9.jpg" },
                new Flower { Id = 10, Name = "Хризантема", Description = "Яркая желтая хризантема, символизирующая радость и долголетие. Для ярких композиций.", Price = 1099.20m, Stock = 90, ImagePath = "/images/flowers/10.jpg" },
                new Flower { Id = 11, Name = "Лаванда", Description = "Ароматная лаванда с фиолетовыми соцветиями, создающая атмосферу уюта и спокойствия.", Price = 815.20m, Stock = 140, ImagePath = "/images/flowers/11.jpg" },
                new Flower { Id = 12, Name = "Гербера", Description = "Яркая оранжевая гербера, символизирующая радость и энергию. Для ярких букетов.", Price = 1275.20m, Stock = 110, ImagePath = "/images/flowers/12.jpg" },
                new Flower { Id = 13, Name = "Ирис", Description = "Синий ирис, символизирующий мудрость и доверие. Идеален для утонченных композиций.", Price = 1435.20m, Stock = 85, ImagePath = "/images/flowers/13.jpg" },
                new Flower { Id = 14, Name = "Нарцисс", Description = "Белый нарцисс с желтым центром, символизирующий обновление и весну. Для свежих букетов.", Price = 959.20m, Stock = 130, ImagePath = "/images/flowers/14.jpg" },
                new Flower { Id = 15, Name = "Георгин", Description = "Пышный красный георгин, добавляющий яркости и роскоши в любой букет.", Price = 1755.20m, Stock = 65, ImagePath = "/images/flowers/15.jpg" }
            );

            // Seed Toys
            modelBuilder.Entity<Toy>().HasData(
                new Toy { Id = 1, Name = "Плюшевый медведь", Description = "Милый коричневый плюшевый медведь, мягкий и уютный. Идеальный подарок для детей и взрослых.", Price = 7675.20m, Stock = 20, ImagePath = "/images/toys/1.jpg" },
                new Toy { Id = 2, Name = "Плюшевый кролик", Description = "Нежный белый плюшевый кролик, создающий атмосферу тепла и уюта. Отличный выбор для подарка.", Price = 6235.20m, Stock = 25, ImagePath = "/images/toys/2.jpg" },
                new Toy { Id = 3, Name = "Плюшевая панда", Description = "Очаровательная плюшевая панда, мягкая и приятная на ощупь. Для любителей экзотических животных.", Price = 7195.20m, Stock = 30, ImagePath = "/images/toys/3.jpg" },
                new Toy { Id = 4, Name = "Плюшевый единорог", Description = "Волшебный плюшевый единорог с блестящим рогом, дарящий сказочное настроение. Для мечтателей.", Price = 8635.20m, Stock = 15, ImagePath = "/images/toys/4.jpg" },
                new Toy { Id = 5, Name = "Плюшевая собака", Description = "Пушистая плюшевая собака, создающая ощущение верного друга. Подходит для детей.", Price = 6715.20m, Stock = 22, ImagePath = "/images/toys/5.jpg" },
                new Toy { Id = 6, Name = "Плюшевая кошка", Description = "Милая плюшевая кошка с мягкой шерстью, идеальная для обнимашек. Для любителей кошек.", Price = 5755.20m, Stock = 28, ImagePath = "/images/toys/6.jpg" },
                new Toy { Id = 7, Name = "Плюшевый слон", Description = "Серый плюшевый слон с длинным хоботом, дарящий тепло и уют. Для оригинальных подарков.", Price = 8155.20m, Stock = 18, ImagePath = "/images/toys/7.jpg" },
                new Toy { Id = 8, Name = "Плюшевый лев", Description = "Величественный плюшевый лев с пышной гривой, символизирующий силу и смелость.", Price = 9595.20m, Stock = 12, ImagePath = "/images/toys/8.jpg" },
                new Toy { Id = 9, Name = "Плюшевый жираф", Description = "Высокий плюшевый жираф с длинной шеей, создающий радостное настроение. Для детей.", Price = 9115.20m, Stock = 15, ImagePath = "/images/toys/9.jpg" },
                new Toy { Id = 10, Name = "Плюшевая обезьяна", Description = "Игривая плюшевая обезьяна, мягкая и веселая. Отличный подарок для активных детей.", Price = 6955.20m, Stock = 20, ImagePath = "/images/toys/10.jpg" },
                new Toy { Id = 11, Name = "Плюшевый дельфин", Description = "Мягкий плюшевый дельфин, напоминающий о море и приключениях. Для любителей морских животных.", Price = 7195.20m, Stock = 25, ImagePath = "/images/toys/11.jpg" },
                new Toy { Id = 12, Name = "Плюшевый пингвин", Description = "Милый плюшевый пингвин, создающий зимнее настроение. Идеален для новогодних подарков.", Price = 6475.20m, Stock = 30, ImagePath = "/images/toys/12.jpg" },
                new Toy { Id = 13, Name = "Плюшевая сова", Description = "Мудрая плюшевая сова с большими глазами, символизирующая знания и уют.", Price = 7675.20m, Stock = 20, ImagePath = "/images/toys/13.jpg" },
                new Toy { Id = 14, Name = "Плюшевый медвежонок", Description = "Маленький плюшевый медвежонок, идеальный для маленьких детей и коллекционеров.", Price = 5755.20m, Stock = 35, ImagePath = "/images/toys/14.jpg" },
                new Toy { Id = 15, Name = "Плюшевый тигр", Description = "Яркий плюшевый тигр с полосками, символизирующий силу и энергию. Для ярких подарков.", Price = 8635.20m, Stock = 18, ImagePath = "/images/toys/15.jpg" }
            );
        }
    }
}