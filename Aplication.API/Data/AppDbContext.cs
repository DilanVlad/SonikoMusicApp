using Application.Models;
using Application.Models.Favorite;
using Application.Models.Identity;
using Application.Models.Implementations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Models.Suscription;

    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Application.Models.Music> Musics { get; set; } = default!;

        public DbSet<Application.Models.Album> Albums { get; set; } = default!;

        public DbSet<Application.Models.Playlist> Playlists { get; set; } = default!;




        public DbSet<Application.Models.Suscription.SubscriptionPlan> SubscriptionPlans { get; set; } = default!;

        public DbSet<Application.Models.Suscription.UserSubscription> UserSubscriptions { get; set; } = default!;

        public DbSet<Application.Models.Implementations.Notification> Notifications { get; set; } = default!;

        public DbSet<Application.Models.Implementations.Download> Downloads { get; set; } = default!;

        public DbSet<Application.Models.Implementations.Follow> Follows { get; set; } = default!;


protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar FavoriteArtist
        modelBuilder.Entity<FavoriteArtist>()
            .HasOne(fa => fa.User)
            .WithMany(u => u.FavoriteArtists)
            .HasForeignKey(fa => fa.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<FavoriteArtist>()
            .HasOne(fa => fa.Artist)
            .WithMany() // Sin navegación inversa
            .HasForeignKey(fa => fa.ArtistId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configurar Follow (mismo problema)
        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Artist)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.ArtistId)
            .OnDelete(DeleteBehavior.NoAction);
    }
        public DbSet<Application.Models.PlaylistMusic> PlaylistMusics { get; set; } = default!;

        public DbSet<Application.Models.Favorite.FavoriteArtist> FavoriteArtists { get; set; } = default!;
        public DbSet<Application.Models.Favorite.FavoriteMusic> FavoriteMusics { get; set; } = default!;
}