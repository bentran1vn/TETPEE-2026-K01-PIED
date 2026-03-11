using Microsoft.EntityFrameworkCore;
using TetPee.Repository.Entity;

namespace TetPee.Repository;

public class AppDbContext : DbContext
{
    public static Guid UserId1 = Guid.NewGuid(); // Seller
    public static Guid UserId2 = Guid.NewGuid(); // User
    
    public static Guid SellerId1 = Guid.NewGuid();
    
    public static Guid CategoryParentId1 = Guid.NewGuid();
    public static Guid CategoryParentId2 = Guid.NewGuid();
    
    public static Guid ProductId1 = Guid.NewGuid();
    public static Guid ProductId2 = Guid.NewGuid();
    public static Guid ProductId3 = Guid.NewGuid();
    public static Guid ProductId4 = Guid.NewGuid();
    
    public static Guid OrderId1 = Guid.NewGuid();
    public static Guid OrderId2 = Guid.NewGuid();
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductStorage> ProductStorages { get; set; }
    public DbSet<Storage> Storages { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ==================== User Configuration ====================
        modelBuilder.Entity<User>(builder =>
        {
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);
            
            builder.HasIndex(u => u.Email)
                .IsUnique();
            
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            
            // LastName - required, max 100 characters
            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            // builder.Property(u => u.VerifyCode)
            //     .IsRequired();
            
            // ImageUrl - nullable, max 500 characters (URL)
            builder.Property(u => u.ImageUrl)
                .HasMaxLength(500);
            
            // PhoneNumber - nullable, max 20 characters
            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);
            
            // HashedPassword - required, max 500 characters
            builder.Property(u => u.HashedPassword)
                .IsRequired()
                .HasMaxLength(500);
            
            builder.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("User");
            
            // Relationship: User has one Seller (one-to-one)
            builder.HasOne(u => u.Seller)
                .WithOne(s => s.User)
                .HasForeignKey<Seller>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // DeleteBehavior.Cascade: Khi một User bị xóa, thì Seller liên quan cũng sẽ bị xóa theo.
            // DeleteBehavior.Restrict: Ngăn chặn việc xóa một User nếu có Seller liên quan tồn tại.
                //(Tham chiếu tới PK tồn tại)
                // 1 Project còn Task thì không xoá được
            // DeleteBehavior.NoAction: Không thực hiện hành động gì đặc biệt khi User bị xóa. ( Gàn giống Restrict, xử lí ở DB)
            // DeleteBehavior.SetNull: Khi một User bị xóa, thì trường UserId trong bảng Seller sẽ được đặt thành NULL.
                //(Áp dụng khi trường FK cho phép NULL)

            List<User> users = new List<User>()
            {
                new ()
                {
                    Id = UserId1,
                    Email = "tan182205@gmail.com",
                    FirstName = "Tan",
                    LastName = "Tran",
                    HashedPassword = "hashed_password_1",
                },
                new ()
                {
                    Id = UserId2,
                    Email = "tan182206@gmail.com",
                    FirstName = "Tan",
                    LastName = "Tran",
                    HashedPassword = "hashed_password_1",
                }
                ,new ()
                {
                    Id = new Guid("0101b85c-b450-4bb9-8226-0d02b0eb6e03"),
                    Email = "tan182207@gmail.com",
                    FirstName = "Tan",
                    LastName = "Tran",
                    HashedPassword = "hashed_password_1",
                },
                
            };
            
            for(int i = 0; i < 1000; i++)
            {
                var newUser = new User()
                {
                    Id = Guid.NewGuid(),
                    Email = "tan" + i + "@gmail.com",
                    FirstName = "Tan" + i,
                    LastName = "Tran" + i,
                    HashedPassword = "hashed_password_" + i,
                };
                users.Add(newUser);
            }
            
            builder.HasData(users);
        });
        
        modelBuilder.Entity<Seller>(builder =>
        {
            builder.Property(s => s.TaxCode)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(s => s.CompanyName)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Property(s => s.CompanyAddress)
                .IsRequired()
                .HasMaxLength(500);

            var seller = new List<Seller>()
            {
                new ()
                {
                    Id = SellerId1,
                    TaxCode = "TAXCODE123",
                    CompanyName = "ABC Company",
                    CompanyAddress = "123 Main St, Cityville",
                    // UserId = 
                    UserId = UserId1
                },
                new ()
                {
                    Id = Guid.NewGuid(),
                    TaxCode = "TAXCODE123",
                    CompanyName = "ABC Company",
                    CompanyAddress = "123 Main St, Cityville",
                    UserId = new Guid("0101b85c-b450-4bb9-8226-0d02b0eb6e03")
                }
            };
            
            builder.HasData(seller);
        });

        modelBuilder.Entity<Category>(builder =>
        {
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            var categories = new List<Category>()
            {
                new ()
                {
                    Id = CategoryParentId1,
                    Name = "Áo",
                },
                new ()
                {
                    Id = CategoryParentId2,
                    Name = "Quẩn",
                },
                new ()
                {
                    Id = Guid.NewGuid(),
                    Name = "Áo thể thao",
                    ParentId = CategoryParentId1
                },
                new ()
                {
                    Id = Guid.NewGuid(),
                    Name = "Áo ba lỗ",
                    ParentId = CategoryParentId1
                },
                new ()
                {
                Id = Guid.NewGuid(),
                Name = "Quần Jeans",
                ParentId = CategoryParentId2
            }
            };
            
            builder.HasData(categories);
        });
        
        modelBuilder.Entity<Product>(builder =>
        {
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(1000);
            
            builder.Property(p => p.UrlImage)
                .IsRequired()
                .HasMaxLength(500);
            
            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            var products = new List<Product>()
            {
                new Product()
                {
                    Id = ProductId1,
                    Name = "Áo Thun Nam",
                    Description = "Áo thun nam chất liệu cotton cao cấp, thoáng mát, phù hợp cho mọi hoạt động hàng ngày.",
                    UrlImage = "https://example.com/images/ao_thun_nam.jpg",
                    Price = 199000m,
                    SellerId = SellerId1
                },
                new Product()
                {
                    Id = ProductId2,
                    Name = "Quần Jeans Nữ",
                    Description = "Quần jeans nữ dáng ôm, tôn dáng, chất liệu denim co giãn, phù hợp cho mọi dịp.",
                    UrlImage = "https://example.com/images/quan_jeans_nu.jpg",
                    Price = 399000m,
                    SellerId = SellerId1
                },
                new Product()
                {
                    Id = ProductId3,
                    Name = "Áo Sơ Mi Nam",
                    Description = "Áo sơ mi nam công sở, thiết kế hiện đại, chất liệu vải cao cấp, thoáng mát.",
                    UrlImage = "https://example.com/images/ao_so_mi_nam.jpg",
                    Price = 299000m,
                    SellerId = SellerId1
                },
                new Product()
                {
                    Id = ProductId4,
                    Name = "Chân Váy Nữ",
                    Description = "Chân váy nữ xòe, thiết kế trẻ trung, chất liệu vải mềm mại, phù hợp cho mọi dịp.",
                    UrlImage = "https://example.com/images/chan_vay_nu.jpg",
                    Price = 249000m,
                    SellerId = SellerId1
                }
            };
            
            builder.HasData(products);
        });

        modelBuilder.Entity<Order>(builder =>
        {
            var orders = new List<Order>()
            {
                new Order()
                {
                    Id = OrderId1,
                    UserId = UserId2,
                    Address = "Bien Hoa, Dong Nai",
                    TotalAmount = 100000m,
                    Status = "Completed"
                },
                new Order()
                {
                    Id = OrderId2,
                    UserId = UserId2,
                    Address = "Bien Hoa, Dong Nai",
                    TotalAmount = 100000m,
                    Status = "Completed"
                }
            };
            
            builder.HasData(orders);
        });
        
        modelBuilder.Entity<OrderDetail>(builder =>
        {
            var orderDetails = new List<OrderDetail>()
            {
                new OrderDetail()
                {
                    Id = Guid.NewGuid(),
                    OrderId = OrderId1,
                    ProductId = ProductId1,
                    Quantity = 2,
                    UnitPrice = 199000m
                },
                new OrderDetail()
                {
                    Id = Guid.NewGuid(),
                    OrderId = OrderId1,
                    ProductId = ProductId2,
                    Quantity = 1,
                    UnitPrice = 399000m
                },
                new OrderDetail()
                {
                    Id = Guid.NewGuid(),
                    OrderId = OrderId2,
                    ProductId = ProductId3,
                    Quantity = 1,
                    UnitPrice = 299000m
                }
            };
            
            builder.HasData(orderDetails);
        });
    }
}