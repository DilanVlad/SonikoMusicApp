using Application.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Favorite
{
    public class FavoriteMusic
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MusicId { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;

        // Navegación
        public User? User { get; set; }
        public Music? Music { get; set; }
    }
}
