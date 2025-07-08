using Application.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? CoverImagePath { get; set; }

        // Artista
        public int ArtistId { get; set; }
        public User? Artist { get; set; }

        
        public List<Music>? Musics { get; set; }
    }
}
