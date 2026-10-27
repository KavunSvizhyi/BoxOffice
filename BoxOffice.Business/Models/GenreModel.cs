using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Models
{
    public class GenreModel
    {
        public int Id { get; set; }

        public string GenreName { get; set; } = null!;

        public ICollection<int> PerfomanceIds { get; set; } = null!;
    }
}
