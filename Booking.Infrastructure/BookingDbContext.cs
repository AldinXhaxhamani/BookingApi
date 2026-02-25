using Booking.Application;
using Booking.Domain.Apartments;
using Booking.Domain.Entities;
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
        public DbSet<Domain.Entities.Booking> Bookings { get; set; }

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






            base.OnModelCreating(modelBuilder);

        }
    }
}
