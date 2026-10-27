using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Entities
{
    public class Genre : BaseEntity
    {
        public string GenreName { get; set; } = null!;
        public ICollection<Perfomance> Perfomances { get; set; } = null!;
    }
}
