using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Models
{
    public class PerformanceModel
    {
        public int Id { get; set; }

        public string PerformanceName { get; set; } = null!;

        public int AuthorId { get; set; }

        public int GenreId { get; set; }

        public DateTime EventDate { get; set; }

        public ICollection<int> TicketIds { get; set; } = null!;
    }
}
