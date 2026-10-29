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

            // Mỗi Seat được tạo riêng cho một Showtime. Vì vậy một Seat chỉ
            // được xuất hiện trong tối đa một BookingDetail, kể cả khi có các
            // request đặt vé đến đồng thời.
            modelBuilder.Entity<BookingDetail>()
                .HasIndex(d => d.SeatId)
                .IsUnique();

            // Giữ tương thích với schema Identity đã có trong CSDL. Nếu không
            // cấu hình rõ, EF Core 10 sẽ đề xuất đổi các cột này sang 450 ký tự.
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>()
                .Property(login => login.LoginProvider)
                .HasMaxLength(128);
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>()
                .Property(login => login.ProviderKey)
                .HasMaxLength(128);
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>()
                .Property(token => token.LoginProvider)
                .HasMaxLength(128);
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>()
                .Property(token => token.Name)
                .HasMaxLength(128);
           

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
