using Domain.Models.Enteties;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace infrastructure
{
    public class DbContextClass : DbContext
    {
        public DbContextClass(DbContextOptions<DbContextClass> contextOptions) : base(contextOptions)
        {

        }

        public DbSet<Morceau> Morceau { get; set; }
        public DbSet<ApiCloud> Api_Cloud { get; set; }
        public DbSet<Playlist> Playlist { get; set; }
        public DbSet<MorceauPlaylist> MorceauPlaylist { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Setting> Setting { get; set; }
        public DbSet<keyvalidation> keyvalidation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MorceauPlaylist>()
                .HasKey(mp => mp.id);

            modelBuilder.Entity<MorceauPlaylist>()
                .HasOne(mp => mp.Morceau)
                .WithMany(m => m.MorceauPlaylists)
                .HasForeignKey(mp => mp.idMorceau)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MorceauPlaylist>()
                .HasOne(mp => mp.Playlist)
                .WithMany(p => p.MorceauPlaylists)
                .HasForeignKey(mp => mp.idPlaylist)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
