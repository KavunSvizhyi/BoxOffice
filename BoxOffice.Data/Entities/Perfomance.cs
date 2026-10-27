using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BoxOffice.Data.Entities
{
    public class Perfomance : BaseEntity
    {
        [StringLength(50)]
        public string PerfomanceName { get; set; } = null!;

        public int AuthorId { get; set; }

        public Author? Author { get; set; }

        public int GenreId { get; set; }

        public Genre? Genre { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EventDate { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = null!;
    }
}
