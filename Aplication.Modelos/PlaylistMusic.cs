using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class PlaylistMusic
    {
        public int Id { get; set; }

        public int PlaylistId { get; set; }
        public Playlist? Playlist { get; set; }

        public int MusicId { get; set; }
        public Music? Music { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.Now;
        public int Order { get; set; }
    }
}
