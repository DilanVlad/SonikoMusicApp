using Application.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Favorite
{
    public class FavoriteArtist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ArtistId { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;

        // Navegation
        public User? User { get; set; }
        public User? Artist { get; set; }
    }
}
