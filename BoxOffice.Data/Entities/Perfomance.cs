using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Entities
{
    public class Perfomance : BaseEntity
    {
        public string PerfomanceName { get; set; } = null!;
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        public DateTime EventDate { get; set; }
        public ICollection<Ticket> Tickets { get; set; } = null!;
    }
}
