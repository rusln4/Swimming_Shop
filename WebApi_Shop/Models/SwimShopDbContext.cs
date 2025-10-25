using Microsoft.EntityFrameworkCore;

namespace WebApi_Shop.Models;

public partial class SwimShopDbContext : DbContext
{
    public SwimShopDbContext()
    {
    }

    public SwimShopDbContext(DbContextOptions<SwimShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Убрана директива #warning и добавлена защита от переопределения настроек,
        // если DbContext зарегистрирован через DI (AddDbContext)
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(
                "server=localhost;user=root;password=root;database=swim_shop_db",
                ServerVersion.Parse("8.0.42-mysql"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.IdCartItem).HasName("PRIMARY");

            entity.ToTable("cart_items");

            entity.HasIndex(e => e.IdProductCart, "fk_id_product_cart_idx");

            entity.HasIndex(e => e.IdUserCart, "fk_id_user_cart_idx");

            entity.HasIndex(e => new { e.IdUserCart, e.IdProductCart }, "unique_user_product").IsUnique();

            entity.Property(e => e.IdCartItem).HasColumnName("id_cart_item");
            entity.Property(e => e.AddedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("added_date");
            entity.Property(e => e.IdProductCart).HasColumnName("id_product_cart");
            entity.Property(e => e.IdUserCart).HasColumnName("id_user_cart");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasColumnName("quantity");

            entity.HasOne(d => d.IdProductCartNavigation).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.IdProductCart)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_id_product_cart");

            entity.HasOne(d => d.IdUserCartNavigation).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.IdUserCart)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_id_user_cart");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategories).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.Property(e => e.IdCategories).HasColumnName("id_categories");
            entity.Property(e => e.NameCategory)
                .HasMaxLength(100)
                .HasColumnName("name_category");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.IdUser, "fk_order_user_idx");

            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','confirmed','shipped','delivered','cancelled')")
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_user");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.IdOrderItem).HasName("PRIMARY");

            entity.ToTable("order_items");

            entity.HasIndex(e => e.IdOrder, "fk_order_item_order_idx");

            entity.HasIndex(e => e.IdProduct, "fk_order_item_product_idx");

            entity.Property(e => e.IdOrderItem).HasColumnName("id_order_item");
            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.IdOrder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_item_order");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_item_product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("PRIMARY");

            entity.ToTable("products");

            entity.HasIndex(e => e.IdCategoryProduct, "fk_category_idx");

            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.CountProduct).HasColumnName("count_product");
            entity.Property(e => e.DescriptionProduct)
                .HasColumnType("text")
                .HasColumnName("description_product");
            entity.Property(e => e.IdCategoryProduct).HasColumnName("id_category_product");
            entity.Property(e => e.NameProduct)
                .HasMaxLength(100)
                .HasColumnName("name_product");
            entity.Property(e => e.PictureProduct)
                .HasMaxLength(500)
                .HasColumnName("picture_product");
            entity.Property(e => e.PriceProduct)
                .HasPrecision(10, 2)
                .HasColumnName("price_product");

            entity.HasOne(d => d.IdCategoryProductNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdCategoryProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_category");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUsers).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.RoleUser, "fk_user_role_idx");

            entity.Property(e => e.IdUsers).HasColumnName("id_users");
            entity.Property(e => e.AddressUser)
                .HasMaxLength(200)
                .HasColumnName("address_user");
            entity.Property(e => e.LastnameUser)
                .HasMaxLength(45)
                .HasColumnName("lastname_user");
            entity.Property(e => e.MailUser)
                .HasMaxLength(45)
                .HasColumnName("mail_user");
            entity.Property(e => e.NameUser)
                .HasMaxLength(45)
                .HasColumnName("name_user");
            entity.Property(e => e.PasswordUser)
                .HasMaxLength(45)
                .HasColumnName("password_user");
            entity.Property(e => e.PhoneUser)
                .HasMaxLength(45)
                .HasColumnName("phone_user");
            entity.Property(e => e.RoleUser).HasColumnName("role_user");

            entity.HasOne(d => d.RoleUserNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_role");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.IdUserRoles).HasName("PRIMARY");

            entity.ToTable("user_roles");

            entity.Property(e => e.IdUserRoles).HasColumnName("id_user_roles");
            entity.Property(e => e.NameUserRole)
                .HasMaxLength(45)
                .HasColumnName("name_user_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
