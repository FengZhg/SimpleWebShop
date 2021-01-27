using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace backEnd
{
    public partial class myShopContext : DbContext
    {
        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] {
            new DebugLoggerProvider()
        });
        public myShopContext()
        {
        }

        public myShopContext(DbContextOptions<myShopContext> options)
            : base(options)
        {
        }

        public virtual DbSet<KeyWordIndexLcx> KeyWordIndexLcx { get; set; }
        public virtual DbSet<OrderIndexLcx> OrderIndexLcx { get; set; }
        public virtual DbSet<OrderLcx> OrderLcx { get; set; }
        public virtual DbSet<Recommend> Recommend { get; set; }
        public virtual DbSet<UserLcx> UserLcx { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseLoggerFactory(MyLoggerFactory).UseMySql("server=172.17.0.1;userid=root;password=root;database=myShop;Port=3306;sslmode=None;charset=utf8",
                    x => {
                        x.ServerVersion("8.0.22-mysql");
                        x.EnableRetryOnFailure();
                    });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KeyWordIndexLcx>(entity =>
            {
                entity.ToTable("KeyWordIndexLCX");

                entity.HasIndex(e => new { e.KeyWord, e.ProductId })
                    .HasName("keyWord")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.KeyWord).HasColumnName("keyWord");

                entity.Property(e => e.ProductId).HasColumnName("productId");
            });

            modelBuilder.Entity<OrderIndexLcx>(entity =>
            {
                entity.ToTable("OrderIndexLCX");

                entity.HasIndex(e => new { e.UserLcxid, e.OrderId })
                    .HasName("UserLCXId")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.OrderId)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UserLcxid)
                    .HasColumnName("UserLCXId")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<OrderLcx>(entity =>
            {
                entity.ToTable("OrderLCX");

                entity.HasIndex(e => e.id)
                    .HasName("id")
                    .IsUnique();

                entity.Property(e => e.id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.buy_count).HasColumnName("buy_count");

                entity.Property(e => e.counts).HasColumnName("counts");

                entity.Property(e => e.goods_id).HasColumnName("goods_id");

                entity.Property(e => e.goods_name)
                    .HasColumnName("goods_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.is_pay)
                    .HasColumnName("is_pay")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.thumb_url)
                    .HasColumnName("thumb_url")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.user_id)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Recommend>(entity =>
            {
                entity.HasKey(e => e.goods_id)
                    .HasName("PRIMARY");

                entity.ToTable("recommend");

                entity.Property(e => e.goods_id)
                    .HasColumnName("goods_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.category).HasColumnName("category");

                entity.Property(e => e.comments_count).HasColumnName("comments_count");

                entity.Property(e => e.counts).HasColumnName("counts");

                entity.Property(e => e.goods_name)
                    .HasColumnName("goods_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.hd_thumb_url)
                    .HasColumnName("hd_thumb_url")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.image_url)
                    .HasColumnName("image_url")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.market_price).HasColumnName("market_price");

                entity.Property(e => e.normal_price).HasColumnName("normal_price");

                entity.Property(e => e.price).HasColumnName("price");

                entity.Property(e => e.sales_tip)
                    .HasColumnName("sales_tip")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.short_name)
                    .HasColumnName("short_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.thumb_url)
                    .HasColumnName("thumb_url")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<UserLcx>(entity =>
            {
                entity.ToTable("UserLCX");

                entity.HasIndex(e => e.Account)
                    .HasName("account")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Account)
                    .HasColumnName("account")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MailAddress)
                    .HasColumnName("mailAddress")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("phoneNumber")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UserPower)
                    .HasColumnName("userPower")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
