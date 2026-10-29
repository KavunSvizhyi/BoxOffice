using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Data.Entities
{
    public class Author : BaseEntity
    {
        public int PersonId { get; set; }

        public Person Person { get; set; } = null!;

        public ICollection<Performance> Performances { get; set; } = null!;
    }
}
