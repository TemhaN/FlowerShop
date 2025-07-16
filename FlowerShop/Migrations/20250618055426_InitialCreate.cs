using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlowerShop.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bouquets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bouquets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Flowers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flowers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Toys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    BouquetId = table.Column<int>(type: "INTEGER", nullable: true),
                    FlowerId = table.Column<int>(type: "INTEGER", nullable: true),
                    ToyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favorites_Bouquets_BouquetId",
                        column: x => x.BouquetId,
                        principalTable: "Bouquets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favorites_Flowers_FlowerId",
                        column: x => x.FlowerId,
                        principalTable: "Flowers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favorites_Toys_ToyId",
                        column: x => x.ToyId,
                        principalTable: "Toys",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    BouquetId = table.Column<int>(type: "INTEGER", nullable: true),
                    FlowerId = table.Column<int>(type: "INTEGER", nullable: true),
                    ToyId = table.Column<int>(type: "INTEGER", nullable: true),
                    Comment = table.Column<string>(type: "TEXT", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Bouquets_BouquetId",
                        column: x => x.BouquetId,
                        principalTable: "Bouquets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Reviews_Flowers_FlowerId",
                        column: x => x.FlowerId,
                        principalTable: "Flowers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Reviews_Toys_ToyId",
                        column: x => x.ToyId,
                        principalTable: "Toys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CartId = table.Column<int>(type: "INTEGER", nullable: false),
                    BouquetId = table.Column<int>(type: "INTEGER", nullable: true),
                    FlowerId = table.Column<int>(type: "INTEGER", nullable: true),
                    ToyId = table.Column<int>(type: "INTEGER", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Bouquets_BouquetId",
                        column: x => x.BouquetId,
                        principalTable: "Bouquets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Flowers_FlowerId",
                        column: x => x.FlowerId,
                        principalTable: "Flowers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CartItems_Toys_ToyId",
                        column: x => x.ToyId,
                        principalTable: "Toys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    BouquetId = table.Column<int>(type: "INTEGER", nullable: true),
                    FlowerId = table.Column<int>(type: "INTEGER", nullable: true),
                    ToyId = table.Column<int>(type: "INTEGER", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Bouquets_BouquetId",
                        column: x => x.BouquetId,
                        principalTable: "Bouquets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderItems_Flowers_FlowerId",
                        column: x => x.FlowerId,
                        principalTable: "Flowers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Toys_ToyId",
                        column: x => x.ToyId,
                        principalTable: "Toys",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Bouquets",
                columns: new[] { "Id", "Description", "ImagePath", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 1, "Роскошный букет из алых роз и нежных лилий, украшенный пышной зеленью и атласной лентой. Идеален для выражения глубоких чувств и романтических моментов.", "/images/bouquets/1.jpg", "Романтическое счастье", 14395.20m, 50 },
                    { 2, "Яркий букет из разноцветных тюльпанов и маргариток, создающий атмосферу свежести и радости. Подходит для весенних праздников и подарков близким.", "/images/bouquets/2.jpg", "Весеннее вдохновение", 9595.20m, 30 },
                    { 3, "Солнечный букет из подсолнухов и красных роз, наполненный теплом и энергией. Отличный выбор для ярких и запоминающихся моментов.", "/images/bouquets/3.jpg", "Закатное сияние", 16795.20m, 40 },
                    { 4, "Изысканный букет из белых лилий и роз, символизирующий чистоту и утонченность. Подходит для свадеб и торжественных событий.", "/images/bouquets/4.jpg", "Классическая элегантность", 11995.20m, 60 },
                    { 5, "Пестрый букет из разноцветных цветов, включая герберы и хризантемы, создающий радостное настроение. Для любого праздника.", "/images/bouquets/5.jpg", "Яркая гармония", 11035.20m, 45 },
                    { 6, "Романтичный букет из красных и белых роз, символизирующий страсть и нежность. Украшен атласной лентой и зеленью.", "/images/bouquets/6.jpg", "Вечная любовь", 19195.20m, 35 },
                    { 7, "Легкий букет из маргариток и гвоздик, напоминающий о летних прогулках. Простота и свежесть в каждом цветке.", "/images/bouquets/7.jpg", "Летний бриз", 9115.20m, 50 },
                    { 8, "Нежный букет из пастельных роз и тюльпанов, создающий романтичную и мечтательную атмосферу. Для особых случаев.", "/images/bouquets/8.jpg", "Пастельная мечта", 13435.20m, 25 },
                    { 9, "Экзотический букет из орхидей и лилий, символизирующий утонченность и величие. Идеален для торжественных мероприятий.", "/images/bouquets/9.jpg", "Королевская роскошь", 23995.20m, 20 },
                    { 10, "Яркий букет из садовых цветов, создающий ощущение прогулки по цветущему саду. Отличный подарок для близких.", "/images/bouquets/10.jpg", "Садовая радость", 10555.20m, 55 },
                    { 11, "Изысканный букет из белых орхидей и голубых гортензий, напоминающий звездное небо. Для романтических и торжественных событий.", "/images/bouquets/11.jpg", "Звездное сияние", 21595.20m, 30 },
                    { 12, "Яркий и разнообразный букет из разноцветных роз, гербер и тюльпанов. Подходит для поздравлений и праздников.", "/images/bouquets/12.jpg", "Радужный микс", 12475.20m, 40 },
                    { 13, "Легкий букет из белых хризантем и розовых пионов, создающий атмосферу утренней свежести. Для нежных подарков.", "/images/bouquets/13.jpg", "Нежное утро", 13915.20m, 35 },
                    { 14, "Теплый букет из оранжевых роз и желтых хризантем, напоминающий осенние краски. Для уютных моментов.", "/images/bouquets/14.jpg", "Осенний вальс", 11515.20m, 50 },
                    { 15, "Роскошный букет из экзотических цветов, включая орхидеи и лилии, с яркими акцентами зелени. Для особых событий.", "/images/bouquets/15.jpg", "Цветочная фантазия", 26395.20m, 20 }
                });

            migrationBuilder.InsertData(
                table: "Flowers",
                columns: new[] { "Id", "Description", "ImagePath", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 1, "Классическая алая роза, символ страсти и любви. Идеальна для романтических букетов и подарков.", "/images/flowers/1.jpg", "Алая роза", 1435.20m, 100 },
                    { 2, "Яркий желтый тюльпан, дарящий радость и солнечное настроение. Подходит для весенних композиций.", "/images/flowers/2.jpg", "Желтый тюльпан", 955.20m, 150 },
                    { 3, "Элегантная белая лилия, символизирующая чистоту и утонченность. Для торжественных букетов.", "/images/flowers/3.jpg", "Белая лилия", 1675.20m, 80 },
                    { 4, "Яркий подсолнух, наполненный солнечной энергией. Отличный выбор для позитивных букетов.", "/images/flowers/4.jpg", "Подсолнух", 1195.20m, 120 },
                    { 5, "Экзотическая пурпурная орхидея, добавляющая изысканности любому букету. Для особых случаев.", "/images/flowers/5.jpg", "Орхидея", 2395.20m, 50 },
                    { 6, "Нежная розовая гвоздика, символизирующая благодарность и нежность. Универсальный выбор.", "/images/flowers/6.jpg", "Гвоздика", 859.20m, 200 },
                    { 7, "Простая белая маргаритка, дарящая легкость и свежесть. Подходит для повседневных букетов.", "/images/flowers/7.jpg", "Маргаритка", 619.20m, 180 },
                    { 8, "Пышный розовый пион, символизирующий процветание и романтику. Для роскошных композиций.", "/images/flowers/8.jpg", "Пион", 1915.20m, 70 },
                    { 9, "Синяя гортензия, добавляющая объем и нежность в букеты. Для утонченных подарков.", "/images/flowers/9.jpg", "Гортензия", 2155.20m, 60 },
                    { 10, "Яркая желтая хризантема, символизирующая радость и долголетие. Для ярких композиций.", "/images/flowers/10.jpg", "Хризантема", 1099.20m, 90 },
                    { 11, "Ароматная лаванда с фиолетовыми соцветиями, создающая атмосферу уюта и спокойствия.", "/images/flowers/11.jpg", "Лаванда", 815.20m, 140 },
                    { 12, "Яркая оранжевая гербера, символизирующая радость и энергию. Для ярких букетов.", "/images/flowers/12.jpg", "Гербера", 1275.20m, 110 },
                    { 13, "Синий ирис, символизирующий мудрость и доверие. Идеален для утонченных композиций.", "/images/flowers/13.jpg", "Ирис", 1435.20m, 85 },
                    { 14, "Белый нарцисс с желтым центром, символизирующий обновление и весну. Для свежих букетов.", "/images/flowers/14.jpg", "Нарцисс", 959.20m, 130 },
                    { 15, "Пышный красный георгин, добавляющий яркости и роскоши в любой букет.", "/images/flowers/15.jpg", "Георгин", 1755.20m, 65 }
                });

            migrationBuilder.InsertData(
                table: "Toys",
                columns: new[] { "Id", "Description", "ImagePath", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 1, "Милый коричневый плюшевый медведь, мягкий и уютный. Идеальный подарок для детей и взрослых.", "/images/toys/1.jpg", "Плюшевый медведь", 7675.20m, 20 },
                    { 2, "Нежный белый плюшевый кролик, создающий атмосферу тепла и уюта. Отличный выбор для подарка.", "/images/toys/2.jpg", "Плюшевый кролик", 6235.20m, 25 },
                    { 3, "Очаровательная плюшевая панда, мягкая и приятная на ощупь. Для любителей экзотических животных.", "/images/toys/3.jpg", "Плюшевая панда", 7195.20m, 30 },
                    { 4, "Волшебный плюшевый единорог с блестящим рогом, дарящий сказочное настроение. Для мечтателей.", "/images/toys/4.jpg", "Плюшевый единорог", 8635.20m, 15 },
                    { 5, "Пушистая плюшевая собака, создающая ощущение верного друга. Подходит для детей.", "/images/toys/5.jpg", "Плюшевая собака", 6715.20m, 22 },
                    { 6, "Милая плюшевая кошка с мягкой шерстью, идеальная для обнимашек. Для любителей кошек.", "/images/toys/6.jpg", "Плюшевая кошка", 5755.20m, 28 },
                    { 7, "Серый плюшевый слон с длинным хоботом, дарящий тепло и уют. Для оригинальных подарков.", "/images/toys/7.jpg", "Плюшевый слон", 8155.20m, 18 },
                    { 8, "Величественный плюшевый лев с пышной гривой, символизирующий силу и смелость.", "/images/toys/8.jpg", "Плюшевый лев", 9595.20m, 12 },
                    { 9, "Высокий плюшевый жираф с длинной шеей, создающий радостное настроение. Для детей.", "/images/toys/9.jpg", "Плюшевый жираф", 9115.20m, 15 },
                    { 10, "Игривая плюшевая обезьяна, мягкая и веселая. Отличный подарок для активных детей.", "/images/toys/10.jpg", "Плюшевая обезьяна", 6955.20m, 20 },
                    { 11, "Мягкий плюшевый дельфин, напоминающий о море и приключениях. Для любителей морских животных.", "/images/toys/11.jpg", "Плюшевый дельфин", 7195.20m, 25 },
                    { 12, "Милый плюшевый пингвин, создающий зимнее настроение. Идеален для новогодних подарков.", "/images/toys/12.jpg", "Плюшевый пингвин", 6475.20m, 30 },
                    { 13, "Мудрая плюшевая сова с большими глазами, символизирующая знания и уют.", "/images/toys/13.jpg", "Плюшевая сова", 7675.20m, 20 },
                    { 14, "Маленький плюшевый медвежонок, идеальный для маленьких детей и коллекционеров.", "/images/toys/14.jpg", "Плюшевый медвежонок", 5755.20m, 35 },
                    { 15, "Яркий плюшевый тигр с полосками, символизирующий силу и энергию. Для ярких подарков.", "/images/toys/15.jpg", "Плюшевый тигр", 8635.20m, 18 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "$2a$12$lFPZmwhnNM4ym8cOSV0Na.930m0mqcNX/Cy26x06Br996bRu4YXPW", "Admin", "admin" },
                    { 2, "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", "User", "alina_kz" },
                    { 3, "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", "User", "dmitriy88" },
                    { 4, "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", "User", "sofiya_love" },
                    { 5, "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", "User", "nurzhan_k" },
                    { 6, "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", "User", "ayana_flowers" },
                    { 7, "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", "User", "ermek95" },
                    { 8, "$2a$11$yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", "User", "zarina_smile" }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "BouquetId", "Comment", "CreatedAt", "FlowerId", "Rating", "ToyId", "UserId" },
                values: new object[,]
                {
                    { 1, 1, "Красивый букет, свежие цветы!", new DateTime(2025, 6, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), null, 5, null, 2 },
                    { 2, null, "Роза шикарная, долго стояла.", new DateTime(2025, 6, 2, 12, 0, 0, 0, DateTimeKind.Unspecified), 1, 4, null, 3 },
                    { 3, null, "Медведь очень мягкий, ребенку понравился.", new DateTime(2025, 6, 3, 14, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 1, 2 },
                    { 4, 2, "Яркий и свежий букет, поднял настроение!", new DateTime(2025, 6, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), null, 5, null, 3 },
                    { 5, null, "Лилии пахнут восхитительно.", new DateTime(2025, 6, 5, 11, 0, 0, 0, DateTimeKind.Unspecified), 3, 4, null, 4 },
                    { 6, null, "Кролик такой милый, мягкий!", new DateTime(2025, 6, 6, 13, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 2, 5 },
                    { 7, 4, "Элегантный букет, идеально для свадьбы.", new DateTime(2025, 6, 7, 15, 0, 0, 0, DateTimeKind.Unspecified), null, 5, null, 6 },
                    { 8, null, "Орхидея выглядит экзотично, но быстро завяла.", new DateTime(2025, 6, 8, 10, 0, 0, 0, DateTimeKind.Unspecified), 5, 3, null, 7 },
                    { 9, null, "Единорог стал любимой игрушкой дочки!", new DateTime(2025, 6, 9, 12, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 4, 8 },
                    { 10, 6, "Романтичный букет, идеально для свидания.", new DateTime(2025, 6, 10, 14, 0, 0, 0, DateTimeKind.Unspecified), null, 4, null, 2 },
                    { 11, 3, "Букет превзошёл ожидания, очень яркий!", new DateTime(2025, 6, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), null, 5, null, 3 },
                    { 12, null, "Тюльпаны свежие, но быстро увяли.", new DateTime(2025, 6, 12, 11, 0, 0, 0, DateTimeKind.Unspecified), 2, 3, null, 4 },
                    { 13, null, "Панда такая мягкая, отличный подарок!", new DateTime(2025, 6, 13, 13, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 3, 5 },
                    { 14, 5, "Букет шикарный, но доставка задержалась.", new DateTime(2025, 6, 14, 15, 0, 0, 0, DateTimeKind.Unspecified), null, 4, null, 6 },
                    { 15, null, "Хризантемы долго стояли, рекомендую!", new DateTime(2025, 6, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), 4, 5, null, 7 },
                    { 16, null, "Медведь просто огромный, супер!", new DateTime(2025, 6, 16, 12, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 1, 8 },
                    { 17, 1, "Снова заказал этот букет, всё идеально!", new DateTime(2025, 6, 17, 14, 0, 0, 0, DateTimeKind.Unspecified), null, 5, null, 2 },
                    { 18, null, "Розы как всегда на высоте!", new DateTime(2025, 6, 18, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, 4, null, 3 },
                    { 19, null, "Кролик очень понравился племяннице!", new DateTime(2025, 6, 19, 11, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 2, 4 },
                    { 20, 2, "Букет яркий, но некоторые цветы завяли быстро.", new DateTime(2025, 6, 20, 13, 0, 0, 0, DateTimeKind.Unspecified), null, 3, null, 5 },
                    { 21, null, "Лилии великолепны, аромат потрясающий!", new DateTime(2025, 6, 21, 15, 0, 0, 0, DateTimeKind.Unspecified), 3, 5, null, 6 },
                    { 22, null, "Единорог такой милый, дочка в восторге!", new DateTime(2025, 6, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 4, 7 },
                    { 23, 4, "Букет идеально подошёл для юбилея!", new DateTime(2025, 6, 23, 12, 0, 0, 0, DateTimeKind.Unspecified), null, 5, null, 8 },
                    { 24, null, "Орхидея красивая, но уход сложный.", new DateTime(2025, 6, 24, 14, 0, 0, 0, DateTimeKind.Unspecified), 5, 3, null, 2 },
                    { 25, null, "Панда стала любимой игрушкой сына!", new DateTime(2025, 6, 25, 9, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 3, 3 },
                    { 26, 6, "Букет для свидания был идеальным!", new DateTime(2025, 6, 26, 11, 0, 0, 0, DateTimeKind.Unspecified), null, 4, null, 4 },
                    { 27, null, "Тюльпаны свежие, яркие цвета!", new DateTime(2025, 6, 27, 13, 0, 0, 0, DateTimeKind.Unspecified), 2, 5, null, 5 },
                    { 28, null, "Медведь огромный, качество отличное!", new DateTime(2025, 6, 28, 15, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 1, 6 },
                    { 29, 3, "Букет яркий, доставка быстрая!", new DateTime(2025, 6, 29, 10, 0, 0, 0, DateTimeKind.Unspecified), null, 5, null, 7 },
                    { 30, null, "Хризантемы долго радовали глаз!", new DateTime(2025, 6, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), 4, 5, null, 8 },
                    { 31, null, "Кролик мягкий, идеально для подарка!", new DateTime(2025, 7, 1, 14, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 2, 2 },
                    { 32, 5, "Букет шикарный, но цена высокая.", new DateTime(2025, 7, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), null, 4, null, 3 },
                    { 33, null, "Розы прекрасные, аромат чудесный!", new DateTime(2025, 7, 3, 11, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, null, 4 },
                    { 34, null, "Единорог просто волшебный!", new DateTime(2025, 7, 4, 13, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 4, 5 },
                    { 35, 1, "Букет свежий, красиво оформлен!", new DateTime(2025, 7, 5, 15, 0, 0, 0, DateTimeKind.Unspecified), null, 5, null, 6 },
                    { 36, null, "Лилии пахнут невероятно!", new DateTime(2025, 7, 6, 10, 0, 0, 0, DateTimeKind.Unspecified), 3, 4, null, 7 },
                    { 37, null, "Панда такая милая, качество супер!", new DateTime(2025, 7, 7, 12, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 3, 8 },
                    { 38, 2, "Букет яркий, поднял настроение!", new DateTime(2025, 7, 8, 14, 0, 0, 0, DateTimeKind.Unspecified), null, 5, null, 2 },
                    { 39, null, "Орхидея красивая, но быстро завяла.", new DateTime(2025, 7, 9, 9, 0, 0, 0, DateTimeKind.Unspecified), 5, 3, null, 3 },
                    { 40, null, "Медведь мягкий, ребёнок в восторге!", new DateTime(2025, 7, 10, 11, 0, 0, 0, DateTimeKind.Unspecified), null, 5, 1, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_BouquetId",
                table: "CartItems",
                column: "BouquetId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_FlowerId",
                table: "CartItems",
                column: "FlowerId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ToyId",
                table: "CartItems",
                column: "ToyId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_BouquetId",
                table: "Favorites",
                column: "BouquetId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_FlowerId",
                table: "Favorites",
                column: "FlowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_ToyId",
                table: "Favorites",
                column: "ToyId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_BouquetId",
                table: "OrderItems",
                column: "BouquetId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_FlowerId",
                table: "OrderItems",
                column: "FlowerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ToyId",
                table: "OrderItems",
                column: "ToyId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BouquetId",
                table: "Reviews",
                column: "BouquetId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_FlowerId",
                table: "Reviews",
                column: "FlowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ToyId",
                table: "Reviews",
                column: "ToyId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Bouquets");

            migrationBuilder.DropTable(
                name: "Flowers");

            migrationBuilder.DropTable(
                name: "Toys");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
