using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Booking.Infrastructure
{
    public class BookingDbContext : DbContext
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
                .WithMany() 
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany() 
                .HasForeignKey(ur => ur.RoleId);

            base.OnModelCreating(modelBuilder);




            base.OnModelCreating(modelBuilder);

    

        }




    }
}
