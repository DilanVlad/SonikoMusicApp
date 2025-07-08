using Application.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Models;

    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Application.Models.Music> Musics { get; set; } = default!;

        public DbSet<Application.Models.Album> Albums { get; set; } = default!;

        public DbSet<Application.Models.Playlist> Playlists { get; set; } = default!;


        public DbSet<Application.Models.PlaylistMusic> PlaylistMusics { get; set; } = default!;

}
