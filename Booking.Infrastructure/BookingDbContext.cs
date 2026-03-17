using Booking.Application;
using Booking.Domain.Apartments;
using Booking.Domain.Bookings;
using Booking.Domain.Entities;
using Booking.Domain.Review;
using Booking.Domain.Reviews;
using Booking.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Booking.Infrastructure
{
    public class BookingDbContext : DbContext, IApplicationContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> op) : base(op){ 
        }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OwnerProfile> Owners { get; set; }    
        public DbSet<UserRole> UserRoles { get; set; }   
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Review> Reviews { get; set; }
       
        public DbSet<BookingEntity> Bookings { get; set; }

        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }

        public DbSet<PropertyMonthlyAvailability> PropertyAvailabilities { get; set; }
        public DbSet<PropertySeasonPrice> PropertySeasonPrices { get; set; }
        public DbSet<PropertyDiscount> PropertyDiscounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());



            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles) 
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(u => u.UserRoles) 
                .HasForeignKey(ur => ur.RoleId);


            modelBuilder.Entity<Property>()
                .HasOne(p => p.Owner)
                .WithMany()
                .HasForeignKey(p => p.OwnerId)      
                .HasPrincipalKey(o => o.UserId);



            modelBuilder.Entity<BlacklistedToken>(entity =>
            {
                entity.HasKey(b => b.Id);
                
                entity.HasIndex(b => b.Token)
                      .IsUnique();
            });

            modelBuilder.Entity<PropertyMonthlyAvailability>()
                .HasOne(a => a.Property)
                .WithMany()
                .HasForeignKey(a => a.PropertyId);

            // 1 rresht per cdo property per nje muaj per nje vit
            modelBuilder.Entity<PropertyMonthlyAvailability>()
                .HasIndex(a => new { a.PropertyId, a.Year, a.Month })
                .IsUnique();


            modelBuilder.Entity<PropertySeasonPrice>()
                .HasOne(s => s.Property)
                .WithMany()
                .HasForeignKey(s => s.PropertyId);

            modelBuilder.Entity<PropertySeasonPrice>()
                .Property(s => s.PricePerNight)
                .HasPrecision(18, 2);

            // only one row per property per season
            modelBuilder.Entity<PropertySeasonPrice>()
                .HasIndex(s => new { s.PropertyId, s.Season })
                .IsUnique();

            modelBuilder.Entity<PropertyDiscount>()
                .HasOne(d => d.Property)
                .WithMany()
                .HasForeignKey(d => d.PropertyId);

            modelBuilder.Entity<PropertyDiscount>()
                .Property(d => d.DiscountPerNight)
                .HasPrecision(18, 2);

            // only one discount per property
            modelBuilder.Entity<PropertyDiscount>()
                .HasIndex(d => d.PropertyId)
                .IsUnique();


            base.OnModelCreating(modelBuilder);

        }
    }
}
