using Application.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Music
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Duration { get; set; }
        public MusicalGenre Genre { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        public string FilePath { get; set; }


        // Relation with User (Artist)
        public int ArtistId { get; set; }
        public User? Artist { get; set; }

        // enums of Genres
        public enum MusicalGenre
        {
            Rock = 1,
            Pop = 2,
            Jazz = 3,
            Blues = 4,
            Classical = 5,
            Electronic = 6,
            HipHop = 7,
            Country = 8
            
            
        }
    }
}
