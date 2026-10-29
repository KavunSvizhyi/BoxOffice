using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Models
{
    public class SearchFilterModel
    {
        public string? PerformanceName { get; set; }

        public int? AuthorId { get; set; }

        public int? GenreId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
