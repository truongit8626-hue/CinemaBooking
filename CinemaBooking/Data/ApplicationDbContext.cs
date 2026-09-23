using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CinemaBooking.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }


        // 🔥 THÊM ĐOẠN NÀY
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Seat>()
            .HasOne(s => s.Showtime)
            .WithMany()
            .HasForeignKey(s => s.ShowtimeId)
            .OnDelete(DeleteBehavior.Restrict);
           

            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.Room)
                .WithMany(r => r.Showtimes)
                .HasForeignKey(s => s.RoomId);

            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.Movie)
                .WithMany()
                .HasForeignKey(s => s.MovieId);
            // UserPreference -> ApplicationUser
            modelBuilder.Entity<UserPreference>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Không cho một user lưu trùng cùng một thể loại
            modelBuilder.Entity<UserPreference>()
                .HasIndex(p => new { p.UserId, p.Genre })
                .IsUnique();
        }
        
    }
}