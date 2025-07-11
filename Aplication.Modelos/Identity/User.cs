using Application.Models.Favorite;
using Application.Models.Implementations;
using Application.Models.Suscription;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Identity
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // Artist 
        public List<Music>? Musics { get; set; }
        public List<Album>? Albums { get; set; }
        public List<Playlist>? Playlists { get; set; }
        public List<UserSubscription>? Subscriptions { get; set; }
        public List<FavoriteArtist>? FavoriteArtists { get; set; }
        public List<FavoriteMusic>? FavoriteMusics { get; set; }
        public List<Notification>? Notifications { get; set; }
        public List<Download>? Downloads { get; set; }

        [InverseProperty("Follower")]
        public List<Follow>? Following { get; set; } // Artistas que sigue

        [InverseProperty("Artist")]
        public List<Follow>? Followers { get; set; } // Usuarios que lo siguen (si es artista)

    }
}
